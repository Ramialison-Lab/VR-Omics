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
            StartCoroutine(DetectHMD());
        }

        private IEnumerator DetectHMD()
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
                    StartCoroutine(Reconfigure());
                    SceneManager.sceneLoaded += (scene, mode) =>
                    {
                        if (scene.name != "Visualisation")
                            return;
                        StartCoroutine(ReconfigureVisualization());
                    };
                    VR = true;
                    yield break;
                }

                yield return new WaitForEndOfFrame();
            }
        }

        private void Update()
        {
            OnUpdate?.Invoke();
        }

        private IEnumerator Reconfigure()
        {
            // take down default camera
            Destroy(GameObject.Find("Main Camera"));
            //XR IM needs to be add first to avoid XR IT automatically adding one
            XRInteractionManager = InstantiatePrefab("XR Interaction Manager");
            XROrigin = InstantiatePrefab("XR Origin");
            GameObject Canvas = GameObject.Find("Canvas");

            {//Configure canvas
                Canvas canvas = Canvas.GetComponent<Canvas>();
                CanvasUtilities.CenterCanvas(canvas);
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

            // Input Action Manager
            InputActionManager = InstantiatePrefab("Input Action Manager");

            yield return null;//TODO test if in one frame
        }

        private IEnumerator ReconfigureVisualization()
        {
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
                Canvas.transform.localScale = new Vector3(4f / 990, 2.5f / 619, 1);
                Canvas canvas = Canvas.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.WorldSpace;
                BoxCollider collider = Canvas.AddComponent<BoxCollider>();
                collider.size = new Vector3(960, 600, 0.05f);
                Rigidbody rigidbody = Canvas.AddComponent<Rigidbody>();
                rigidbody.isKinematic = true;
                var canvasInteractable = Canvas.AddComponent<XRGrabInteractable>();
                canvasInteractable.interactionManager = XRInteractionManager.GetComponent<XRInteractionManager>();
                canvasInteractable.throwOnDetach = false;

                // Tracked UI Raycasts
                Canvas.AddComponent<TrackedDeviceGraphicRaycaster>(); // XR UI Input Module already as default input system
            }

            yield return null; // TODO
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
        internal delegate void ToExecute();
        internal ToExecute OnUpdate;
        /// <summary>
        /// Is VR active?
        /// </summary>
        public bool VR { get; private set; }
    }
}
