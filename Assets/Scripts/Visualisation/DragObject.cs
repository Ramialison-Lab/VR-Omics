using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragObject : MonoBehaviour
{
    private Vector3 mOffset;
    private float mZCoord;
    public float xoffset;
    public float yoffset;
    public float zoffset;
    public string datasetName;
    Vector3 origin;

    void OnMouseDown()
    {
        mZCoord = Camera.main.WorldToScreenPoint(
        gameObject.transform.position).z;

        // Store offset = gameobject world pos - mouse world pos
        mOffset = gameObject.transform.position - GetMouseAsWorldPoint();
    }

    private Vector3 GetMouseAsWorldPoint()
    {
        // Pixel coordinates of mouse (x,y)
        Vector3 mousePoint = Input.mousePosition;

        // z coordinate of game object on screen
        mousePoint.z = mZCoord;

        // Convert it to world points
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    void OnMouseDrag()
    {
        Color newColor = GetComponent<Renderer>().material.color;
        newColor.a = 0.2f;
        GetComponent<Renderer>().material.color = newColor;
        transform.position = GetMouseAsWorldPoint() + mOffset;
        xoffset = origin.x - transform.position.x;
        yoffset = origin.y - transform.position.y;
        zoffset = origin.z - transform.position.z;

        //TBD pass datasetname, x,y and z offset 

        GameObject.Find("ScriptHolder").GetComponent<SpotDrawer>().moveSlice(xoffset, yoffset, zoffset, datasetName);

    }

    public void resetCoords(string datasetName)
    {
        this.datasetName = datasetName;
        xoffset = 0;
        yoffset = 0;
        zoffset = 0;

        origin = transform.position;
    }

    public string getDatasetName()
    {
        return datasetName;
    }
}
