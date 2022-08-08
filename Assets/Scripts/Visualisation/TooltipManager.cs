using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TooltipManager : MonoBehaviour
{

    public string tooltipText = "";
    public GameObject tooltipBox;


    public void enterTT(GameObject go)
    {
        tooltipBox.SetActive(true);   
        tooltipBox.transform.GetComponent<RectTransform>().position = go.transform.localPosition;
        tooltipBox.GetComponentInChildren<TMP_Text>().text = "Test";
    }

    public void exitTT()
    {
        tooltipBox.SetActive(false);
    }

}