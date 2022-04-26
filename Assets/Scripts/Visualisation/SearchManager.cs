using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchManager : MonoBehaviour
{
    public string gene = "";
    public List<string> datasetPaths;
    public List<string> geneNameList;
    public List<string> ensembleIds;
    private GameObject sh;
    public string ensembleId;
    public string datasetFound;
    private FileReader fr;

    public List<string> resultDataset;
    public List<string> resultSpotname;
    public List<float> resultExpression;
    public float[] resultExpressionTemp;
    
    private void Start()
    {
        sh = GameObject.Find("ScriptHolder");
        fr = sh.GetComponent<FileReader>();
        datasetPaths = sh.GetComponent<DataTransferManager>().getDatasetpaths();        
        searchEnsembleId(gene);

        readExpressionList(gene);
    }


    private void readExpressionList(string geneName)
    {
        int posInGeneList;
        foreach (string p in datasetPaths)
        {
            fr.readGeneNames(p);
            List<string> tempGeneNames = fr.getGeneNameList();
            if (tempGeneNames.Contains(geneName))
            {

                posInGeneList = tempGeneNames.IndexOf(geneName);

                // get exp values
                fr.readGeneExpressionValues(p);

                resultExpressionTemp = fr.getExpressionValues();

                //get indices list

                fr.readIndices(p);
                List<int> indicesTemps = new List<int>(fr.getIndices());

                fr.readIndPtr(p);
                List<int> indPtrs = new List<int>(fr.getIndptr());


              //  StartCoroutine(calculateExpresssionValues(indicesTemps,indPtrs,resultExpressionTemp,posInGeneList));


              //  Debug.Log(resultExpression.Count);



                //check if indices contains value for posInGeneList
                // if yes → 1st value for spot[0]
                //if not val =0
                //posinlist plus 1st entry of indptr

                //check if indices contains vlaue for posinGenelist+indprt
                //yes → 2nd value; no → 2nd value 0
                //posinGenelist + next indprt until indPtr end

            }
            else
            {
                // If gene not in list set all spots gene vlaues to 0
            }

            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! Clear all lists and arrays at the end
        }
    }

    public void searchEnsembleId(string geneName)
    {       
        foreach(string p in datasetPaths)
        {
            fr.readGeneNames(p);
            geneNameList = fr.getGeneNameList();
            if (geneNameList.Contains(geneName))
            {
                fr.readEnsembleIds(p);
                ensembleIds = fr.getEnsembleIds();
                ensembleId = ensembleIds[geneNameList.IndexOf(geneName)];                
                geneNameList.Clear();
                fr.clearGeneNames();
                ensembleIds.Clear();
                break;
            }
            geneNameList.Clear();
            fr.clearGeneNames();
        }
    }

    IEnumerator calculateExpresssionValues(List<int> indicesTemps, List<int> indPtrs, float[] resultExpressionTemp, int posInGeneList)
    {

        for (int x = 0; x < indPtrs.Count; x++)
        {
            if (indicesTemps.Contains(posInGeneList))
            {
                resultExpression.Add(resultExpressionTemp[indicesTemps.IndexOf(posInGeneList)]);
            }
            else
            {
                resultExpression.Add(0);
            }

            posInGeneList = posInGeneList + indPtrs[x];

        }

        yield return null;
    }
}
