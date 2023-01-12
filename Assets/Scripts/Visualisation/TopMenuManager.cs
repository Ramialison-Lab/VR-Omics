/*
* Copyright (c) 2018 Liefe Science Informatics (university of Konstanz, Germany)
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
