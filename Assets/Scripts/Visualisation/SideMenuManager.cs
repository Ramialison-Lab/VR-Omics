using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

public class SideMenuManager : MonoBehaviour
{
    TMP_Text[] texts;
    public List<List<string>> SpotNameDictionary = new List<List<string>>();
    private DataTransferManager dfm;

    private void Start()
    {
        texts = this.gameObject.GetComponentsInChildren<TMP_Text>();
        foreach (TMP_Text tt in texts) Debug.Log(tt.name);

        dfm = GameObject.Find("ScriptHolder").GetComponent<DataTransferManager>();
    }

    public void setSpotInfo(string SpotName, string Dataset, int id, Vector3 loc, float expVal)
    {
        int datasetId = dfm.hdf5datapaths.IndexOf(Dataset);
        foreach (TMP_Text tt in texts) tt.text = "";
        texts[0].text = SpotName;
        texts[1].text = "Dataset: " + Dataset.Split('\\').Last();
        texts[2].text = "Expressionvalue: " + expVal.ToString();
        texts[3].text = "Location: " + loc.ToString();

        int pos = dfm.SpotNameDictionary[datasetId].IndexOf(SpotName);
       // readSpotInfo(pos, Dataset);

    }

    private void readSpotInfo(int pos, string dataset)
    {
        //TBD LINKPATH
        //TBD needs to be added when csv file is added to python pipeline!
        string[] lines = File.ReadAllLines(dataset.Replace(dataset.Split('\\').Last(), "") + "test2Csv.csv");

            List<string> values = new List<string>();
            values = lines.ToList<string>();

        List<string> res = values[pos].Split(',').ToList();
            texts[4].text = "n_genes_by_counts: " + res[4];
            texts[5].text = "pct top 50: " + res[8];
            texts[6].text = "cluster: " + res[16];
            texts[7].text = "total mt: " + res[12];
            texts[8].text = "Percantage mt: " + res[13];
            texts[9].text = "n_counts: " + res[15];

        res.Clear();
    }
}
