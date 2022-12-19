using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonFunctionManager : MonoBehaviour
{
    DataTransfer df;
    public List<GameObject> functionPanels = new List<GameObject>();

    public void setFunction(DataTransfer df)
    {
        this.df = df;
        if (df.c18)
        {
            //C18
            functionPanels[0].SetActive(false);
            functionPanels[1].SetActive(false);
            functionPanels[2].SetActive(false);
            functionPanels[4].SetActive(false);
            functionPanels[5].SetActive(false);
            functionPanels[6].SetActive(false);
            functionPanels[10].SetActive(false);
            functionPanels[11].SetActive(false);
            functionPanels[16].SetActive(false);

        }
        else if ((df.visium && !df.visiumMultiple) && !df.c18)
        {
            functionPanels[17].SetActive(false);

        }

        else if ((df.visium || df.visiumMultiple) && !df.c18)
        {
            // visium multiple slices
            functionPanels[10].SetActive(false);
            functionPanels[16].SetActive(false);
            functionPanels[17].SetActive(false);


        }
        else if (df.tomoseq)
        {
            functionPanels[0].SetActive(false);
            functionPanels[1].SetActive(false);
            functionPanels[2].SetActive(false);
            functionPanels[3].SetActive(false);
            functionPanels[4].SetActive(false);
            functionPanels[5].SetActive(false);
            functionPanels[6].SetActive(false);
            functionPanels[10].SetActive(false);
            functionPanels[11].SetActive(false);
            functionPanels[12].SetActive(false);
            functionPanels[16].SetActive(false);
        }
        else if (df.stomics)
        {
            functionPanels[0].SetActive(false);
            functionPanels[1].SetActive(false);
            functionPanels[2].SetActive(false);
            functionPanels[3].SetActive(false);
            functionPanels[4].SetActive(false);
            functionPanels[5].SetActive(false);
            functionPanels[6].SetActive(false);
            functionPanels[10].SetActive(false);
            functionPanels[11].SetActive(false);
            functionPanels[16].SetActive(false);
        }
        else if (df.xenium)
        {
            functionPanels[0].SetActive(false);
            functionPanels[1].SetActive(false);
            functionPanels[2].SetActive(false);
            functionPanels[4].SetActive(false);
            functionPanels[10].SetActive(false);
            functionPanels[11].SetActive(false);
            functionPanels[16].SetActive(false);

        }
        else if (df.merfish)
        {
            functionPanels[0].SetActive(false);
            functionPanels[1].SetActive(false);
            functionPanels[2].SetActive(false);
            functionPanels[4].SetActive(false);
            functionPanels[10].SetActive(false);
            functionPanels[11].SetActive(false);
            functionPanels[16].SetActive(false);
        }
        else if (df.other)
        {
            // custom option
            functionPanels[0].SetActive(false);
            functionPanels[1].SetActive(false);
            functionPanels[2].SetActive(false);
            functionPanels[3].SetActive(false);
            functionPanels[4].SetActive(false);
            functionPanels[5].SetActive(false);
            functionPanels[6].SetActive(false);
            functionPanels[10].SetActive(false);
            functionPanels[11].SetActive(false);
            functionPanels[13].SetActive(false);
            functionPanels[16].SetActive(false);


        }

        if (df.objectUsed)
        {
            functionPanels[8].SetActive(false);
            functionPanels[14].SetActive(false);
        }


    }
}
