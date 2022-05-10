using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AutoCompleteManager : MonoBehaviour
{
    public GameObject InputGameObject;
    public HashSet<string> geneNames;

    public void setGeneNameList(List<string> geneNames)
    {
        this.geneNames = new HashSet<string>(geneNames);
    }

    public void textEnter()
    {
        if(InputGameObject.GetComponent<TMP_InputField>().text == "") { return; }
        foreach(string x in geneNames)
        {
            if (x.StartsWith(InputGameObject.GetComponent<TMP_InputField>().text))
            {
                //create Button for each entry
                Debug.Log(x);
                // addListener with function to it and transfer button as GO

            }
        }
    }

    public void valueSelected(GameObject btn)
    {
        InputGameObject.GetComponent<TMP_InputField>().text = btn.name;

        //might need to be changed to lowercase 
        GameObject.Find("ScriptHolder").GetComponent<SearchManager>().readExpressionList(btn.name);
    }

}
