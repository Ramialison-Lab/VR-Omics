using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataTransfer : MonoBehaviour
{
    public bool visium =false;
    public bool c18 = false;
    public bool xenium = false;
    public bool tomoseq = false;

    public void setTEchnology(string str) {

        switch (str)
        {

            case "visium": visium = true;
                break;
            case "c18": c18 = true;
                break;
            case "xenium": xenium = true;
                break;
            case "tomoseq": tomoseq = true;
                break;
        }

    }


    public void startC18()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
