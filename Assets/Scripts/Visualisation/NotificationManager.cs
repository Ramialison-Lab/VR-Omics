using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{

    public TMP_Text textComponent;
    private GameObject panel;

    private void Start()
    {
        // Assuming you have a reference to the TMP_Text component and the panel object
        // Set them in the Inspector of the attached script.
        // If not, you can also find them dynamically in Start or Awake.
        if (textComponent == null)
            textComponent = GetComponentInChildren<TMP_Text>();
        if (panel == null)
            panel = this.gameObject;

        // Initially, hide the panel.
        HideNotificationBanner();
    }

    /// <summary>
    /// Permanently shows notification banner until it's closed or the HideNotificationBanner function is called.
    /// </summary>
    /// <param name="text"></param>
    public void ShowNotificationBanner(string text)
    {
        // Show the panel and set the text.
        textComponent.text = text;
        panel.SetActive(true);
    }

    /// <summary>
    /// Hide the active notification banner. 
    /// </summary>
    public void HideNotificationBanner()
    {
        // Hide the panel and reset the text.
        textComponent.text = "";
        panel.SetActive(false);
    }

    /// <summary>
    /// Activates the notification banner for the entered amount of time
    /// </summary>
    /// <param name="text"></param>
    /// <param name="duration"></param>
    public void ShowNotificationBannerForDuration(string text, float duration)
    {
        // Show the panel with the specified text.
        ShowNotificationBanner(text);

        // Start a coroutine to hide the panel after the specified duration.
        StartCoroutine(HidePanelAfterDelay(duration));
    }

    private IEnumerator HidePanelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        // Hide the panel after the specified duration.
        HideNotificationBanner();
    }
}




