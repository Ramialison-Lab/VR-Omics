using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeManager : MonoBehaviour
{
    public GameObject parentObj;
    public Vector2 mousePos;
    private void OnMouseDrag()
    {
        if (transform.gameObject.name == "widthDrag")
        {
            float scaleSize = Input.mousePosition.x - mousePos.x;
            if(scaleSize>0) parentObj.transform.localScale = new Vector3(parentObj.transform.localScale.x+0.01f, parentObj.transform.localScale.y, parentObj.transform.localScale.z);
            if(scaleSize<0) parentObj.transform.localScale = new Vector3(parentObj.transform.localScale.x-0.01f, parentObj.transform.localScale.y, parentObj.transform.localScale.z);
        }       
        
        if (transform.gameObject.name == "heightDrag")
        {
            float scaleSize = Input.mousePosition.z - mousePos.y;
            if(scaleSize>0) parentObj.transform.localScale = new Vector3(parentObj.transform.localScale.x, parentObj.transform.localScale.y, parentObj.transform.localScale.z + 0.01f);
            if(scaleSize<0) parentObj.transform.localScale = new Vector3(parentObj.transform.localScale.x, parentObj.transform.localScale.y, parentObj.transform.localScale.z - 0.01f);
        }
    }

    public void setParent(GameObject go)
    {
        parentObj = go;
    }

    private void LateUpdate()
    {
        mousePos = Input.mousePosition;
    }
}
