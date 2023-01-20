/*
* Copyright (c) 2023 Murdoch Children's Research Institute, Parkville, Melbourne
* author: Denis Bienroth
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
* 
*/using UnityEngine;
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
