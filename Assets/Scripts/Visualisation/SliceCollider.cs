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
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

public class SliceCollider : MonoBehaviour
{
    int l_slice;
    int r_slice;
    int top_slice;
    int btm_slice;
    int depth;
    public List<GameObject> sliceColliders;
    public List<int> zcoords;
    public TMP_Dropdown dd;
    public List<GameObject> HandEobjs;
    public Material transparentMat;
    public GameObject object3d;
    public List<GameObject> BtnPanels = new List<GameObject>(3);
    public GameObject btngroup;
    private DataTransferManager dfm;
    int countSFs = 0;

    public bool objectUsed = false;
    public bool objectResize = false;
    public bool objectMove = false;
    public bool objectRotate = false;

    private Color transparentColor = new Color(1, 1, 1, 0);
    private void Start()
    {
        dfm = GameObject.Find("ScriptHolder").GetComponent<DataTransferManager>();
    }

    public void adjustSliceCollider(GameObject sliceCollider, Vector2 minVec, Vector2 maxVec, Vector2 centerPoint, int depth, string datasetName)
    {
        //TBD setslicecollider funciton here:
        sliceCollider.GetComponent<MeshRenderer>().enabled = false;

        sliceColliders.Add(sliceCollider);
        zcoords.Add(depth);
        Color newColor = sliceCollider.GetComponent<Renderer>().material.color;
        newColor.a = 0f;
        sliceCollider.GetComponent<Renderer>().material.color = newColor;
        sliceCollider.AddComponent<DragObject>();
        sliceCollider.GetComponent<DragObject>().resetCoords();
        // sliceCollider.GetComponent<DragObject>().setCenterPoint(cube.transform.position);
        // sliceCollider.GetComponent<DragObject>().setMetaData(colMin, colMax, rowMin, rowMax, depth, datasetName);
        sliceCollider.GetComponent<Renderer>().material.color = transparentColor;

        if (gameObject.GetComponent<DataTransferManager>().addHAndEImg)
        {
            adjustHEStainImage(sliceCollider, datasetName, minVec, maxVec);
        }
    }

    private void adjustHEStainImage(GameObject sliceCollider, string datasetName, Vector2 minVec, Vector2 maxVec)
    {
        GameObject imagePlane = GameObject.CreatePrimitive(PrimitiveType.Cube);
        imagePlane.name = "StainImageObject";

        //imagePlane.transform.localScale = new Vector3(sliceCollider.transform.localScale.x * 1.5f, 0.1f, sliceCollider.transform.localScale.z * 1.361f);
        //imagePlane.transform.SetParent(sliceCollider.transform);

        //imagePlane.transform.localPosition = Vector3.zero;
        imagePlane.transform.position = sliceCollider.transform.position;

        imagePlane.GetComponent<Renderer>().material = transparentMat;

        string imagePath = System.IO.Directory.GetCurrentDirectory() + "/Assets/Images/Error_Images/spatial_file_not_found.png";

        int index = datasetName.LastIndexOf("\\");
        string path = "";
        if (index >= 0)
        {
            path = datasetName.Substring(0, index);
        }
        //can't find the spatial image from directory
        string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
        try
        {
            foreach (string s in files)
            {
                if (s.Contains("tissue_hires_image.png") && !s.Contains("meta")) imagePath = s;
            }
        }
        catch (Exception ex) { dfm.logfile.Log(ex, "The tissue image couldn't be found. Make sure the image is in the directory and called tissue_hires_image.png"); }
        //Debug.Log(imagePath);

        byte[] byteArray = File.ReadAllBytes(imagePath);
        Texture2D sampleTexture = new Texture2D(2, 2);
        bool isLoaded = sampleTexture.LoadImage(byteArray);

        #region Scaling H&E image

        //imagePlane.transform.localPosition = new Vector3(imagePlane.transform.localPosition.x, imagePlane.transform.localPosition.y + 0.054f, imagePlane.transform.localPosition.z);
        // Debug.Log("Raw size: " + sampleTexture.width + " x " + sampleTexture.height);

        float highres_w = sampleTexture.width;
        float highres_h = sampleTexture.height;

        float sF = dfm.scaleFactors[countSFs];

        float fullres_w = highres_w / sF;
        float fullres_h = highres_h / sF;

        //Debug.Log(fullres_w);
        //Debug.Log(fullres_h);


        string[] positionList = dfm.positionList;

        string[] lines = File.ReadAllLines(positionList[countSFs]);

        string[] values = lines[0].Split(',');


        int min_x = int.Parse(values[4]);
        int max_x = int.Parse(values[4]);
        int min_y = int.Parse(values[5]);
        int max_y = int.Parse(values[5]);  

        for (int i=0; i < lines.Length; i++){

            values = lines[i].Split(',');


            if (int.Parse(values[4]) < min_x)
                {
                    min_x = int.Parse(values[4]);
                }
            else if (int.Parse(values[4]) > max_x)
                {
                    max_x = int.Parse(values[4]);
                }

            if (int.Parse(values[5]) < min_y)
            {
                min_y = int.Parse(values[5]);
            }
            else if (int.Parse(values[5]) > max_y)
            {
                max_y = int.Parse(values[5]);
            }
        }

        //Debug.Log(min_x);
        //Debug.Log(max_x);
        //Debug.Log(min_y);
        //Debug.Log(max_y);


        float distance_w_left = min_x;
        float distance_w_right = fullres_w - max_x;
        float distance_h_bottom = min_y;
        float distance_h_top = fullres_h - max_y;


        float percentage_distance_w_left = distance_w_left/ fullres_w;
        float percentage_distance_w_right = distance_w_right/fullres_w;
        float percentage_distance_h_bottom = distance_h_bottom/fullres_h;
        float percentage_distance_h_top = distance_h_top/fullres_h;

        //Debug.Log(percentage_distance_w_left);
        //Debug.Log(percentage_distance_w_right);
        //Debug.Log(percentage_distance_h_bottom);
        //Debug.Log(percentage_distance_h_top);

        imagePlane.transform.localScale = new Vector3(1,1,1);
        float IP_x = imagePlane.transform.localScale.x; 
        float IP_y = imagePlane.transform.localScale.y; 

        //Debug.Log(IP_x);
        //Debug.Log(IP_y);

        float percentage_highres_w = IP_x - (IP_x * percentage_distance_w_left) - (IP_x * percentage_distance_w_right);
        float percentage_highres_h = IP_y - (IP_y * percentage_distance_h_top) - (IP_y * percentage_distance_h_bottom);

        //Debug.Log(percentage_highres_w);
        //Debug.Log(percentage_highres_h);

        float scaleWidth = sliceCollider.transform.localScale.x / (IP_x * percentage_highres_w);
        float scaleHeight = sliceCollider.transform.localScale.y / (IP_y * percentage_highres_h);

        // this value says how much percent of the imageplane are stain image
        // Overlay this percentage with the actual spot dimensions

        //float scaleWidth = sliceCollider.transform.localScale.x / percentage_highres_w;
        //float scaleHeight = sliceCollider.transform.localScale.y / percentage_highres_h;

        //Debug.Log(scaleWidth);
        //Debug.Log(scaleHeight);

        imagePlane.transform.localScale = new Vector3(imagePlane.transform.localScale.x * scaleWidth, imagePlane.transform.localScale.y * scaleHeight, imagePlane.transform.localScale.z);

        //imagePlane.transform.localScale = new Vector3(imagePlane.transform.localScale.x * scaleWidth, imagePlane.transform.localScale.y * scaleHeight, imagePlane.transform.localScale.z);

        countSFs++;
        #endregion

        imagePlane.GetComponent<Renderer>().material.mainTexture = sampleTexture;
        imagePlane.AddComponent<HAndEImageManager>();
        //TODO: correct path to Visium spatial image
        imagePlane.GetComponent<HAndEImageManager>().setImagePath(imagePath);
        imagePlane.AddComponent<BoxCollider>();
        HandEobjs.Add(imagePlane);
        imagePlane.SetActive(false);
        imagePlane.transform.Rotate(new Vector3(0, 0, 180));

    }


    public void setSliceCollider(int colMin, int colMax, int rowMin, int rowMax, int depth, string datasetName)
    {
        //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        
        //cube.GetComponent<MeshRenderer>().enabled = false;
        //if (GameObject.Find("ScriptHolder").GetComponent<DataTransferManager>().visium)
        //{
        //    // linear approximation of SliceCollider position based on Screen dimensions
        //    float height = (float)Screen.height * 7.693f + 7780.92f; 
        //    float width = (float)Screen.width * 3.552f + 5805.2f;

        //    float posX = (float)Screen.width * 1.495f + 3864.75f;
        //    float posY = (float)Screen.height * (-3.415f) - 4038.34f;

        //    cube.transform.position = new Vector3(posX, posY, 0);
        //    cube.transform.localScale = new Vector3(width, height, 1);           
        //}
        //else
        //{
        //    // calculate size of collider TBD- some points not coverd
        //    var centerx = rowMax + (rowMin - rowMax) / 2;
        //    var centery = colMin + (colMax - colMin) / 2;
        //    cube.transform.position = new Vector3(centerx, centery, depth);
        //    cube.transform.localScale = new Vector3(rowMin - rowMax, colMax - colMin, 1);
        //}
        //sliceColliders.Add(cube);
        //zcoords.Add(depth);
        //Color newColor = cube.GetComponent<Renderer>().material.color;
        //newColor.a = 0f;
        //cube.GetComponent<Renderer>().material.color = newColor;
        //cube.AddComponent<DragObject>();
        //cube.GetComponent<DragObject>().resetCoords();
        //cube.GetComponent<DragObject>().setCenterPoint(cube.transform.position);
        //cube.GetComponent<DragObject>().setMetaData(colMin, colMax, rowMin, rowMax, depth, datasetName);
        //cube.GetComponent<Renderer>().material.color = transparentColor;
    
        //if (gameObject.GetComponent<DataTransferManager>().addHAndEImg)
        //{
        //    GameObject imagePlane = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //    imagePlane.name = "StainImageObject";

        //    imagePlane.transform.localScale = new Vector3(cube.transform.localScale.x *1.5f, 0.1f, cube.transform.localScale.z * 1.361f);
        //    imagePlane.transform.SetParent(cube.transform);

        //    imagePlane.transform.Rotate(new Vector3(-270, -90, 90));
        //    imagePlane.transform.localPosition = Vector3.zero;
        //    imagePlane.GetComponent<Renderer>().material = transparentMat;

        //    string imagePath = System.IO.Directory.GetCurrentDirectory() + "/Assets/Images/Error_Images/spatial_file_not_found.png";
        //    string[] files = Directory.GetFiles(datasetName, "*", SearchOption.AllDirectories);
        //    foreach (string s in files)
        //    {
        //        if (s.Split("\\").Last() == "tissue_hires_image.png") imagePath = s;
        //    }
        //    byte[] byteArray = File.ReadAllBytes(imagePath);
        //    Texture2D sampleTexture = new Texture2D(2, 2);
        //    bool isLoaded = sampleTexture.LoadImage(byteArray);
        //    imagePlane.GetComponent<Renderer>().material.mainTexture = sampleTexture;
        //    imagePlane.AddComponent<HAndEImageManager>();
        //    //TODO: correct path to Visium spatial image
        //    imagePlane.GetComponent<HAndEImageManager>().setImagePath(imagePath);
        //    imagePlane.AddComponent<BoxCollider>();
        //    HandEobjs.Add(imagePlane);
        //    imagePlane.SetActive(false);

        //}
    }

    private void Update()
    {
        //Movement of 3d object
        if (Input.GetMouseButton(0))
        {
            clicked();
        }
        
        if(Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.M))
        {
            deactivateModes();
            BtnPanels[0].SetActive(true);
            objectMove = true;
        }
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.R))
        {
            //switch Rotate mode
            deactivateModes();
            BtnPanels[1].SetActive(true);
            objectRotate = true;
        }
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.L))
        {
            //switch Resize mode
            deactivateModes();
            BtnPanels[2].SetActive(true);
            objectResize = true;
        }

        if (objectUsed)
        {
            if (objectMove) { moveObject(); }
            if (objectResize) resizeObject();
            if (objectRotate) rotateObject();
        }


    }

    private void deactivateModes()
    {
        btngroup.SetActive(true);
        objectResize = false;
        objectMove = false;
        objectRotate = false;
        foreach(GameObject go in BtnPanels)
        {
            go.SetActive(false);
        }
    }

    public List<GameObject> getHandEObjs()
    {
        return HandEobjs;
    }

    private Vector3 screenPos;
    private Vector3 worldPos;

    //recalculate spots based on user click on collider
    private void clicked()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(ray, out hit))
        {
            //TBD not using name cube here
            if (hit.collider.gameObject.name == "SliceCollider")
            {
                GameObject.Find("ScriptHolder").GetComponent<SpotDrawer>().identifySpot(hit.point.x, hit.point.y, hit.collider.gameObject.GetComponent<DragObject>().getDatasetName(), hit.collider.gameObject);
            }
        }
    }

    // detecting rotation of slices
    public void prepareRotation(int direction)
    {
        foreach (GameObject x in sliceColliders)
        {
            //if ( paths[dd.value].Contains(x.GetComponent<DragObject>().datasetName))
            {
                x.GetComponent<DragObject>().rotate(direction);
            }
        }
    }

    public void toggleObjectMovement(GameObject panel)
    {
        objectResize = false;
        objectRotate = false;
        BtnPanels[1].SetActive(false);
        BtnPanels[2].SetActive(false);
        if (panel.activeSelf) panel.SetActive(false);
        else panel.SetActive(true);
        objectMove = !objectMove;
    }
    
    public void toggleObjectRotate(GameObject panel)
    {
        objectMove = false;
        objectResize = false;
        BtnPanels[0].SetActive(false);
        BtnPanels[2].SetActive(false);
        if (panel.activeSelf) panel.SetActive(false);
        else panel.SetActive(true);
        objectRotate = !objectRotate;
    }

    public void toggleObjectResize(GameObject panel)
    {

        objectMove = false;
        objectRotate = false;        
        BtnPanels[0].SetActive(false);
        BtnPanels[1].SetActive(false);
        if (panel.activeSelf) panel.SetActive(false);
        else panel.SetActive(true);
        objectResize = !objectResize;
    }    
    /// <summary>
    /// Move  3D Object
    /// </summary>
    private void moveObject()
    {
        //KEYBINDING
        if (Input.GetKey(KeyCode.G))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                object3d.transform.Translate(Vector3.back * 10 * Time.deltaTime);

            }
            else
            {

                object3d.transform.Translate(Vector3.down * 10 * Time.deltaTime);
            }
        }
        if (Input.GetKey(KeyCode.T))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                object3d.transform.Translate(Vector3.forward * 10 * Time.deltaTime);

            }
            else
            {
                object3d.transform.Translate(Vector3.up * 10 * Time.deltaTime, Space.World);
            }
        }
        if (Input.GetKey(KeyCode.F))
        {

            
                object3d.transform.Translate(Vector3.left * 10 * Time.deltaTime, Camera.main.transform);
            
        }
        if (Input.GetKey(KeyCode.H))
        {

                object3d.transform.Translate(Vector3.right * 10 * Time.deltaTime, Camera.main.transform);
            
        }
    }

    /// <summary>
    /// Resize feature of 3d object
    /// </summary>
    private void resizeObject()
    {

        //KEYBINDING
        if (Input.GetKey(KeyCode.G))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                object3d.transform.localScale -= new Vector3(0, 0, 0.1f);
            }
            else
            {
                object3d.transform.localScale -= new Vector3(0, 0.1f, 0);
            }
        }
        if (Input.GetKey(KeyCode.T))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                object3d.transform.localScale += new Vector3(0, 0, 0.1f);
            }
            else
            {
                object3d.transform.localScale += new Vector3(0, 0.1f, 0);
            }
        }
        if (Input.GetKey(KeyCode.F))
        {

            object3d.transform.localScale += new Vector3(0.1f, 0, 0);

        }
        if (Input.GetKey(KeyCode.H))
        {
            object3d.transform.localScale -= new Vector3(0.1f, 0, 0);
        }
    }

    private void rotateObject()
    {

        //KEYBINDING
        if (Input.GetKey(KeyCode.H))
        {
           object3d.transform.eulerAngles -= new Vector3(0, 0.1f, 0);           
        }
        if (Input.GetKey(KeyCode.F))
        {
            object3d.transform.eulerAngles += new Vector3(0, 0.1f, 0);
        }
        if (Input.GetKey(KeyCode.T))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                object3d.transform.eulerAngles += new Vector3(0, 0, 0.1f);
            }
            else
            {
                object3d.transform.eulerAngles += new Vector3(0.1f, 0, 0);
            }
        }
        if (Input.GetKey(KeyCode.G))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                object3d.transform.eulerAngles -= new Vector3(0, 0, 0.1f);
            }
            else
            {
                object3d.transform.eulerAngles -= new Vector3(0.1f, 0, 0);
            }
        }
    }

    public GameObject topMenuPanel;

    public void objectGroupExpand(GameObject panel)
    {
        RectTransform topMenu_rect = topMenuPanel.GetComponent<RectTransform>();
        int topMenu_offset = 100;

        if (panel.activeSelf)
        {
            panel.SetActive(false);
            topMenu_rect.sizeDelta = new Vector2(topMenu_rect.sizeDelta.x - topMenu_offset, topMenu_rect.sizeDelta.y);
        }
        else { 
            panel.SetActive(true);
            topMenu_rect.sizeDelta = new Vector2(topMenu_rect.sizeDelta.x + topMenu_offset, topMenu_rect.sizeDelta.y);

        }
        if (btngroup.activeSelf) { 
            btngroup.SetActive(false); 
        }
        else { 
            btngroup.SetActive(true); 
        }
    }

}
