using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{

    public GameObject containerPrefab;
    public GameObject colourPanel;
    public List<GameObject> containerList;

    private void Start()
    {
        containerList.Add(Instantiate(containerPrefab, colourPanel.transform));
        containerList.Add(Instantiate(containerPrefab, colourPanel.transform));
        containerList.Add(Instantiate(containerPrefab, colourPanel.transform));
        containerList.Add(Instantiate(containerPrefab, colourPanel.transform));
        containerList.Add(Instantiate(containerPrefab, colourPanel.transform));

    }
    public void AddContainer()
    {
        if(containerList.Count<8) containerList.Add(Instantiate(containerPrefab, colourPanel.transform));
    }

    public void RemoveContainer()
    {
        Destroy(containerList.Last());
        containerList.Remove(containerList.Last());
    }
    public List<string> colourParam;
    public void submitSelection()
    {
        colourParam.Clear();
        foreach(GameObject go in containerList)
        {
           // TMP_InputField txt = go.GetComponentInChildren<TMP_InputField>();

            colourParam.Add("NaN");
            InputField[] rgbs = go.GetComponentsInChildren<InputField>();
            foreach (InputField rgb in rgbs)
            {
                colourParam.Add(rgb.text);
            }

            TMP_Dropdown colourDropdown = go.GetComponentInChildren<TMP_Dropdown>();
            colourParam.Add(colourDropdown.options[colourDropdown.value].text);
        }

        GameObject.Find("ScriptHolder").GetComponent<SpotDrawer>().setColourScheme(colourParam);

    }

    public void defaultColour()
    {
        GameObject.Find("ScriptHolder").GetComponent<SpotDrawer>().defaultColour();
    }
}
