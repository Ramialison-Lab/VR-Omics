using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisiumParameters : MonoBehaviour
{
    public Dropdown m_Dropdown;
    public string str = "";
    public GameObject DownloadUI;
    public GameObject p_selection;
    public List<string> paramList = new List<string>();
    public GameObject warningMessage;
    public GameObject w_text_format;

    // Called if dropdown value is changed → selected database parameter
    public void dropdownValueChanged()
    {
        str = GameObject.Find("m_dropdown").GetComponent<Dropdown>()
            .options[GameObject.Find("m_dropdown").GetComponent<Dropdown>().value].text;
    }

    // called if database was selected to enter the filter parameters
    public void startParameterSelection()
    {

        if (str == "")
        {
            warningMessage.SetActive(true);
        }
        else
        {
            DownloadUI.SetActive(false);
            p_selection.SetActive(true);
            if (warningMessage.activeSelf) warningMessage.SetActive(false);
        }
    }

    // called to get all filter parameters from the input fields
    public void getAllParametersFromInputFields()
    {
        int number;
        foreach (InputField inputField in p_selection.GetComponentsInChildren<InputField>())
        {
            foreach (Text text in inputField.GetComponentsInChildren<Text>())
            {
                if (int.TryParse(text.text, out number))
                {
                    paramList.Add(text.text);
                }
                else
                {
                    w_text_format.SetActive(true);
                    paramList.Clear();
                    break;
                }
            }
        }

        //TBD
        // these parameters need to be passed to the notebook
        // str is the paramter for the sampl_id in scanpy https://scanpy.readthedocs.io/en/stable/generated/scanpy.datasets.visium_sge.html
        // paramList holds all parameters for the filter section of the notebook 

    }

}
