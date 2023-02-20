using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class JSONReader : MonoBehaviour
{
    public static string ReadJsonFile(string filePath)
    {
        Debug.Log(filePath);

        // Check if the file exists
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"JSON file not found at {filePath}");
        }

        // Read the JSON file as a string
        string jsonString = File.ReadAllText(filePath);

        // Deserialize the JSON string to a C# object
        object jsonObject = JsonConvert.DeserializeObject(jsonString);

        // Serialize the C# object back to a JSON string
        string outputJson = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);

        // Return the output JSON string
        return outputJson;
    }

    public float readScaleFactor(string filePath)
    {
        float scaleFactor =0;

        string jsonContent = ReadJsonFile(filePath);

        string string_SF = extractScaleFactor(jsonContent);

        int index = string_SF.IndexOf(',');
        if (index != -1)
        {
            string_SF = string_SF.Substring(0, index);
        }
        string_SF.Substring(1);

        scaleFactor = float.Parse(string_SF);

        return scaleFactor;
    }

    private string extractScaleFactor(string jsonContent)
    {
        string start = "tissue_hires_scalef\":";
        string end = "\"fiducial_diameter_fullres";
        int startIndex = jsonContent.IndexOf(start);
        if (startIndex == -1) // If start string not found, return empty string
        {
            return string.Empty;
        }

        int endIndex = jsonContent.IndexOf(end, startIndex + start.Length);
        if (endIndex == -1) // If end string not found, return empty string
        {
            return string.Empty;
        }

        int length = endIndex - startIndex - start.Length;
        if (length < 0) // If end occurs before start, return empty string
        {
            return string.Empty;
        }

        string result = jsonContent.Substring(startIndex + start.Length, length);
        return result;
    }
}
