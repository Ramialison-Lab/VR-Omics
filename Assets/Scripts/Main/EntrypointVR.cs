/*
* Copyright (c) 2018 Liefe Science Informatics (university of Konstanz, Germany)
* author: Dimitar Garkov
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"),
* to deal in the Software without restriction, including without limitation
* the rights to use, copy, modify, merge, publish, distribute, sublicense,
* and/or sell copies of the Software, and to permit persons to whom the Software
* is furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in all
* copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
* INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
* PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
* HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
* CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE
* OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

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
            IsDetectingHMD = true;
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
                    IsDetectingHMD = false;
                    string currentScene = SceneManager.GetActiveScene().name;
                    switch (currentScene)
                    {
                        case "Pipeline":
                            StartCoroutine(InitializeVR(ReconfigurePipeline()));
                            SceneManager.sceneLoaded += (scene, mode) =>
                            {
                                if (scene.name != "Visualisation")
                                    return;
                                StartCoroutine(ReconfigureVisualization());
                            };
                            break;
                        case "Visualisation":
                            SpotDrawer = GameObject.Find("ScriptHolder").GetComponent<SpotDrawer>();
                            StartCoroutine(InitializeVR(ReconfigureVisualization(true)));                            
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

        private IEnumerator ReconfigurePipeline()
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
                float w = canvas.renderingDisplaySize.x, h = canvas.renderingDisplaySize.y;
                CenterCanvas(canvas);
                Vector2 min = SpotDrawer.Min;
                Vector2 max = SpotDrawer.Max;
                float data_aspect = (max.x - min.x) / (max.y - min.y);
                float h_aspect = 4f / data_aspect;
                Canvas.transform.localScale = new Vector3(6f / w, h_aspect / h, 1);
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
                SpotDrawer.SetVRDimensions();

                yield return new WaitUntil(() => CanvasRecentered(canvas));
                // Translate, rotate spots along with canvas                
                canvasInteractable.selectExited.AddListener((SelectExitEventArgs args) =>
                {
                    void Recompute(SpotDrawer.SpotWrapper[] spots, SpotDrawer.SpotWrapper[] spotsCopy)
                    {
                        // no additional transformations here
                    }
                    SpotDrawer.OnTransform += Recompute;
                });
            }

            yield return null; // TODO
        }

        private IEnumerator TransformSpots2VRCanvas()
        {
            GameObject Canvas = GameObject.Find("PythonBindCanvas");
            Canvas canvas = Canvas.GetComponent<Canvas>();
            // Wait some frames, until VR-canvas is up to date
            yield return new WaitUntil(() => CanvasRecentered(canvas));

            void Recompute(SpotDrawer.SpotWrapper[] spots, SpotDrawer.SpotWrapper[] spotsCopy)
            {
                // no additional transformations here
            }
            SpotDrawer.OnTransform += Recompute;
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
        public bool IsDetectingHMD { get; set; }
        private static Dictionary<Canvas, bool> RecenterStatus = new Dictionary<Canvas, bool>();

        #region Visualization Canvas
        private SpotDrawer SpotDrawer;
        #endregion
    }
}
