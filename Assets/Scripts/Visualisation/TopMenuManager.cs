using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopMenuManager : MonoBehaviour
{
    private Vector3 PosA;
    private Vector3 PosB;    
    private Vector3 PosABtn;
    private Vector3 PosBBtn;
    private float duration = 0.2f;
    private float elapsedTime;
    private bool move = false;
    private bool up = true;
    public GameObject topMenubtn;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (move)
        {
            elapsedTime += Time.deltaTime;
            float complete = elapsedTime / duration;

            transform.position = Vector3.Lerp(PosA, PosB, complete);
            topMenubtn.transform.position = Vector3.Lerp(PosABtn, PosBBtn, complete);
        }
        if (PosA == PosB) move = false;
    }

    public void togglePanel() {
        elapsedTime = 0;
        if (up)
        {
            PosB = new Vector3(transform.position.x, transform.position.y - 50, transform.position.z);
            PosBBtn = new Vector3(topMenubtn.transform.position.x, topMenubtn.transform.position.y - 50, topMenubtn.transform.position.z);
            up = false;
        }
        else
        {
            PosB = new Vector3(transform.position.x, transform.position.y + 50, transform.position.z) ;
            PosBBtn = new Vector3(topMenubtn.transform.position.x, topMenubtn.transform.position.y + 50, topMenubtn.transform.position.z);
            up = true;
        }
        PosA = transform.position;
        PosABtn = topMenubtn.transform.position;
        move = true;
    
    }
}
