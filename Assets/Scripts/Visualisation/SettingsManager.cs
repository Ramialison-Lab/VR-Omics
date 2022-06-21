using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{

    public GameObject containerPrefab;
    public GameObject colourPanel;
    public List<GameObject> containerList;

    private void Start()
    {
        containerList.Add(Instantiate(containerPrefab, colourPanel.transform));

    }
    public void AddContainer()
    {
        if(containerList.Count<10) containerList.Add(Instantiate(containerPrefab, colourPanel.transform));
    }

    public void RemoveContainer()
    {
        Destroy(containerList.Last());
        containerList.Remove(containerList.Last());
    }

}
