using System.IO;
using System.Linq;
using UnityEngine;
using TMPro;
using System;

public class ReadGeneInformation : MonoBehaviour
{
    public GameObject sidePanel;
    public GameObject TMPpro_text;
    DataTransferManager dfm;

    string[] header;

    private void Start()
    {
        dfm = GetComponent<DataTransferManager>();
    }

    public void readGeneInformation(string geneName)
    {
        if (dfm.xenium)
        {
            readXeniumGeneInfo(geneName);
        }
    }

    private void readXeniumGeneInfo(string geneName)
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("sidePanelText"))
        {
            Destroy(go);
        }     
        

        string[] lines = File.ReadAllLines(dfm.xeniumGenePanelPath);

        header = lines[0].Split(',');

        foreach (string line in lines)
        {
            string[] values = line.Split(',');

            if (values[0].ToLower() == geneName.ToLower())
            {
                for (int i = 1; i <values.Length ; i++) {
                    try
                    {
                        readInfo(values,i);
                    }
                    catch (Exception e) { } }
            }
        }

        lines = File.ReadAllLines(dfm.moran_results);

        header = lines[0].Split(',');

        foreach (string line in lines)
        {
            string[] values = line.Split(',');

            if (values[0].ToLower() == geneName.ToLower())
            {
                for (int i = 1; i < values.Length; i++)
                {
                    try
                    {
                        readInfo(values, i);
                    }
                    catch (Exception e) { }
                }
            }
        }


    }

    private void readInfo(string[] values, int index)
    {
        GameObject text = Instantiate(TMPpro_text, sidePanel.transform);

        string header_text = header[index];
        if (header_text.Length > 20) header_text = header_text.Substring(0, 20);
        string value_text = values[index];
        if (value_text.Length > 15) value_text = value_text.Substring(0, 15);

        text.GetComponent<TMP_Text>().text = header_text + ": " + values[index];

    }
}
