using System.Collections.Generic;
using UnityEngine;

public class FilePathEndings : MonoBehaviour
{
    // Create a dictionary to store file names for different technologies
    public Dictionary<string, TechnologyFiles> technologyFileNames = new Dictionary<string, TechnologyFiles>();

    [System.Serializable]
    public class TechnologyFiles
    {
        public string locationMetadataCSV = "";
        public string geneCountCSV = "";
        public string h5 = "";
        public string tissuePositionListCSV = "";
        public string scalefactorsJSON = "";
        public string highresTissueImagePNG = "";
        public string obsmCSV= "";
        public string genePanelCSV= "";
        public string resultCSV = "";
        public string geneInformationCSV = "";

    }

    private DataTransferManager dfm;

    private void Awake()
    {

        technologyFileNames["Visium"] = new TechnologyFiles
        {
            locationMetadataCSV = "*metadata.csv",
            geneCountCSV = "*transposed.csv",
            h5 = "*.h5",
            tissuePositionListCSV = "tissue_positions_list.csv",
            scalefactorsJSON = "scalefactors_json.json",
            highresTissueImagePNG = "tissue_hires_image.png",
            obsmCSV = "obsm.csv",
            genePanelCSV = "gene_panel.csv",
            resultCSV = "",
        };

        technologyFileNames["Xenium"] = new TechnologyFiles
        {
            locationMetadataCSV = "processed_cells.csv",
            geneCountCSV = "gene_transposed_counts.csv",
            obsmCSV = "obsm.csv",
            genePanelCSV = "feature_matrix.csv",
            resultCSV = "results.csv",
        };

        technologyFileNames["Merfish"] = new TechnologyFiles
        {
            locationMetadataCSV = "metadata_processed.csv",
            geneCountCSV = "gene_transposed_processed.csv",
            obsmCSV = "obsm.csv",
            genePanelCSV = "feature_matrix.csv",
            resultCSV = "results.csv",
        };

        technologyFileNames["Nanostring"] = new TechnologyFiles
        {
            locationMetadataCSV = "meta_data.csv",
            geneCountCSV = "gene_transposed_counts.csv",
            obsmCSV = "obsm.csv",
            genePanelCSV = "panel.csv",
            resultCSV = "*results.csv",
            geneInformationCSV = "gene_information.csv",
        };

        technologyFileNames["SlideSeqV2"] = new TechnologyFiles
        {
            locationMetadataCSV = "metadata.csv",
            geneCountCSV = "gene_transposed_counts.csv",
            obsmCSV = "umap.csv",
            genePanelCSV = "genepanel.csv",
        };

    }

}
