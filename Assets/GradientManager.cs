using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GradientManager : MonoBehaviour
{
    [SerializeField]
    Gradient gradient;
    [SerializeField]
    float duration;
    float t = 0f;

    void Start()
    {
        var gck = new GradientColorKey[5];
        var alphakeys = new GradientAlphaKey[2];
        float rgb = 255;

        gck[0].color = new Color(65 / rgb, 105 / rgb, 255 / rgb); // Blue
        gck[0].time = 0f;
        gck[1].color = new Color(135 / rgb, 206 / rgb, 250 / rgb); // Cyan
        gck[1].time = .25f;
        gck[2].color = new Color(60 / rgb, 179 / rgb, 113 / rgb); // green
        gck[2].time = 0.50F;
        gck[3].color = new Color(255 / rgb, 230 / rgb, 0); // yellow
        gck[3].time = 0.75F;
        gck[4].color = new Color(180 / rgb, 0, 0); // Red
        gck[4].time = 1f;

        alphakeys[0].alpha = 1f;
        alphakeys[0].time = 0f;
        alphakeys[1].alpha = 1f;
        alphakeys[1].time = 1f;

        gradient.SetKeys(gck, alphakeys);
    }

    void Update()
    {
        float value = Mathf.Lerp(0f, 1f, t);
        t += Time.deltaTime / duration;
        Color color = gradient.Evaluate(value);
        this.gameObject.GetComponent<Renderer>().material.color = color;
    }
}
