using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HAndEImageManager : MonoBehaviour
{
    public void createDragObjects()
    {
        return;

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

        GameObject diagDrag = GameObject.CreatePrimitive(PrimitiveType.Plane);
        diagDrag.name = "diagDrag";

        diagDrag.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        diagDrag.transform.SetParent(this.transform);
        diagDrag.transform.Rotate(new Vector3(-90, 0, 0));

        diagDrag.transform.position = new Vector3(transform.root.position.x * 2, 0, transform.root.position.x * 2);
        diagDrag.transform.localPosition = new Vector3(diagDrag.transform.localPosition.x, 0, heighthDrag.transform.localPosition.x);
        diagDrag.transform.localScale = new Vector3(0.05f, diagDrag.transform.localScale.y, diagDrag.transform.localScale.z);
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            clicked();
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
}
