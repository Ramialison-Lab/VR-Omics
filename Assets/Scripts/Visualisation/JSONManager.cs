using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JSONManager : MonoBehaviour
{
    public float readScaleFactor(string path)
    {
        float scaleFactor = -1;
        try
        {
            //   var jsonString = File.ReadAllText(Application.dataPath + "/Datasets/scalefactors_json.json");
            var jsonString = File.ReadAllText(path + "/spatial/scalefactors_json.json");
            string target = "tissue_hires_scalef\": ";
            string end = ", \"fiducial";
            scaleFactor = float.Parse(jsonString.Substring(jsonString.IndexOf(target) + target.Length, jsonString.IndexOf(end) - (jsonString.IndexOf("tissue_hires_scalef\": ") + target.Length)));
        }
        catch (Exception) {}
        return scaleFactor;

    }
}
