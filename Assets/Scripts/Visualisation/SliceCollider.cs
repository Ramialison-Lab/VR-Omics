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

    // Adding a collider slice to each of the Visium slices to detect user input
    public void setSliceCollider(int btmslice, int topslice, int rslice, int lslice, int d, string datasetName)
    {

        // create cube as slice
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        // calculate size of collider TBD- some points not coverd
        var centerx = lslice + (rslice - lslice) / 2;
        var centery = btmslice + (topslice - btmslice) / 2;
        cube.transform.position = new Vector3(centerx, centery, d);
        cube.transform.localScale = new Vector3(rslice - lslice, topslice - btmslice, 1);
        // make them invisible
        cube.GetComponent<MeshRenderer>().enabled = false;
        sliceColliders.Add(cube);
        zcoords.Add(d);
        // attach DragObject script to move the slices
        cube.AddComponent<DragObject>();
        cube.GetComponent<DragObject>().resetCoords(datasetName);
        cube.GetComponent<DragObject>().setCenterPoint(cube.transform.position);
        Color newColor = cube.GetComponent<Renderer>().material.color;
        newColor.a = 0f;
        cube.GetComponent<Renderer>().material.color = newColor;

        GameObject imagePlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        imagePlane.transform.SetParent(cube.transform);
        imagePlane.transform.localScale = new Vector3(0.1f, 1, 0.1f);
        imagePlane.transform.Rotate(new Vector3(-90, 0, 0));
        imagePlane.transform.localPosition = new Vector3(0, 0, -2);
        string imagepath = datasetName.Replace(datasetName.Split('\\').Last(), "");
        byte[] byteArray = File.ReadAllBytes(imagepath + "\\spatial\\tissue_hires_image.png");
        Texture2D sampleTexture = new Texture2D(2, 2);
        bool isLoaded = sampleTexture.LoadImage(byteArray);

        imagePlane.GetComponent<Renderer>().material.mainTexture = sampleTexture;
        imagePlane.AddComponent<HAndEImageManager>();
        imagePlane.AddComponent<BoxCollider>();
        imagePlane.GetComponent<HAndEImageManager>().createDragObjects();
        //GameObject newCanvas = new GameObject("Canvas");
        //Canvas c = newCanvas.AddComponent<Canvas>();
        //c.renderMode = RenderMode.ScreenSpaceOverlay;
        //newCanvas.AddComponent<CanvasScaler>();
        //newCanvas.AddComponent<GraphicRaycaster>();
        //GameObject panel = new GameObject("Panel");
        //panel.AddComponent<CanvasRenderer>();
        //RawImage i = panel.AddComponent<RawImage>();
        //panel.transform.SetParent(newCanvas.transform, false);
        //newCanvas.transform.position = new Vector3(0, 0, 0);
        //newCanvas.transform.SetParent(cube.transform);

        //if (isLoaded)
        //{
        //    i.texture = sampleTexture;
        //}
    }
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            clicked();
        }
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
                GameObject.Find("ScriptHolder").GetComponent<SpotDrawer>().identifySpot(hit.point.x, hit.point.y, hit.collider.gameObject.GetComponent<DragObject>().getDatasetName());
            }
        }
    }

    // detecting rotation of slices
    public void prepareRotation(int direction)
    {
        List<string> paths = GameObject.Find("ScriptHolder").GetComponent<DataTransferManager>().getDatasetpaths();

        foreach (GameObject x in sliceColliders)
        {
            if (x.GetComponent<DragObject>().datasetName == paths[dd.value])
            {
                x.GetComponent<DragObject>().rotate(direction);
            }
        }
    }
}
