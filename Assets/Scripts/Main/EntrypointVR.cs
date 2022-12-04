using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace VROmics.Main
{

    /// <summary>
    /// This represents the entrypoint from the initial Desktop configuration to a VR, interactable reconfiguration.
    /// The reconfiguration is executed automatically, given conditions are met.
    /// 
    /// Author: Dimitar Garkov
    /// </summary>
    public class EntrypointVR : MonoBehaviour
    {
        public static EntrypointVR Instance { get; private set; }

        void Awake()
        {
            gameObject.hideFlags = hideFlags |= HideFlags.HideAndDontSave | HideFlags.HideInHierarchy;
            Instance = this;
        }

        private void Start()
        {
            //StartCoroutine(DetectHMD());
        }

        internal IEnumerator DetectHMD()
        {
            var HMDs = new List<InputDevice>();
            while (true)
            {
                InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeadMounted, HMDs);
                foreach (var hmd in HMDs)
                {
                    if (!hmd.isValid)
                        continue;

                    Debug.Log("Detected a valid HMD:" + hmd.name);
                    HMD = hmd;
                    VR = true;
                    string currentScene = SceneManager.GetActiveScene().name;
                    switch (currentScene)
                    {
                        case "Pipeline":
                            StartCoroutine(InitializeVR(Reconfigure()));
                            SceneManager.sceneLoaded += (scene, mode) =>
                            {
                                if (scene.name != "Visualisation")
                                    return;
                                StartCoroutine(ReconfigureVisualization());
                            };
                            break;
                        case "Visualisation":
                            StartCoroutine(InitializeVR(ReconfigureVisualization(true)));
                            SpotDrawer = GameObject.Find("ScriptHolder").GetComponent<SpotDrawer>();
                            StartCoroutine(TransformSpots2VRCanvas());
                            break;
                    }
                    yield break;
                }

                yield return new WaitForEndOfFrame();
            }
        }

        private void Update()
        {
            OnUpdate?.Invoke();
        }

        private IEnumerator InitializeVR(IEnumerator sceneCoroutine)
        {
            // take down default camera
            Destroy(GameObject.Find("Main Camera"));
            //XR IM needs to be add first to avoid XR IT automatically adding one
            XRInteractionManager = InstantiatePrefab("XR Interaction Manager");
            XROrigin = InstantiatePrefab("XR Origin");

            // Input Action Manager
            InputActionManager = InstantiatePrefab("Input Action Manager");

            yield return StartCoroutine(sceneCoroutine);
        }

        private IEnumerator Reconfigure()
        {
            GameObject Canvas = GameObject.Find("Canvas");

            {//Configure canvas
                Canvas canvas = Canvas.GetComponent<Canvas>();
                CenterCanvas(canvas);
                canvas.transform.localScale = new Vector3(4f / 960, 2.5f / 600, 1);
            }

            //Link XR Interaction Manager
            foreach (Transform t in XROrigin.transform)
            {
                if (t.gameObject.name == "LeftHand Controller" || t.gameObject.name == "RightHand Controller")
                {
                    var xrRayInteractor = t.GetComponent<XRRayInteractor>();
                    xrRayInteractor.interactionManager = XRInteractionManager.GetComponent<XRInteractionManager>();
                    xrRayInteractor.lineType = XRRayInteractor.LineType.ProjectileCurve;
                }
            }
            BoxCollider collider = Canvas.AddComponent<BoxCollider>();
            collider.size = new Vector3(960, 600, 0.05f);
            Rigidbody rigidbody = Canvas.AddComponent<Rigidbody>();
            rigidbody.isKinematic = true;
            var canvasInteractable = Canvas.AddComponent<XRGrabInteractable>();
            canvasInteractable.interactionManager = XRInteractionManager.GetComponent<XRInteractionManager>();
            canvasInteractable.throwOnDetach = false;

            // Tracked UI Raycasts
            Canvas.AddComponent<TrackedDeviceGraphicRaycaster>(); // XR UI Input Module already as default input system

            yield return null;//TODO test if in one frame
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startSceneVR">Is this the scene where VR was first started from?</param>
        /// <returns></returns>
        private IEnumerator ReconfigureVisualization(bool startSceneVR = false)
        {
            if (!startSceneVR)
                Destroy(GameObject.Find("Main Camera"));

            var maincamera = XROrigin.transform.GetChild(0).GetChild(0).gameObject;
            maincamera.tag = "MainCamera";
            maincamera.layer = 1; //TransparentFX
            maincamera.GetComponent<CharacterController>().enabled = true;
            PlayerController pc = maincamera.GetComponent<PlayerController>();
            pc.enabled = true;
            pc.IF = GameObject.Find("SearchIF");
            pc.menuCanvas = GameObject.Find("MainMenuPanel");

            { //Configure Canvas (TODO refactor?)
                GameObject Canvas = GameObject.Find("PythonBindCanvas");
                Canvas canvas = Canvas.GetComponent<Canvas>();
                for (int i = 0; i < Canvas.transform.childCount; i++)
                {
                    var dataview = Canvas.transform.GetChild(i);
                    if (dataview.name == "Data View")
                        _dataviewTransform = dataview.GetComponent<RectTransform>();
                }
                float w = canvas.renderingDisplaySize.x, h = canvas.renderingDisplaySize.y;
                CenterCanvas(canvas);
                Canvas.transform.localScale = new Vector3(4.44f / w, 2.5f / h, 1);
                canvas.renderMode = RenderMode.WorldSpace;
                BoxCollider collider = Canvas.AddComponent<BoxCollider>();
                collider.size = new Vector3(w, h, 0.05f);
                Rigidbody rigidbody = Canvas.AddComponent<Rigidbody>();
                rigidbody.isKinematic = true;
                var canvasInteractable = Canvas.AddComponent<XRGrabInteractable>();
                canvasInteractable.interactionManager = XRInteractionManager.GetComponent<XRInteractionManager>();
                canvasInteractable.throwOnDetach = false;

                // Tracked UI Raycasts
                Canvas.AddComponent<TrackedDeviceGraphicRaycaster>(); // XR UI Input Module already as default input system

                yield return new WaitUntil(() => CanvasRecentered(canvas));
                //  TODO TODO TODO fix accumulating offset (e.g. move the canvas 5-10 times) TODO TODO TODO
                // Translate, rotate spots along with canvas
                (Vector3 pos, Vector3 rot) t0 = (canvas.transform.position, canvas.transform.eulerAngles);
                Vector3 o_t0 = DataView_Origin;
                Vector3 o_c = canvas.transform.position;
                canvasInteractable.selectExited.AddListener((SelectExitEventArgs args) =>
                {
                    (Vector3 pos, Vector3 rot) t1 = (canvas.transform.position, canvas.transform.eulerAngles);
                    Vector3 o_t1 = DataView_Origin;
                    Vector3 odelta = new Vector3(o_t1.x - o_t0.x, o_t1.y - o_t0.y, o_t1.z - o_t0.z); //o_t1 - o_t0
                    Vector3 rotdelta = new Vector3(t1.rot.x - t0.rot.x, t1.rot.y - t0.rot.y, t1.rot.z - t0.rot.z); //t1.rot - t0.rot
                    void DoTRS(SpotDrawer.SpotWrapper[] spots)
                    {
                        var translateM = Matrix4x4.Translate(odelta);
                        var rotateM = Matrix4x4.TRS(t1.pos, Quaternion.Euler(rotdelta), Vector3.one)
                            * Matrix4x4.Translate(-t1.pos);
                        foreach (SpotDrawer.SpotWrapper spot in spots)
                        {
                            // align with canvas
                            spot.Location = rotateM.MultiplyPoint(spot.Location);
                            spot.Location = translateM.MultiplyPoint(spot.Location);                            
                        }
                        SpotDrawer.OnTransform -= DoTRS;
                    }
                    SpotDrawer.OnTransform += DoTRS;
                    // t1 -> t0
                    t0 = t1;
                    o_t0 = o_t1;
                });
            }

            yield return null; // TODO
        }

        private IEnumerator TransformSpots2VRCanvas()
        {
            Canvas canvas = GameObject.Find("PythonBindCanvas").GetComponent<Canvas>();
            // Wait some frames, until VR-canvas is up to date
            yield return new WaitUntil(() => CanvasRecentered(canvas));

            // Translate spots onto canvas
            void DoTRS(SpotDrawer.SpotWrapper[] spots)
            {
                var o = DataView_Origin;
                var canvasM = Matrix4x4.TRS(o, canvas.transform.rotation, canvas.transform.localScale);
                foreach (SpotDrawer.SpotWrapper spot in spots)
                    spot.Location = canvasM.MultiplyPoint(spot.Location); // align with canvas at o

                SpotDrawer.OnTransform -= DoTRS;
            }
            SpotDrawer.OnTransform += DoTRS;
        }

        public static void CenterCanvas(Canvas canvas, float posZ = 2)
        {
            RecenterStatus[canvas] = false;
            canvas.renderMode = RenderMode.WorldSpace;
            {// grab aggregate height
                float h = 0;
                int fw = 30, fc = 30; // use a frame window of 30
                var maincamera = Camera.main; //cache camera reference
                void SetCanvasPose()
                {
                    if (Instance.HMD.TryGetFeatureValue(CommonUsages.userPresence, out var userPresence))
                        if (!userPresence)
                            return;

                    if (maincamera.transform.localPosition.y > 0 && fc-- > 0)
                        h += maincamera.transform.localPosition.y / fw;

                    if (fc == 0)
                    {
                        canvas.transform.localPosition = new Vector3(0, h, posZ);
                        // center canvas in front of the VR user
                        canvas.transform.localEulerAngles = new Vector3(
                            0,
                            Mathf.Acos(Vector3.Dot(Vector3.forward, maincamera.transform.forward)) * Mathf.Rad2Deg,
                            0);
                        Instance.OnUpdate -= SetCanvasPose;
                        RecenterStatus[canvas] = true;
                    }
                }
                Instance.OnUpdate += SetCanvasPose;
            }
        }

        public static bool CanvasRecentered(Canvas canvas)
        {
            if (!RecenterStatus.ContainsKey(canvas))
                return false;

            return RecenterStatus[canvas];
        }

        private GameObject InstantiatePrefab(string name, Transform parent = null, bool dontDestroy = true)
        {
            var gobj = Instantiate(Resources.Load(name), parent) as GameObject;
            gobj.SetActive(true);
            gobj.name = name;
            if (dontDestroy)
                DontDestroyOnLoad(gobj);
            return gobj;
        }

        public InputDevice HMD { get; private set; }
        public GameObject XROrigin { get; private set; }
        public GameObject XRInteractionManager { get; private set; }
        public GameObject InputActionManager { get; private set; }
        public delegate void ToExecute();
        internal ToExecute OnUpdate;
        /// <summary>
        /// Is VR active?
        /// </summary>
        public bool VR { get; private set; }
        private static Dictionary<Canvas, bool> RecenterStatus = new Dictionary<Canvas, bool>();

        #region Visualization Canvas
        private RectTransform _dataviewTransform;
        /// <summary>
        /// World position of the lower, bottom corner of the data view.
        /// </summary>
        public Vector3 DataView_Origin => _dataviewTransform.GetChild(0).transform.position;
        private SpotDrawer SpotDrawer;
        #endregion
    }
}
