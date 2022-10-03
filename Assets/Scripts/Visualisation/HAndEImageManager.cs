using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HAndEImageManager : MonoBehaviour
{
    public string imagePath;
    public void createDragObjects()
    {
        // TBD resize feature
        GameObject widthDrag = GameObject.CreatePrimitive(PrimitiveType.Plane);
        widthDrag.name = "widthDrag";
        widthDrag.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        widthDrag.transform.SetParent(this.transform);
        widthDrag.transform.Rotate(new Vector3(-90, 0, 0));

        widthDrag.transform.position = new Vector3(transform.root.position.x * 2,0 ,0);
        widthDrag.transform.localPosition = new Vector3(widthDrag.transform.localPosition.x, 0, 0) ;
        widthDrag.transform.localScale = new Vector3(0.05f, widthDrag.transform.localScale.y, widthDrag.transform.localScale.z);

        GameObject heighthDrag = GameObject.CreatePrimitive(PrimitiveType.Plane);
        heighthDrag.name = "heightDrag";
        heighthDrag.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        heighthDrag.transform.SetParent(this.transform);
        heighthDrag.transform.Rotate(new Vector3(-90, 0, 0));

        heighthDrag.transform.position = new Vector3(0, 0, transform.root.position.x * 2);
        heighthDrag.transform.localPosition = new Vector3(0, 0, heighthDrag.transform.localPosition.x);
        heighthDrag.transform.localScale = new Vector3(0.1f, heighthDrag.transform.localScale.y, 0.05f );
        heighthDrag.AddComponent<ResizeManager>();
        widthDrag.AddComponent<ResizeManager>();
        heighthDrag.GetComponent<ResizeManager>().setParent(this.gameObject);
        widthDrag.GetComponent<ResizeManager>().setParent(this.gameObject);

        //GameObject diagDrag = GameObject.CreatePrimitive(PrimitiveType.Plane);
        //diagDrag.name = "diagDrag";

        //diagDrag.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        //diagDrag.transform.SetParent(this.transform);
        //diagDrag.transform.Rotate(new Vector3(-90, 0, 0));

        //diagDrag.transform.position = new Vector3(transform.root.position.x * 2, 0, transform.root.position.x * 2);
        //diagDrag.transform.localPosition = new Vector3(diagDrag.transform.localPosition.x, 0, heighthDrag.transform.localPosition.x);
        //diagDrag.transform.localScale = new Vector3(0.05f, diagDrag.transform.localScale.y, diagDrag.transform.localScale.z);
    }

    private Vector3 mOffset;
    private float mZCoord;
    public bool dragMode = false;
    public bool resizeMode = false;
    

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

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            clicked();
        }

            if (Input.GetKey(KeyCode.K))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    transform.localScale += new Vector3(0, 0, 0.025f);
                }
                else
                {
                    transform.Translate(Vector3.forward * 10 * Time.deltaTime);
                }
            }
            if (Input.GetKey(KeyCode.I))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    transform.localScale -= new Vector3(0, 0, 0.025f);
                }
                else
                {
                    transform.Translate(Vector3.up * 10 * Time.deltaTime, Space.World);
                }
            }
            if (Input.GetKey(KeyCode.J))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    transform.localScale -= new Vector3(0.025f, 0, 0);
                }
                else
                {
                    transform.Translate(Vector3.left * 10 * Time.deltaTime, Camera.main.transform);
                }
            }
            if (Input.GetKey(KeyCode.L))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    transform.localScale += new Vector3(0.025f, 0, 0);
                }
                else
                {
                    transform.Translate(Vector3.right * 10 * Time.deltaTime, Camera.main.transform);
                }
            }    
    }

    public void clicked()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit = new RaycastHit();



        if (Physics.Raycast(ray, out hit))
        {
         //   Debug.Log(hit.point);
        }
    }

    public void setAlpha(float alpha, Material transpMat)
    {
        Color color = this.gameObject.GetComponent<Renderer>().material.color;
        color.a = alpha;
        transpMat.color = color;
        this.gameObject.GetComponent<Renderer>().material = transpMat;
        byte[] byteArray = File.ReadAllBytes(imagePath);
        Texture2D sampleTexture = new Texture2D(2, 2);
        bool isLoaded = sampleTexture.LoadImage(byteArray);

        this.gameObject.GetComponent<Renderer>().material.mainTexture = sampleTexture;

    }

    public void setImagePath(string imagePath)
    {
        this.imagePath = imagePath;
    }

}
