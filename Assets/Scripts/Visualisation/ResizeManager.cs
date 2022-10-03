using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeManager : MonoBehaviour
{
    public GameObject parentObj;
    public Vector2 mousePos;
    public float scaleFactor = 0.005f;



    public void setParent(GameObject go)
    {
        parentObj = go;
    }

    private void LateUpdate()
    {
        mousePos = Input.mousePosition;
    }
}
