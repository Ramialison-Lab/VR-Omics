using System;
using System.IO;
using System.Linq;
using UnityEngine;

public class UMAPManager : MonoBehaviour
{
    public SpotDrawer sd;
    public DataTransferManager dfm;
    public float[] x_coordinates;
    public float[] y_coordinates;
    public float[] z_coordinates;    
    public float[] x_coordinates_spatial;
    public float[] y_coordinates_spatial;
    public float[] z_coordinates_spatial;    
    public float[] x_coordinates_umap;
    public float[] y_coordinates_umap;
    public float[] z_coordinates_umap;
    public float[] x_coordinates_tsne;
    public float[] y_coordinates_tsne;
    public float[] z_coordinates_tsne;
    public string[] location_names;
    public string[] dataset_names;

    private Vector2 middlePoint;
    bool move = false;

    bool firstUMAP = true;
    bool firstTSNE = true;

    /// <summary>
    /// Saving the spatial coordinates to swap between different modes
    /// </summary>
    /// <param name="x_coordinates"></param>
    /// <param name="y_coordinates"></param>
    /// <param name="z_coordinates"></param>
    /// <param name="location_names"></param>
    /// <param name="dataset_names"></param>
    public void SetCoordinatesForUMAP(
             float[] x_coordinates,
             float[] y_coordinates,
             float[] z_coordinates,
             string[] location_names,
             string[] dataset_names
        )     
    {
        this.x_coordinates_spatial = x_coordinates;
        this.y_coordinates_spatial = y_coordinates;
        this.z_coordinates_spatial = z_coordinates;
        this.location_names = location_names;
        this.dataset_names = dataset_names;
    }

    /// <summary>
    /// Switch to UMAP view
    /// </summary>
    public void SwitchUmap()
    {
        int x_position = -1;
        int y_position = -1;

        if (firstUMAP)
        {
            dfm = gameObject.GetComponent<DataTransferManager>();
            //TODO: adapt not only [0]
            string[] lines = File.ReadAllLines(dfm.obsmPath[0]);

            x_position = CSVHeaderInformation.CheckForColumnNumber("X_umap1", lines[0]);
            y_position = CSVHeaderInformation.CheckForColumnNumber("X_umap2", lines[0]);

            if (dfm.slideseqv2)
            {
                x_position = 0;
                y_position = 0;
            }

            lines = lines.Skip(1).ToArray();

            x_coordinates_umap = new float[lines.Length];
            y_coordinates_umap = new float[lines.Length];
            z_coordinates_umap = new float[lines.Length];


            for (int i = 0; i < lines.Length; i++)
            {
                string[] values = lines[i].Split(',');

                if (dfm.xenium)
                {
                    x_coordinates_umap[i] = float.Parse(values[x_position]) * 500;
                    y_coordinates_umap[i] = float.Parse(values[y_position]) * 500;
                    z_coordinates_umap[i] = 0;
                }
                else
                {
                    x_coordinates_umap[i] = float.Parse(values[x_position]) * 10;
                    y_coordinates_umap[i] = float.Parse(values[y_position]) * 10;
                    z_coordinates_umap[i] = 0;
                }
            }

            firstUMAP = false;
        }

        x_coordinates = x_coordinates_umap;
        y_coordinates = y_coordinates_umap;
        z_coordinates = z_coordinates_umap;

        StartSpotDrawer();
        if (dfm.visium)
        {
            move = true;
            middlePoint = CalculateMiddlePoint(x_coordinates_tsne, y_coordinates_tsne);
        }
    }

    /// <summary>
    /// Switch to T-sne view
    /// </summary>
    public void SwitchTSNE()
    {
        if (dfm.slideseqv2) return;
        if (firstTSNE)
        {
            dfm = gameObject.GetComponent<DataTransferManager>();
            //TODO: adapt not only [0]
            string[] lines = File.ReadAllLines(dfm.obsmPath[0]);
            string[] valuews = lines[0].Split(',');

            int x_position = CSVHeaderInformation.CheckForColumnNumber("X_tsne1", lines[0]);
            int y_position = CSVHeaderInformation.CheckForColumnNumber("X_tsne2", lines[0]);

            //TODO: use headerCheck
            lines = lines.Skip(1).ToArray();

            x_coordinates_tsne = new float[lines.Length];
            y_coordinates_tsne = new float[lines.Length];
            z_coordinates_tsne = new float[lines.Length];


            for (int i = 0; i < lines.Length; i++)
            {
                string[] values = lines[i].Split(',');
                if (dfm.xenium)
                {
                    x_coordinates_tsne[i] = float.Parse(values[x_position]) *100;
                    y_coordinates_tsne[i] = float.Parse(values[y_position]) *100;
                    z_coordinates_tsne[i] = 0;
                }
                else
                {
                    x_coordinates_tsne[i] = float.Parse(values[x_position]);
                    y_coordinates_tsne[i] = float.Parse(values[y_position]);
                    z_coordinates_tsne[i] = 0;
                }

            }
            firstTSNE = false;
        }

        x_coordinates = x_coordinates_tsne;
        y_coordinates = y_coordinates_tsne;
        z_coordinates = z_coordinates_tsne;

        StartSpotDrawer();

        if (dfm.visium)
        {
            move = true;
            middlePoint = CalculateMiddlePoint(x_coordinates_tsne, y_coordinates_tsne);
        }
    }

    /// <summary>
    /// Switch to default spatial view
    /// </summary>
    public void SwitchSpatial()
    {
        x_coordinates = x_coordinates_spatial;
        y_coordinates = y_coordinates_spatial;
        z_coordinates = z_coordinates_spatial;

        StartSpotDrawer();

        if (dfm.visium)
        {
            move = true;
            middlePoint = CalculateMiddlePoint(x_coordinates_tsne, y_coordinates_tsne);
        }
    }

    /// <summary>
    /// Pass selected view coordinates to SpotDrawer
    /// </summary>
    private void StartSpotDrawer()
    {
        //TODO: ensure to reset Cluster
        sd = gameObject.GetComponent<SpotDrawer>();
        sd.StartDrawer(x_coordinates, y_coordinates, z_coordinates, location_names, dataset_names);
    }

    /// <summary>
    /// Calculate Vec2 middlepoint
    /// </summary>
    /// <param name="xCoordinates"></param>
    /// <param name="yCoordinates"></param>
    /// <returns></returns>
    private Vector2 CalculateMiddlePoint(float[] xCoordinates, float[] yCoordinates)
    {
        if (xCoordinates.Length != yCoordinates.Length || xCoordinates.Length == 0)
        {
            throw new ArgumentException("The input arrays should have the same non-zero length.");
        }

        // Calculate the average of x and y coordinates
        float xSum = xCoordinates.Sum();
        float ySum = yCoordinates.Sum();
        float xAverage = xSum / xCoordinates.Length;
        float yAverage = ySum / yCoordinates.Length;

        // Create a Vector2 using the calculated averages
        Vector2 middlePoint = new Vector2(xAverage, yAverage);
        return middlePoint;
    }

    private void LateUpdate()
    {
        if (move)
        {
            Camera.main.transform.position = new Vector3(middlePoint.x, middlePoint.y, Camera.main.transform.position.z);
            move = false;
        }
    }
}
