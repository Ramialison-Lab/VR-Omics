using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ExportManager : MonoBehaviour
{
    private GameObject sh;
    //TBD add right datapath
    private string filePath = "C:\\Users\\Denis.Bienroth\\Desktop\\Testdatasets\\Outputs\\output.csv";
    StreamWriter writer;
    private void Start()
    {
        sh = GameObject.Find("ScriptHolder");
    }
    public void printCSV()
    {
        writeHeader();
        sh.GetComponent<SpotDrawer>().callDataForExport();
    }

    public void printLine(List<string> dataEntry)
    {
        writer.WriteLine(dataEntry[0] + "," + dataEntry[1] + "," + dataEntry[2] + "," +  dataEntry[3] + "," + dataEntry[4]);
    }

    private void writeHeader()
    {
        writer = new StreamWriter(filePath);

        writer.WriteLine("Barcode" + "," + "Expressionvalue" + "," + "Row" + "," + "Col" + "," + "Dataset" + "," + "Unique_ID");
    }
}
