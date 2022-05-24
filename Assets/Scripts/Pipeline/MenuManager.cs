using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Button visiumButton;
    public Button csvButton;
    public GameObject csvbtn;
    public GameObject visiumbtn;
    public GameObject ortext;
    public GameObject uploadContextMenu;
    public GameObject downloadContextMenu;
    public GameObject p_selection;
    public GameObject uploadlocal;

    //TBD Mouseover gameobjects
    public GameObject visText;
    public GameObject csvText;

    // This function is called if ths csv upload option was selected to upload a csv database from the local machine
    public void CSVoptionSelected()
    {

        fadeFunction();
        visiumbtn.SetActive(false);
        uploadContextMenu.SetActive(true);
    }

    // this function is callledd if the database needs to be downloaded and pre-processed from https://www.10xgenomics.com/
    public void VisiumoptionSelected()
    {

        fadeFunction();
        csvbtn.SetActive(false);
        downloadContextMenu.SetActive(true);
    }

    // TBD Visual effect for selection of one of the options csv or 10x  
    private void fadeFunction()
    {
        csvButton.GetComponent<Image>().CrossFadeAlpha(0.2f, 1.0f, false);
        visiumButton.GetComponent<Image>().CrossFadeAlpha(0.2f, 1.0f, false);
        ortext.SetActive(false);
    }

    //TBD anything from here belongs to section MouseOver 
    public void MouseEnterCSV()
    {
        csvText.SetActive(true);
    }

    public void MouseEnterVisium()
    {
        visText.SetActive(true);
    }

    public void MouseExitBoth()
    {
        visText.SetActive(false);
        csvText.SetActive(false);
    }
    public void returnScreen()
    {
        uploadContextMenu.SetActive(false);
        downloadContextMenu.SetActive(false);
        ortext.SetActive(true);
        visiumbtn.SetActive(true);
        csvbtn.SetActive(true);
        if (p_selection.activeSelf) p_selection.SetActive(false);
        if (uploadlocal.activeSelf) uploadlocal.SetActive(false);
    }

}
