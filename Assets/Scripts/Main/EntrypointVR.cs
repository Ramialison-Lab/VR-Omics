using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// This represents the entrypoint from the initial Desktop configuration to a VR, interactable reconfiguration.
/// The reconfiguration is executed automatically, given conditions are met.
/// </summary>
public class EntrypointVR : MonoBehaviour
{
    void Awake()
    {
        gameObject.hideFlags = hideFlags |= HideFlags.HideAndDontSave | HideFlags.HideInHierarchy;
        Canvas = GameObject.Find("Canvas");
        if (!Canvas)
            throw new System.Exception("Could not find Canvas!");
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

        {//Configure canvas
            Canvas canvas = Canvas.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            {// grab aggregate height
                float h = 0;
                int fw = 60, fc = 60; // use a frame window of 60
                var maincamera = Camera.main; //cache camera reference
                void SetCanvasPose()
                {
                    if (maincamera.transform.localPosition.y > 0 && fc-- > 0)
                        h += maincamera.transform.localPosition.y / fw;

                    if (fc == 0)
                    {
                        canvas.transform.localPosition = new Vector3(0, h, 2);
                        // center canvas in front of the VR user
                        canvas.transform.localEulerAngles = new Vector3(
                            0,
                            Mathf.Acos(Vector3.Dot(Vector3.forward, maincamera.transform.forward)) * Mathf.Rad2Deg,
                            0);
                        OnUpdate -= SetCanvasPose;
                    }
                }
                OnUpdate += SetCanvasPose;
            }
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
        var xrGrabInteractable = Canvas.AddComponent<XRGrabInteractable>(); //already has BoxCollider, Rigidbody on
        xrGrabInteractable.interactionManager = XRInteractionManager.GetComponent<XRInteractionManager>();

        // Input Action Manager
        InputActionManager = InstantiatePrefab("Input Action Manager");

        yield return null;//TODO test if in one frame
    }

    private GameObject InstantiatePrefab(string name, Transform parent = null)
    {
        var gobj = Instantiate(Resources.Load(name), parent) as GameObject;
        gobj.SetActive(true);
        gobj.name = name;
        return gobj;
    }

    private static InputDevice HMD;
    private static GameObject XROrigin;
    private static GameObject XRInteractionManager;
    private static GameObject Canvas;
    private static GameObject InputActionManager;
    private delegate void ToExecute();
    private ToExecute OnUpdate;
}
