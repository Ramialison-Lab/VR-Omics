using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MacOSLimitationManager : MonoBehaviour
{
    [Header("GameObjects to Disable MAC")]
    public GameObject[] gameObjectsToDisable;

    void Start()
    {
        // Check if running on MacOS
        if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
        {
            DisableElements();
        }
    }

    void DisableElements()
    {
        // Disable all specified GameObjects
        foreach (GameObject obj in gameObjectsToDisable)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }

        Debug.Log("Mac detected: Features disabled.");
    }
}
