using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;

public class ConcatManager : MonoBehaviour
{

    public TMP_InputField[] concatParams;
    public Toggle svgAnalysis;
    public Toggle tsnetoggle;

    List<RawImage> imagesList = new List<RawImage>();
    bool dragAndDrop = false;
    private RawImage clickedRawImage;
    private bool isDragging = false;
    private float rotationSpeed = 5.0f; // Adjust this value to control the rotation speed
    private int[] rotationValues;

    public void SetImageList(List<RawImage> images)
    {
        imagesList = images;
        dragAndDrop = true;
        rotationValues = new int[images.Count];
        for(int i=0; i < rotationValues.Length; i++)
        {
            rotationValues[i] = 0;
        }

    }

    bool once = true;
    private void Update()
    {

        if (Input.GetMouseButtonDown(0) && !isDragging)
        {
            // Get the mouse click position in screen coordinates
            Vector2 clickPosition = Input.mousePosition;

            // Convert the screen coordinates to RectTransform coordinates
            foreach (RawImage rawImage in imagesList)
            {
                RectTransform rawImageRect = rawImage.rectTransform;

                // Check if the click position is within the RectTransform's bounds
                if (RectTransformUtility.RectangleContainsScreenPoint(rawImageRect, clickPosition))
                {
                    // Store the RawImage that was clicked
                    clickedRawImage = rawImage;
                    isDragging = true; // Start dragging
                    break; // Exit the loop once we find a clicked RawImage
                }
            }
        }

        if (isDragging)
        {
            // Get the mouse position in world coordinates
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 0f; // Set the Z coordinate to 0 (assuming 2D space)

            // Move the clicked RawImage to follow the mouse position
            if (clickedRawImage != null)
            {
                clickedRawImage.rectTransform.position = mousePosition;
            }

            // If the clicked image overlaps with any other image
            foreach (RawImage rawImage in imagesList)
            {

                if(rawImage != clickedRawImage)
                {
                    if (RectOverlap(rawImage.rectTransform, clickedRawImage.rectTransform) && rawImage != clickedRawImage)
                    {
                        clickedRawImage.transform.localPosition = CalculateSnapPosition(rawImage.rectTransform, clickedRawImage.rectTransform);

                    }
                }
            }


                if (Input.GetMouseButtonDown(1))
            {
                clickedRawImage.rectTransform.Rotate(Vector3.forward, -90);
                int index = imagesList.IndexOf(clickedRawImage);
                rotationValues[index] = rotationValues[index] - 90;
                if(rotationValues[index] <= -360)
                {
                    rotationValues[index] = 0;
                }           
              
            }
        }

        // Stop dragging when the left mouse button is released
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            clickedRawImage = null;

        }
    }

    private bool RectOverlap(RectTransform firstRect, RectTransform secondRect) // second is clicked image
    {
        
        float first_x_min = firstRect.localPosition.x - firstRect.sizeDelta.x / 2; 
        float first_x_max = firstRect.localPosition.x + firstRect.sizeDelta.x / 2;         
        
        float second_x_min = secondRect.localPosition.x - secondRect.sizeDelta.x / 2; 
        float second_x_max = secondRect.localPosition.x + secondRect.sizeDelta.x / 2;

        float first_y_min = firstRect.localPosition.y - firstRect.sizeDelta.y / 2;
        float first_y_max = firstRect.localPosition.y + firstRect.sizeDelta.y / 2;

        float second_y_max = secondRect.localPosition.y + secondRect.sizeDelta.y / 2;
        float second_y_min = secondRect.localPosition.y - secondRect.sizeDelta.y / 2;


        if((first_x_min <= second_x_min && second_x_min <= first_x_max))
        {
            if (first_y_min <= second_y_min && second_y_min <= first_y_max)
            {
                return true;
            }
            else if (first_y_min <= second_y_max && second_y_max <= first_y_max)
            {
                return true;
            }
        }
        else if((first_x_min <= second_x_max && second_x_max <= first_x_max))
        {
            if (first_y_min <= second_y_min && second_y_min <= first_y_max)
            {
                return true;
            }
            else if (first_y_min <= second_y_max && second_y_max <= first_y_max)
            {
                return true;
            }
        }

        return false;

    }

    private Vector3 CalculateSnapPosition(RectTransform firstRect, RectTransform secondRect) //second is clicked
    {
        Vector3 snapVector = secondRect.localPosition;

        float first_x_min = firstRect.localPosition.x - firstRect.sizeDelta.x / 2;
        float first_x_max = firstRect.localPosition.x + firstRect.sizeDelta.x / 2;

        float second_x_min = secondRect.localPosition.x - secondRect.sizeDelta.x / 2;
        float second_x_max = secondRect.localPosition.x + secondRect.sizeDelta.x / 2;

        float first_y_min = firstRect.localPosition.y - firstRect.sizeDelta.y / 2;
        float first_y_max = firstRect.localPosition.y + firstRect.sizeDelta.y / 2;

        float second_y_max = secondRect.localPosition.y + secondRect.sizeDelta.y / 2;
        float second_y_min = secondRect.localPosition.y - secondRect.sizeDelta.y / 2;


        if ((first_x_min <= second_x_min && second_x_min <= first_x_max))
        {
            if (first_y_min <= second_y_min && second_y_min <= first_y_max)
            {
                if (Math.Abs(second_x_min - first_x_max) > Math.Abs(first_y_max - second_y_min))
                {
                    snapVector = new Vector3(secondRect.localPosition.x, secondRect.localPosition.y + Math.Abs(first_y_max - second_y_min), 0);
                }
                else
                {
                    snapVector = new Vector3(secondRect.localPosition.x + Math.Abs(second_x_min - first_x_max), secondRect.localPosition.y, 0);
                }
                return snapVector;
            }
            else if (first_y_min <= second_y_max && second_y_max <= first_y_max)
            {
                if (Math.Abs(second_x_min - first_x_max) > Math.Abs(first_y_min - second_y_max))
                {
                    snapVector = new Vector3(secondRect.localPosition.x, secondRect.localPosition.y - Math.Abs(first_y_min - second_y_max), 0);
                }
                else
                {
                    snapVector = new Vector3(secondRect.localPosition.x + Math.Abs(second_x_min - first_x_max), secondRect.localPosition.y, 0);
                }
                return snapVector;
            }
        }
        else if ((first_x_min <= second_x_max && second_x_max <= first_x_max))
        {
            if (first_y_min <= second_y_min && second_y_min <= first_y_max)
            {
                if (Math.Abs(second_x_max - first_x_min) > Math.Abs(first_y_max - second_y_min))
                {
                    snapVector = new Vector3(secondRect.localPosition.x, secondRect.localPosition.y + Math.Abs(first_y_max - second_y_min), 0);
                }
                else
                {
                    snapVector = new Vector3(secondRect.localPosition.x - Math.Abs(second_x_max - first_x_min), secondRect.localPosition.y, 0);
                }
                return snapVector;
            }
            else if (first_y_min <= second_y_max && second_y_max <= first_y_max)
            {
                if (Math.Abs(second_x_max - first_x_min) > Math.Abs(second_y_max - first_y_min))
                {
                    snapVector = new Vector3(secondRect.localPosition.x, secondRect.localPosition.y - Math.Abs(second_y_max - first_y_min), 0);
                }
                else
                {
                    snapVector = new Vector3(secondRect.localPosition.x - Math.Abs(second_x_max - first_x_min), secondRect.localPosition.y, 0);
                }
                return snapVector;
            }
        }

        return snapVector;
    }
    /// <summary>
    /// Getting placement of slides and write output file to concat the datasets
    /// </summary>
    public void StartConcatProcess()
    {
        var datapaths = this.gameObject.GetComponent<UIManager>().transferDatapaths;
        Vector2[] imageCoordinates = new Vector2[imagesList.Count];
        //Dimensions store (width, height)
        Vector2[] imageDimensions = new Vector2[imagesList.Count];

        //Rotation to be updated
        string concat_directory = "";

#if UNITY_EDITOR
        concat_directory = this.gameObject.GetComponent<UIManager>().current_directory + "/PythonFiles/Concat_Visium.txt";
#else
        concat_directory = this.gameObject.GetComponent<UIManager>().current_directory + "Assets/PythonFiles/Concat_Visium.txt";

#endif

        if (!File.Exists(concat_directory))
        {
            // Create the file if it doesn't exist
            using (StreamWriter write = File.CreateText(concat_directory))
            {
            }
        }

        string concat_used_directory = "";
#if UNITY_EDITOR
        concat_used_directory = this.gameObject.GetComponent<UIManager>().current_directory + "/PythonFiles/Concat_used_Visium.txt";
#else
        concat_used_directory = this.gameObject.GetComponent<UIManager>().current_directory + "Assets/PythonFiles/Concat_used_Visium.txt";

#endif
        ;

        using (StreamWriter write = File.CreateText(concat_used_directory))
        {
            write.WriteLine("true");
            write.Close();
        }
        

        StreamWriter writer = new StreamWriter(concat_directory, false);
        writer.WriteLine("Datapath, Center, Width, Height, Rotation");

        //get coordinates for each RawImage
        for (int i = 0; i < imagesList.Count; i++)
        {
            RawImage image = imagesList[i];

            imageCoordinates[i] = image.rectTransform.position;
            imageDimensions[i] = image.rectTransform.sizeDelta;

            string line = image.transform.ToString() + ", " + image.rectTransform.position + ", " + image.rectTransform.sizeDelta.x + ", " + image.rectTransform.sizeDelta.y + ", " + rotationValues[i].ToString();

            writer.WriteLine(line);

        }

            writer.Close();

            string longAnalysis = "0";
            string tsne_umap = "0";

        if (svgAnalysis.isOn) longAnalysis = "1";
        if (tsnetoggle.isOn) tsne_umap = "1";
        string[] concat_path_out = new string[8];
        string current_directory = gameObject.GetComponent<UIManager>().current_directory;
        writer = new StreamWriter(current_directory + "/Assets/PythonFiles/Visium_concat_param.txt", false);

        concat_path_out[0] = concatParams[0].text; // Min count
        concat_path_out[1] = concatParams[1].text; // Max count
        concat_path_out[2] = concatParams[2].text; // MT count min
        concat_path_out[3] = concatParams[3].text; // MT count max
        concat_path_out[4] = concatParams[4].text; // Cell min
        concat_path_out[5] = concatParams[5].text; // Cell max
        concat_path_out[7] = longAnalysis; // SVG analysis toggle 
        concat_path_out[8] = tsne_umap; // T-SNE toggle 

        foreach (string param in concat_path_out)
        {
            writer.WriteLine(param);
        }
        writer.Close();

        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = current_directory + "Assets/Scripts/Python_exe/exe_scanpy/dist/Visium_pipeline.exe";
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = false;
        UnityEngine.Debug.Log("Visium Concat File load started.");


        Process p = new Process
        {
            StartInfo = startInfo
        };

        p.Start();
        p.WaitForExit();

    }
}

