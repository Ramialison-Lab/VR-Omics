using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceRotation : MonoBehaviour
{
    private void Update()
    {
        this.gameObject.transform.Rotate(0, 0, 0);

    }
    private void LateUpdate()
    {
        this.gameObject.transform.Rotate(0, 0, 0);
    }
}
