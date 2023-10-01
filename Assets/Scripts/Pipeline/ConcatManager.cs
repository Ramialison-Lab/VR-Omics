using System.Collections;
using System.Collections.Generic;
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

        if (Input.GetMouseButtonUp(0))
        {
            // Stop dragging when the left mouse button is released
            isDragging = false;
            clickedRawImage = null;
        }
    }


    public void StartConcatProcess()
    {

    }
}

