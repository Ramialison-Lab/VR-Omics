using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuCanvas : MonoBehaviour
{
    private Color backupCol;
    public GameObject lockBtn;
    public GameObject ulockBtn;
    private SpotDrawer sd;
    public bool locked = true;
    public bool lasso = false;
    public GameObject sidemenu;
    private void Start()
    {
        sd = GameObject.Find("ScriptHolder").GetComponent<SpotDrawer>();
        backupCol = Camera.main.backgroundColor;
        sd.setMinTresh(0f);
        sd.setMaxTresh(1f);
    }

    public void unlockRotation()
    {
        locked = false;
        lockBtn.SetActive(true);
        ulockBtn.SetActive(false);
    }

    public void lockRotation()
    {
        locked = true;
        ulockBtn.SetActive(true);
        lockBtn.SetActive(false);

    }

    private bool darkmode = false;
    public void darkMode()
    {
        if (!darkmode)
        {
            darkmode = true;
            Camera.main.backgroundColor = Color.black;
        }
        else
        {
            Camera.main.backgroundColor = backupCol;
            darkmode = false;
        }
    }

    public void setColorMinTreshold(GameObject slider)
    {
        sd.setMinTresh(slider.GetComponent<Slider>().value);

    }

    public void setColorMaxTreshold(GameObject slider)
    {
        sd.setMaxTresh(slider.GetComponent<Slider>().value);
    }

    public GameObject sp;
    public void setSphereSize(GameObject slider)
    {
        Slider sl = slider.GetComponent<Slider>();
        sp.transform.localScale = new Vector3(sl.value*10,sl.value*10,sl.value*10);
    }

    public void toggleLasso()
    {
        lasso = !lasso;
    }

    public bool getLasso()
    {
        return lasso;
    }

    public void toggleSideMenu()
    {
        if (sidemenu.activeSelf)
        {
            sidemenu.SetActive(false);
        }
        else
        {
            sidemenu.SetActive(true);
        }
    }
}
