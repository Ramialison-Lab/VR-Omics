using UnityEngine;
using UnityEngine.EventSystems;

public class UIElementDragger : EventTrigger
{
    private bool dragging;
    new private RectTransform transform;
    private Rect canvasRect;
    private GameObject slicePanel;
    private Canvas cvs;

    private void Start()
    {


        transform = GetComponent<RectTransform>();
        canvasRect = GetComponentInParent<Canvas>().pixelRect;
        //  cvs = GameObject.Find("SlicePanelCanvas").GetComponent<Canvas>();
        slicePanel = GameObject.Find("SlicePanel");
    }


    public void Update()
    {
        if (dragging)
        {
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }

        // to clamp element between boundaries

        //Vector2 clamped = transform.anchoredPosition;
        //float imageWidth = transform.rect.x;

        //clamped.x = Mathf.Clamp(clamped.x, 0+imageWidth, 1063-imageWidth);
        //clamped.y = Mathf.Clamp(clamped.y, -606+imageWidth,0-imageWidth);

        //transform.anchoredPosition = clamped;      
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        dragging = true;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;
    }
}
