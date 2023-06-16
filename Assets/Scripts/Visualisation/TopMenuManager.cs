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

public class TopMenuManager : MonoBehaviour
{
    private Vector3 startPos;
    private Vector3 endPos;
    private Vector3 startBtnPos;
    private Vector3 endBtnPos;
    private float duration = 0.2f;
    private float elapsedTime;
    private bool move = false;
    private bool up = true;

    public GameObject topMenubtn;

    private void Update()
    {
        if (move)
        {
            elapsedTime += Time.deltaTime;
            float complete = elapsedTime / duration;

            transform.position = Vector3.Lerp(startPos, endPos, complete);
            topMenubtn.transform.position = Vector3.Lerp(startBtnPos, endBtnPos, complete);
        }

        if (Mathf.Approximately(transform.position.y, endPos.y))
        {
            move = false;
        }
    }

    public void TogglePanel()
    {
        elapsedTime = 0;
        int direction = up ? -50 : 50;

        startPos = transform.position;
        startBtnPos = topMenubtn.transform.position;

        endPos = startPos + new Vector3(0, direction, 0);
        endBtnPos = startBtnPos + new Vector3(0, direction, 0);

        up = !up;
        move = true;
    }
}

