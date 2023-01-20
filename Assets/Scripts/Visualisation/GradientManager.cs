/*
* Copyright (c) 2023 Life Science Informatics (university of Konstanz, Germany)
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

public class GradientManager : MonoBehaviour
{
    [SerializeField]
    Gradient gradient;
    [SerializeField]
    float duration;
    float t = 0f;
    private bool gradientExpand = false;

    //void Start()
    //{
    //    var gck = new GradientColorKey[5];
    //    var alphakeys = new GradientAlphaKey[2];
    //    float rgb = 255;

    //    gck[0].color = new Color(65 / rgb, 105 / rgb, 255 / rgb); // Blue
    //    gck[0].time = 0f;
    //    gck[1].color = new Color(135 / rgb, 206 / rgb, 250 / rgb); // Cyan
    //    gck[1].time = .25f;
    //    gck[2].color = new Color(60 / rgb, 179 / rgb, 113 / rgb); // green
    //    gck[2].time = 0.50F;
    //    gck[3].color = new Color(255 / rgb, 230 / rgb, 0); // yellow
    //    gck[3].time = 0.75F;
    //    gck[4].color = new Color(180 / rgb, 0, 0); // Red
    //    gck[4].time = 1f;

    //    alphakeys[0].alpha = 1f;
    //    alphakeys[0].time = 0f;
    //    alphakeys[1].alpha = 1f;
    //    alphakeys[1].time = 1f;

    //    gradient.SetKeys(gck, alphakeys);
    //}

    //void Update()
    //{
    //    float value = Mathf.Lerp(0f, 1f, t);
    //    t += Time.deltaTime / duration;
    //    Color color = gradient.Evaluate(value);
    //    this.gameObject.GetComponent<Renderer>().material.color = color;
    //}

    public void ExpandGradient(GameObject gradientPanel)
    {
        if (gradientExpand)
        {
            gradientPanel.GetComponent<RectTransform>().sizeDelta = new Vector3(230,60,0);
            gradientExpand = false;        }
        else
        {
            gradientPanel.GetComponent<RectTransform>().sizeDelta = new Vector3(400,60,0);
            gradientExpand = true;
        }
    }
}
