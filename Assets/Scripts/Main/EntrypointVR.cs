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
        DefaultCamera = InstantiatePrefab("Main Camera");
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

    private IEnumerator Reconfigure()
    {
        // take down default camera
        Destroy(DefaultCamera);
        XROrigin = InstantiatePrefab("XR Origin");
        XRInteractionManager = InstantiatePrefab("XR Interaction Manager");

        //Configure canvas
        Canvas canvas = Canvas.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.transform.localPosition = new Vector3(0, 2, 2);
        canvas.transform.localScale = new Vector3(4f / 960, 2.5f / 600, 1);

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
    private static GameObject DefaultCamera;
    private static GameObject XROrigin;
    private static GameObject XRInteractionManager;
    private static GameObject Canvas;
    private static GameObject InputActionManager;
}
