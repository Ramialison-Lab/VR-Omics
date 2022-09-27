using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
