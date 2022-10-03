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
        if (gameObject.GetComponent<DataTransferManager>().addHAndEImg)
        {
            GameObject imagePlane = GameObject.CreatePrimitive(PrimitiveType.Cube);
            imagePlane.name = "StainImageObject";
            imagePlane.AddComponent<ImageAdjustment>();
        //  imagePlane.transform.SetParent(cube.transform);
        //  imagePlane.transform.localScale = new Vector3(0.1f, 1, 0.1f);
            imagePlane.transform.localScale = new Vector3(cube.transform.localScale.x *1.544f, 0.1f, cube.transform.localScale.z * 1.361f);
            imagePlane.transform.SetParent(cube.transform);

            imagePlane.transform.Rotate(new Vector3(-270, -90, 90));
            imagePlane.transform.localPosition = Vector3.zero;
            imagePlane.GetComponent<Renderer>().material = transparentMat;

            //TBD LINKPATH
            string imagepath = datasetName.Replace(datasetName.Split('\\').Last(), "");
            //byte[] byteArray = File.ReadAllBytes(imagepath + "\\figures\\show_spatial_all_hires.svg");
            byte[] byteArray = File.ReadAllBytes(imagepath + "\\spatial\\tissue_hires_image.png");
            Texture2D sampleTexture = new Texture2D(2, 2);
            bool isLoaded = sampleTexture.LoadImage(byteArray);
            calculateImageSize(datasetName);
            imagePlane.GetComponent<Renderer>().material.mainTexture = sampleTexture;
            imagePlane.AddComponent<HAndEImageManager>();
            imagePlane.GetComponent<HAndEImageManager>().setImagePath(imagepath + "\\spatial\\tissue_hires_image.png");
            imagePlane.AddComponent<BoxCollider>();
            imagePlane.GetComponent<HAndEImageManager>().createDragObjects();
            HandEobjs.Add(imagePlane);
            imagePlane.SetActive(false);

        }
    }
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            clicked();
        }
    }
    private void calculateImageSize(string dn)
    {
        string dnLoc = dn.Substring(0, dn.Length - dn.Split('\\').Last().Length);
        //get Scalefactor → HDF5 reader "uns/spatial/V1_Breast_Cancer_Block_A_Section_1/scalefactors/tissue_hires_scalef"
        float scalef = GameObject.Find("ScriptHolder").GetComponent<JSONManager>().readScaleFactor(dnLoc);

        float originalDim = 2000 / scalef;
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
                GameObject.Find("ScriptHolder").GetComponent<SpotDrawer>().identifySpot(hit.point.x, hit.point.y, hit.collider.gameObject.GetComponent<DragObject>().getDatasetName());
            }
        }
    }

    // detecting rotation of slices
    public void prepareRotation(int direction)
    {
        List<string> paths = GameObject.Find("ScriptHolder").GetComponent<DataTransferManager>().hdf5datapaths;

        foreach (GameObject x in sliceColliders)
        {
            if (x.GetComponent<DragObject>().datasetName == paths[dd.value])
            {
                x.GetComponent<DragObject>().rotate(direction);
            }
        }
    }

    public List<GameObject> getSliceColliders()
    {
        return sliceColliders;
    }
}
