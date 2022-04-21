using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class CustomDataSetUpload : MonoBehaviour
{
    List<float> x = new List<float>();
    List<float> y = new List<float>();
    List<float> z = new List<float>();


    private void Start()
    {
        //StartCoroutine(startReadingCustom());
        //GameObject.Find("SpotDrawer").GetComponent<SpotDrawer>().startSpotDrawerCustom();

    }

    IEnumerator startReadingCustom()
    {
        // Test data C18 heart as CSV

        List<string> values = new List<string>();

        string[] lines = File.ReadAllLines("Assets/Datasets/rtest.csv");
        lines = lines.Skip(1).ToArray();

        foreach (string line in lines)
        {
            values = line.Split(',').ToList();
            x.Add(float.Parse(values[9]));
            y.Add(float.Parse(values[10]));
            z.Add(float.Parse(values[11]));

        }

        yield return null;
    }

    public int getSize()
    {
        return x.Count;
    }
    public List<float> getX()
    {
        return x;
    }    
    
    public List<float> getY()
    {
        return y;
    }    
    
    public List<float> getZ()
    {
        return z;
    }

}
