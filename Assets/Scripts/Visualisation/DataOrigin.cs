using UnityEngine;

namespace VROmics.Visualisation
{
    /// <summary>
    /// Constantly provide origin points, while the canvas is being scaled, translated, and rotated.
    /// 
    /// Author: Dimitar Garkov
    /// </summary>
    [RequireComponent(typeof(Canvas)), ExecuteAlways]
    public class DataOrigin : MonoBehaviour
    {
        /// <summary>
        /// The data origin point on the canvas in world coordinates.
        /// </summary>
        public Vector3 Origin { get; private set; }

        /// <summary>
        /// The data origin copy point on the canvas in world coordinates.
        /// </summary>
        public Vector3 OriginCopy { get; private set; }

        /// <summary>
        /// Percentage to fill the data-view from the remaining canvas area. Starting 
        /// from the bottom right corner of the main menu, this gives us a fill percentage,
        /// corresponding to padding for the data (rough estimate).
        /// Primary influence on Editor and VR.
        /// </summary>
        public float Padding { get; private set; }

        public RectTransform LeftOffset;
        public RectTransform RightOffset;
        public RectTransform TopOffset;

        void Update()
        {
            // also up-to-date in the editor
            canvas = GetComponent<Canvas>();
            rectTransform = GetComponent<RectTransform>();

            switch (canvas.renderMode)
            {
                case RenderMode.ScreenSpaceOverlay:
                    Origin = new Vector3(
                        rectTransform.position.x - rectTransform.sizeDelta.x / 2f + LeftOffset.sizeDelta.x,
                        rectTransform.position.y - rectTransform.sizeDelta.y / 2f,
                        rectTransform.position.z);
                    OriginCopy = new Vector3(
                        Origin.x + 0.5f * 
                            (rectTransform.sizeDelta.x - LeftOffset.sizeDelta.x - RightOffset.sizeDelta.x),
                        Origin.y, Origin.z);
                    break;
                case RenderMode.WorldSpace:
                    Origin = new Vector3(
                        rectTransform.position.x + rectTransform.localScale.x * 
                            (-rectTransform.sizeDelta.x / 2f + LeftOffset.sizeDelta.x),
                        rectTransform.position.y - rectTransform.sizeDelta.y / 2f * rectTransform.localScale.y,
                        rectTransform.position.z);
                    OriginCopy = new Vector3(
                        Origin.x + 0.5f * rectTransform.localScale.x *
                            (rectTransform.sizeDelta.x - LeftOffset.sizeDelta.x - RightOffset.sizeDelta.x),
                        Origin.y, Origin.z);
                    // rotate
                    var v = Origin - rectTransform.position; // v from canvas pivot to Origin
                    v = rectTransform.rotation * v;
                    Origin = rectTransform.position + v;

                    v = OriginCopy - rectTransform.position;
                    v = rectTransform.rotation * v;
                    OriginCopy = rectTransform.position + v;
                    break;
            }

            if (RightOffset.sizeDelta.x >= TopOffset.sizeDelta.y) // scales uniformly, take only one
                Padding = 1 - RightOffset.sizeDelta.x / (rectTransform.sizeDelta.x - LeftOffset.sizeDelta.x);
            else
                Padding = 1 - TopOffset.sizeDelta.y / rectTransform.sizeDelta.y;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            float radius = 1;
            switch (canvas.renderMode)
            {
                case RenderMode.ScreenSpaceOverlay:
                    radius = 10;
                    break;
                case RenderMode.WorldSpace:
                    radius = 0.1f;
                    break;
            }
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(Origin, radius);
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(OriginCopy, radius);
        }
#endif

        private Canvas canvas;
        private RectTransform rectTransform;
    }
}