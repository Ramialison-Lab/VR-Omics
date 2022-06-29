using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeManager : MonoBehaviour
{
    public GameObject parentObj;
    public Vector2 mousePos;
    public float scaleFactor = 0.005f;

    private void OnMouseDrag()
    {
        if (GameObject.Find("MainMenuPanel").GetComponent<MenuCanvas>().resizeActive())
        {
            if (transform.gameObject.name == "widthDrag")
            {
                float scaleSize = Input.mousePosition.x - mousePos.x;
                if (scaleSize > 0) parentObj.transform.localScale = new Vector3(parentObj.transform.localScale.x + scaleFactor, parentObj.transform.localScale.y, parentObj.transform.localScale.z);
                if (scaleSize < 0) parentObj.transform.localScale = new Vector3(parentObj.transform.localScale.x - scaleFactor, parentObj.transform.localScale.y, parentObj.transform.localScale.z);
            }

            if (transform.gameObject.name == "heightDrag")
            {
                float scaleSize = Input.mousePosition.y - mousePos.y;
                if (scaleSize < 0) parentObj.transform.localScale = new Vector3(parentObj.transform.localScale.x, parentObj.transform.localScale.y, parentObj.transform.localScale.z + scaleFactor);
                if (scaleSize > 0) parentObj.transform.localScale = new Vector3(parentObj.transform.localScale.x, parentObj.transform.localScale.y, parentObj.transform.localScale.z - scaleFactor);
            }
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
