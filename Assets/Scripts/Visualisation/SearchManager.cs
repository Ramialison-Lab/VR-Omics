using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SearchManager : MonoBehaviour
{
    public string gene = "";
    public List<string> datasetPaths;
    public List<string> geneNameList;
    public List<string> geneNames;
    public List<string> ensembleIds;
    private GameObject sh;
    public string ensembleId;
    public string datasetFound;
    private FileReader fr;

    public List<string> resultDataset;
    public List<string> resultSpotname;
    public List<float> resultExpression;
    public float[] resultExpressionTemp;

    public bool visium = true;
    public bool tomoseq = false;

    private void Start()
    {
        sh = GameObject.Find("ScriptHolder");

        if (gameObject.GetComponent<DataTransferManager>().VisiumActive())
        {
            fr = sh.GetComponent<FileReader>();
            datasetPaths = sh.GetComponent<DataTransferManager>().getDatasetpaths();
            searchEnsembleId(gene);
            foreach (string p in datasetPaths)
            {
                fr.readGeneNames(p);
                geneNames.AddRange(fr.getGeneNameList());
                geneNames = geneNames.Distinct().ToList();
            }

            sh.GetComponent<AutoCompleteManager>().setGeneNameList(geneNames);
        }

        else if (gameObject.GetComponent<DataTransferManager>().TomoseqActive())
        {
            geneNames.AddRange(gameObject.GetComponent<TomoSeqDrawer>().getGeneNames());
            //geneNames = geneNames.Distinct().ToList();
            sh.GetComponent<AutoCompleteManager>().setGeneNameList(geneNames);
        }

        else if (gameObject.GetComponent<DataTransferManager>().StomicsActive())
        {
            geneNames.AddRange(gameObject.GetComponent<DataTransferManager>().getStomicsGeneNames());
            //geneNames = geneNames.Distinct().ToList();
            sh.GetComponent<AutoCompleteManager>().setGeneNameList(geneNames);
        }

    }

    public void readStomicsExpression(string geneName)
    {


        var Xdata= gameObject.GetComponent<FileReader>().readH5Float("C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Stomics\\new.h5ad", "X/data");
        var indices = gameObject.GetComponent<FileReader>().readH5Float("C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Stomics\\new.h5ad", "X/indices");
        var indptr = gameObject.GetComponent<FileReader>().readH5Float("C:\\Users\\Denis.Bienroth\\Desktop\\ST_technologies\\Stomics\\new.h5ad", "X/indptr");

        Debug.Log(Xdata.Count);
        Debug.Log(indices.Count);
        Debug.Log(indptr.Count);

        //Sparse matrix input from transposed HDF5 file 

        //GameObject.Find("ScriptHolder").GetComponent<SpotDrawer>().setColors(normalised);

    }


    // checking the datasets of all slides for the position of the gene 
    public void readExpressionList(string geneName)
    {
        int x = 0;
        //for each dataset selected
        foreach (string p in datasetPaths)
        {
                sh.GetComponent<CSVReader>().searchForGene(p, geneName, x);
                x++; 
        }
    }

    public void searchEnsembleId(string geneName)
    {
        foreach (string p in datasetPaths)
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
