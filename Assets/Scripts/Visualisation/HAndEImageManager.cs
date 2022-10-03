using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HAndEImageManager : MonoBehaviour
{
    public string imagePath;
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
