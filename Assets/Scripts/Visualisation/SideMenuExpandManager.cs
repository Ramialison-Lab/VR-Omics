using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideMenuExpandManager : MonoBehaviour
{
    private Vector3 PosA;
    private Vector3 PosB;
    private Vector3 PosABtn;
    private Vector3 PosBBtn;    
    private Vector3 PosA_Coord;
    private Vector3 PosB_Coord;
    private float duration = 0.2f;
    private float elapsedTime;
    private bool move = false;
    private bool up = true;
    public GameObject sideMenubtn;
    public GameObject CoordinateResetPanel;

    void Update()
    {
        if (move)
        {
            elapsedTime += Time.deltaTime;
            float complete = elapsedTime / duration;

            transform.position = Vector3.Lerp(PosA, PosB, complete);
            sideMenubtn.transform.position = Vector3.Lerp(PosABtn, PosBBtn, complete);
            CoordinateResetPanel.transform.position = Vector3.Lerp(PosA_Coord, PosB_Coord, complete);
        }
        if (PosA == PosB) move = false;
    }

    public void togglePanel()
    {
        elapsedTime = 0;
        if (up)
        {
            PosB = new Vector3(transform.position.x-150, transform.position.y, transform.position.z);
            PosBBtn = new Vector3(sideMenubtn.transform.position.x-150, sideMenubtn.transform.position.y, sideMenubtn.transform.position.z);
            PosB_Coord = new Vector3(CoordinateResetPanel.transform.position.x - 150, CoordinateResetPanel.transform.position.y, CoordinateResetPanel.transform.position.z);
            up = false;
        }
        else
        {
            PosB = new Vector3(transform.position.x +150, transform.position.y, transform.position.z);
            PosBBtn = new Vector3(sideMenubtn.transform.position.x+150, sideMenubtn.transform.position.y, sideMenubtn.transform.position.z);
            PosB_Coord = new Vector3(CoordinateResetPanel.transform.position.x+150, CoordinateResetPanel.transform.position.y, CoordinateResetPanel.transform.position.z);
            up = true;
        }
        PosA = transform.position;
        PosABtn = sideMenubtn.transform.position;
        PosA_Coord = CoordinateResetPanel.transform.position;
        move = true;

    }
}
