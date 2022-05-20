using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

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
    }
}
