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

    }

    public void showGeneExpression()
    {
        readExpressionList(gene);

    }

    private void readExpressionList(string geneName)
    {
        int posInGeneList;
        //for each dataset selected
        foreach (string p in datasetPaths) 
        {
            // create list of all genes in dataset
            fr.readGeneNames(p);
            List<string> tempGeneNames = fr.getGeneNameList();
            // iterate through whole list to find gene
            if (tempGeneNames.Contains(geneName))
            {
                //get position of gene
                posInGeneList = tempGeneNames.IndexOf(geneName);

                sh.GetComponent<CSVReader>().searchGene(p, posInGeneList, geneName);
            }

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
