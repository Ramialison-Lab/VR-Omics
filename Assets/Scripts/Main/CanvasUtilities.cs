using UnityEngine;
using UnityEngine.XR;

namespace VROmics.Main
{

    /// <summary>
    /// Author: Dimitar Garkov
    /// </summary>
    public static class CanvasUtilities
    {
        public static void CenterCanvas(Canvas canvas, float posZ = 2)
        {
            canvas.renderMode = RenderMode.WorldSpace;
            {// grab aggregate height
                float h = 0;
                int fw = 30, fc = 30; // use a frame window of 30
                var maincamera = Camera.main; //cache camera reference
                void SetCanvasPose()
                {
                    if (EntrypointVR.Instance.HMD.TryGetFeatureValue(CommonUsages.userPresence, out var userPresence))
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
                        EntrypointVR.Instance.OnUpdate -= SetCanvasPose;
                    }
                }
                EntrypointVR.Instance.OnUpdate += SetCanvasPose;
            }
        }
    }
}