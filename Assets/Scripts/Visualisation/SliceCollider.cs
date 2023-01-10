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

    public bool objectUsed = false;
    public bool objectResize = false;
    public bool objectMove = false;
    public bool objectRotate = false;

    private Color transparentColor = new Color(1, 1, 1, 0);
    private void Start()
    {
        dfm = GameObject.Find("ScriptHolder").GetComponent<DataTransferManager>();
    }
    public void setSliceCollider(int colMin, int colMax, int rowMin, int rowMax, int depth, string datasetName)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        
        cube.GetComponent<MeshRenderer>().enabled = false;
        if (GameObject.Find("ScriptHolder").GetComponent<DataTransferManager>().visium)
        {

            // linear approximation of SliceCollider position based on Screen dimensions
            float height = (float)Screen.height * 7.693f + 7780.92f; 
            float width = (float)Screen.width * 3.552f + 5805.2f;

            float posX = (float)Screen.width * 1.495f + 3864.75f;
            float posY = (float)Screen.height * (-3.415f) - 4038.34f;

            cube.transform.position = new Vector3(posX, posY, 0);
            cube.transform.localScale = new Vector3(width, height, 1);           

        }
        else
        {
            // calculate size of collider TBD- some points not coverd
            var centerx = rowMax + (rowMin - rowMax) / 2;
            var centery = colMin + (colMax - colMin) / 2;
            cube.transform.position = new Vector3(centerx, centery, depth);
            cube.transform.localScale = new Vector3(rowMin - rowMax, colMax - colMin, 1);
        }
        sliceColliders.Add(cube);
        zcoords.Add(depth);
        Color newColor = cube.GetComponent<Renderer>().material.color;
        newColor.a = 0f;
        cube.GetComponent<Renderer>().material.color = newColor;
        cube.AddComponent<DragObject>();
        cube.GetComponent<DragObject>().resetCoords();
        cube.GetComponent<DragObject>().setCenterPoint(cube.transform.position);
        cube.GetComponent<DragObject>().setMetaData(colMin, colMax, rowMin, rowMax, depth, datasetName);
        cube.GetComponent<Renderer>().material.color = transparentColor;
    
        if (gameObject.GetComponent<DataTransferManager>().addHAndEImg)
        {
            GameObject imagePlane = GameObject.CreatePrimitive(PrimitiveType.Cube);
            imagePlane.name = "StainImageObject";

            imagePlane.transform.localScale = new Vector3(cube.transform.localScale.x *1.5f, 0.1f, cube.transform.localScale.z * 1.361f);
            imagePlane.transform.SetParent(cube.transform);

            imagePlane.transform.Rotate(new Vector3(-270, -90, 90));
            imagePlane.transform.localPosition = Vector3.zero;
            imagePlane.GetComponent<Renderer>().material = transparentMat;

            string imagePath = System.IO.Directory.GetCurrentDirectory() + "/Assets/Images/Error_Images/spatial_file_not_found.png";
            string[] files = Directory.GetFiles(datasetName, "*", SearchOption.AllDirectories);
            foreach (string s in files)
            {
                if (s.Split("\\").Last() == "tissue_hires_image.png") imagePath = s;
            }
            byte[] byteArray = File.ReadAllBytes(imagePath);
            Texture2D sampleTexture = new Texture2D(2, 2);
            bool isLoaded = sampleTexture.LoadImage(byteArray);
            imagePlane.GetComponent<Renderer>().material.mainTexture = sampleTexture;
            imagePlane.AddComponent<HAndEImageManager>();
            //TODO: correct path to Visium spatial image
            imagePlane.GetComponent<HAndEImageManager>().setImagePath(imagePath);
            imagePlane.AddComponent<BoxCollider>();
            HandEobjs.Add(imagePlane);
            imagePlane.SetActive(false);

        }
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
            if (hit.collider.gameObject.name == "Cube")
            {
                GameObject.Find("ScriptHolder").GetComponent<SpotDrawer>().identifySpot(hit.point.x, hit.point.y, hit.collider.gameObject.GetComponent<DragObject>().getDatasetName(), hit.collider.gameObject);
            }
        }
    }

    // detecting rotation of slices
    public void prepareRotation(int direction)
    {

        List<string> paths = GameObject.Find("ScriptHolder").GetComponent<DataTransferManager>().hdf5datapaths;
       

        foreach (GameObject x in sliceColliders)
        {
            if ( paths[dd.value].Contains(x.GetComponent<DragObject>().datasetName))
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

    public void objectGroupExpand(GameObject panel)
    {
        if (panel.activeSelf) panel.SetActive(false);
        else panel.SetActive(true);
        if (btngroup.activeSelf) btngroup.SetActive(false);
        else btngroup.SetActive(true);
    }

}
