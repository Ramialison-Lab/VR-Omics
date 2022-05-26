using UnityEngine;

public class DragObject : MonoBehaviour
{
    private Vector3 mOffset;
    private float mZCoord;
    public float xoffset;
    public float yoffset;
    public float zoffset;
    public string datasetName;
    public Vector3 centerpoint;
    Vector3 origin;
    SpotDrawer sd;
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

    public void setCenterPoint(Vector3 cp)
    {
        centerpoint = cp;
    }

}
