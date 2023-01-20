/*
* Copyright (c) 2023 Life Science Informatics (university of Konstanz, Germany)
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
*/
using UnityEngine;

public class DragObject : MonoBehaviour
{
    private Vector3 mOffset;
    private float mZCoord;
    public float xoffset;
    public float yoffset;
    public float zoffset;
    private int depth;
    public string datasetName;
    public Vector3 centerpoint;
    Vector3 origin;
    SpotDrawer sd;
    public int colMin;
    public int colMax;
    public int rowMin;
    public int rowMax;

    private void Start()
    {
        sd = GameObject.Find("ScriptHolder").GetComponent<SpotDrawer>();

    }
    void OnMouseDown()
    {
        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
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

    public void rotate(int direction)
    {
        if (direction == 1) sd.rotateSlice(1, datasetName, centerpoint, this.gameObject);
        if (direction == 0) sd.rotateSlice(-1, datasetName, centerpoint, this.gameObject);
    }

    void OnMouseDrag()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            transform.position = GetMouseAsWorldPoint() + mOffset;
            xoffset = origin.x - transform.position.x;
            yoffset = origin.y - transform.position.y;
            zoffset = origin.z - transform.position.z;
            sd.moveSlice(xoffset, yoffset, zoffset, datasetName, transform.position.z);
        }
    }

    public void resetCoords()
    {
        xoffset = 0;
        yoffset = 0;
        zoffset = 0;

        origin = transform.position;
    }

    public void setMetaData(int colMin, int colMax, int rowMin, int rowMax, int depth, string datasetName)
    {
        this.colMin = colMin;
        this.colMax = colMax;
        this.rowMin = rowMin;
        this.rowMax = rowMax;
        this.depth = depth;
        this.datasetName = datasetName;

    }

    public string getDatasetName()
    {
        return datasetName;
    }

    public void setCenterPoint(Vector3 cp)
    {
        centerpoint = cp;
    }
}
