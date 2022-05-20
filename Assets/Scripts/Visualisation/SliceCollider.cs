using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using System.Linq;

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

    public void setSliceCollider(int lslice, int rslice, int topslice, int btmslice, int d, string datasetName)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        
        var centerx = lslice + (rslice - lslice) / 2;
        var centery = btmslice + (topslice - btmslice) / 2;
        cube.transform.position = new Vector3(centerx, centery, d);
        cube.transform.localScale = new Vector3(rslice - lslice, topslice - btmslice, 1);
        cube.GetComponent<MeshRenderer>().enabled = false;
        sliceColliders.Add(cube);
        zcoords.Add(d);
        cube.AddComponent<DragObject>();
        cube.GetComponent<DragObject>().resetCoords(datasetName);
        //cube.GetComponent<DragObject>().setCenterPoint(new Vector3(cube.transform.localScale.x / 2, cube.transform.localScale.y / 2, cube.transform.position.z));
        cube.GetComponent<DragObject>().setCenterPoint(cube.transform.position);
        Color newColor = cube.GetComponent<Renderer>().material.color;
        newColor.a = 0f;
        cube.GetComponent<Renderer>().material.color = newColor;
        string imagepath = datasetName.Replace(datasetName.Split('\\').Last(),"");
        Debug.Log(imagepath);
        
        
        
        //TBD overlay H&E stain image read image 
        //byte[] byteArray = File.ReadAllBytes(imagepath + "\\spatial\\tissue_hires_image.png");
        //Texture2D sampleTexture = new Texture2D(2, 2);
        //bool isLoaded = sampleTexture.LoadImage(byteArray);



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
        if (Input.GetMouseButtonDown(0))
        {
            clicked();
        }
    }

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

    public void prepareRotation(int direction)
    {
        List<string> paths = GameObject.Find("ScriptHolder").GetComponent<DataTransferManager>().getDatasetpaths();

        foreach(GameObject x in sliceColliders)
        {
            if (x.GetComponent<DragObject>().datasetName == paths[dd.value])
            {
                x.GetComponent<DragObject>().rotate(direction);
            }
        }
    }
}
