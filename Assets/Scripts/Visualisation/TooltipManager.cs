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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

public class TooltipManager : MonoBehaviour
{
    [SerializeField] private int waitingTimeForTooltip = 1;
    [SerializeField] private string tooltipText = "";
    [SerializeField] private GameObject tooltipBox;
    private bool isActive = false;
    private Coroutine waitCoroutine;
    private Dictionary<string, string> tooltips = new Dictionary<string, string>();

    private void Start()
    {
        string filePath = Path.Combine(Application.dataPath, "Parameter_Files/Tooltip_info.txt");
        ReadCSVFile(filePath);
    }

    private void ReadCSVFile(string filePath)
    {
        try
        {
            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                string[] values = line.Split(',');

                if (values.Length >= 2)
                {
                    string field1 = values[0].Trim();
                    string field2 = values[1].Trim();

                    tooltips[field1] = field2;
                }
            }
        }
        catch (IOException e)
        {
            Debug.LogError("Error reading the CSV file: " + e.Message);
        }
    }

    public void enterTT(GameObject go)
    {
        isActive = true;

        if (waitCoroutine != null)
        {
            StopCoroutine(waitCoroutine);
        }

        waitCoroutine = StartCoroutine(WaitAndContinue(go));
    }

    private IEnumerator WaitAndContinue(GameObject go)
    {
        yield return new WaitForSeconds(waitingTimeForTooltip);

        if (isActive)
        {
            tooltipBox.SetActive(true);
            tooltipBox.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = GetDescription(go.name);
            tooltipBox.transform.position = new Vector2(Input.mousePosition.x + 20, Input.mousePosition.y + 20);
        }

        waitCoroutine = null;
    }

    public void exitTT()
    {
        tooltipBox.SetActive(false);
        isActive = false;
    }

    private string GetDescription(string name)
    {
        string tooltipText = "";

        if (tooltips.ContainsKey(name))
        {
            tooltipText = tooltips[name];
        }

        return tooltipText;
    }
}

