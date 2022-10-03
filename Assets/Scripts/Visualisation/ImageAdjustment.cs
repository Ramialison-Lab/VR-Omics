using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageAdjustment : MonoBehaviour
{

    private void Update()
    {
        if (Input.GetKey(KeyCode.K))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                transform.localScale += new Vector3(0, 0, 0.025f);
            }
            else
            {
                transform.Translate(Vector3.forward *10* Time.deltaTime);
            }
        }
        if (Input.GetKey(KeyCode.I))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                transform.localScale -= new Vector3(0, 0, 0.025f);
            }
            else
            {
                transform.Translate(Vector3.up * 10 * Time.deltaTime, Space.World);
            }
        }
        if (Input.GetKey(KeyCode.J))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                transform.localScale -= new Vector3(0.025f, 0, 0);
            }
            else
            {
                transform.Translate(Vector3.left * 10 * Time.deltaTime, Camera.main.transform);
            }
        }
        if (Input.GetKey(KeyCode.L))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                transform.localScale += new Vector3(0.025f, 0, 0);
            }
            else
            {
                transform.Translate(Vector3.right * 10 * Time.deltaTime, Camera.main.transform);
            }
        }


    }

}

