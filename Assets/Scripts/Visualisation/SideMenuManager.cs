using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System.IO;
using System;

public class SideMenuManager : MonoBehaviour
{
    TMP_Text[] texts;

    private void Start()
    {
        texts = this.gameObject.GetComponentsInChildren<TMP_Text>();

    }

    public void setSpotInfo(string SpotName, string Dataset, int id, Vector3 loc)
    {
        foreach (TMP_Text tt in texts) tt.text = "";
        texts[0].text = SpotName;
        texts[1].text = "Dataset: " +Dataset.Split('\\').Last();
        texts[2].text = "identifier: " +id.ToString();
        texts[3].text = "Location: " + loc.ToString();
        readSpotInfo(id, Dataset.Replace(Dataset.Split('\\').Last(),"")); ;
    }

    private void readSpotInfo(int pos, string dataset)
    {
        string[] lines = File.ReadAllLines(dataset + "\\test2Csv.csv");
        if (lines.Length > pos)
        {
                List<string> values = new List<string>();
                values = lines[pos].Split(',').ToList();
            texts[4].text = "n_genes_by_counts: " + values[4];
            texts[5].text = "pct top 50: " + values[8];
            texts[6].text = "cluster: " + values[16];
            texts[7].text = "total mt: " + values[12];
            texts[8].text = "Percantage mt: " + values[13];
            texts[9].text = "n_counts: " + values[15];
        }
    }
}
