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
    private bool qKey;
    private bool eKey;
    public Vector3 centerpoint;
    Vector3 origin;
    SpotDrawer sd;
    private void Start()
    {
        sd = GameObject.Find("ScriptHolder").GetComponent<SpotDrawer>();
    }
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

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Q)) qKey = true;
        if (Input.GetKeyUp(KeyCode.Q)) qKey = false;        
        if (Input.GetKeyDown(KeyCode.E)) eKey = true;
        if (Input.GetKeyUp(KeyCode.E)) eKey = false;
        if (Input.GetKeyUp(KeyCode.E)) eKey = false;

        if (qKey) sd.rotateSlice(1, datasetName, centerpoint, this.gameObject);
        if (eKey) sd.rotateSlice(-1, datasetName, centerpoint, this.gameObject);

    
        //if (Input.GetMouseButtonDown(0))
        //{
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //    RaycastHit hit = new RaycastHit();

        //    if (Physics.Raycast(ray, out hit))
        //    {
        //        //TBD not using name cube here
        //        if (hit.collider.gameObject.name == "Cube")
        //        {
        //            Debug.Log("pressed");

        //        }
        //    }
        //}

        //if (Input.GetMouseButtonDown(1))
        //{
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //    RaycastHit hit = new RaycastHit();

        //    if (Physics.Raycast(ray, out hit))
        //    {
        //        //TBD not using name cube here
        //        if (hit.collider.gameObject.name == "Cube")
        //        {
        //            Debug.Log("right");

        //        }
        //    }
        //}
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
        sd.moveSlice(xoffset, yoffset, zoffset, datasetName);
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

    public float getXoffset()
    {
        return xoffset;
    }
    public float getYoffset()
    {
        return yoffset;
    }

    public void setCenterPoint(Vector3 cp)
    {
        centerpoint = cp;
    }
}
