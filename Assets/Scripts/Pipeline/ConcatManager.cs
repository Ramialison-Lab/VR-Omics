using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ConcatManager : MonoBehaviour
{
    List<RawImage> imagesList = new List<RawImage>();
    bool dragAndDrop = false;
    private RawImage clickedRawImage;
    private bool isDragging = false;
    private float rotationSpeed = 5.0f; // Adjust this value to control the rotation speed

    public void SetImageList(List<RawImage> images)
    {
        imagesList = images;
        dragAndDrop = true;
    }

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

            // Rotate the RawImage based on mouse wheel input
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            clickedRawImage.rectTransform.Rotate(Vector3.forward, scroll * rotationSpeed);
        }

        // Stop dragging when the left mouse button is released
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            clickedRawImage = null;
        }
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
        int rotation = 0;
        string concat_directory = this.gameObject.GetComponent<UIManager>().current_directory + "/PythonFiles/Concat_Visium.txt";

        if (!File.Exists(concat_directory))
        {
            // Create the file if it doesn't exist
            using (StreamWriter write = File.CreateText(concat_directory))
            {
            }
        }

        string concat_used_directory = this.gameObject.GetComponent<UIManager>().current_directory + "/PythonFiles/Concat_used_Visium.txt";

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

            string line = image.transform.ToString() + ", " + image.rectTransform.position + ", " + image.rectTransform.sizeDelta.x + ", " + image.rectTransform.sizeDelta.y + ", " + rotation.ToString();

            writer.WriteLine(line);

        }

            writer.Close();
    }

}

