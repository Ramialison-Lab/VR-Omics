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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

public class SideMenuManager : MonoBehaviour
{
    public List<List<string>> SpotNameDictionary = new List<List<string>>();
    private DataTransferManager dfm;
    public GameObject TMPpro_text;
    public GameObject sidePanel;


    private void Start()
    {
        dfm = GameObject.Find("ScriptHolder").GetComponent<DataTransferManager>();
    }

    public void setSpotInfo(string SpotName, string Dataset, int id, Vector3 loc, float expVal)
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("sidePanelText")){
            Destroy(go);
        }
        int datasetId = dfm.visium_datapapths.IndexOf(Dataset);
        writeInfo(SpotName);
        //writeInfo("Dataset: " + Dataset.Split('\\').Last());
        writeInfo("Expressionvalue: " + expVal.ToString());
        writeInfo("Location: " + loc.ToString());

        int pos = dfm.SpotNameDictionary[datasetId].IndexOf(SpotName);
        readSpotInfo(pos, Dataset);

    }

    private void readSpotInfo(int pos, string dataset)
    {
        //TBD LINKPATH
        //TBD needs to be added when csv file is added to python pipeline!
        string[] lines = File.ReadAllLines(dataset.Replace(dataset.Split('\\').Last(), "") + "test2Csv.csv");

            List<string> values = new List<string>();
            values = lines.ToList<string>();

        List<string> res = values[pos].Split(',').ToList();
            writeInfo("n_genes_by_counts: " + res[4]);
            writeInfo("pct top 50: " + res[8]);
            writeInfo("cluster: " + res[16]);
            writeInfo("total mt: " + res[12]);
            writeInfo("Percantage mt: " + res[13]);
            writeInfo("n_counts: " + res[15]);

        res.Clear();
    }

    private void writeInfo(string info_text)
    {
        GameObject text = Instantiate(TMPpro_text, sidePanel.transform);

        text.GetComponent<TMP_Text>().text = info_text;

    }
}
