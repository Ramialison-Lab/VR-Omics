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
using UnityEngine;

public class SideMenuExpandManager : MonoBehaviour
{
    [SerializeField] private GameObject sideMenubtn;
    [SerializeField] private GameObject CoordinateResetPanel;

    private Transform myTransform;
    private Transform sideMenubtnTransform;
    private Transform CoordinateResetPanelTransform;

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

    private void Awake()
    {
        myTransform = transform;
        sideMenubtnTransform = sideMenubtn.transform;
        CoordinateResetPanelTransform = CoordinateResetPanel.transform;
    }

    private void Update()
    {
        if (move)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float complete = elapsedTime / duration;

            myTransform.position = Vector3.Lerp(PosA, PosB, complete);
            sideMenubtnTransform.position = Vector3.Lerp(PosABtn, PosBBtn, complete);
            CoordinateResetPanelTransform.position = Vector3.Lerp(PosA_Coord, PosB_Coord, complete);

            if (Mathf.Approximately(PosA.x, PosB.x) && Mathf.Approximately(PosA.y, PosB.y) && Mathf.Approximately(PosA.z, PosB.z))
            {
                move = false;
            }
        }
    }

    public void TogglePanel()
    {
        elapsedTime = 0;
        up = !up;
        int direction = up ? 150 : -150;
        PosB = new Vector3(myTransform.position.x + direction, myTransform.position.y, myTransform.position.z);
        PosBBtn = new Vector3(sideMenubtnTransform.position.x + direction, sideMenubtnTransform.position.y, sideMenubtnTransform.position.z);
        PosB_Coord = new Vector3(CoordinateResetPanelTransform.position.x + direction, CoordinateResetPanelTransform.position.y, CoordinateResetPanelTransform.position.z);
        PosA = myTransform.position;
        PosABtn = sideMenubtnTransform.position;
        PosA_Coord = CoordinateResetPanelTransform.position;
        move = true;
    }
}

