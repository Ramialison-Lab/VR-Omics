using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
using TMPro;
using System;

public class TabManger : MonoBehaviour
{
    private TMP_InputField[] ifs;

    private void Start()
    {
        ifs = gameObject.GetComponentsInChildren<TMP_InputField>();
    }

    private void Update()
    {
        if (ifs.Length > 0)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                foreach (TMP_InputField tmp_if in ifs)
                {
                    if (tmp_if.isFocused)
                    {
                        for (int i = 0; i < ifs.Length; i++)
                        {
                            if (ifs[i] == tmp_if)
                            {
                                try { ifs[i + 1].Select(); } catch (Exception) { ifs[0].Select(); }
                            }
                        }
                    }
                }
            }
        }
    }
}
