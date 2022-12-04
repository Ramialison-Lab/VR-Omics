using UnityEngine;
using VROmics.Main;

namespace VROmics.Pipeline
{
    /// <summary>
    /// Author: Dimitar Garkov
    /// </summary>
    public class SimpleFileBrowserVR : MonoBehaviour
    {
        void OnEnable()
        {
            if (!EntrypointVR.Instance.VR)
                return;

            Canvas canvas = GetComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            // only fix pose for VR
            EntrypointVR.CenterCanvas(canvas, 1.75f);
            canvas.transform.localScale = new Vector3(2f / 960, 1.25f / 600, 1);
        }
    }
}