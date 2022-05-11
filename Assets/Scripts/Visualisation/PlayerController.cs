using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{ 
    public CharacterController _controller;
    public float _speed = 10;
    public float _rotationSpeed = 180;
    public float horizontalSpeed = 2.0F;
    public float verticalSpeed = 2.0F;
    private Vector3 rotation;
    private bool up = false;
    private bool down = false;
    public GameObject IF;

    public void Update()
    {

        Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal") * Time.deltaTime, 0, Input.GetAxisRaw("Vertical") * Time.deltaTime);
        move = this.transform.TransformDirection(move);
        _controller.Move(move * _speed);
        
        //TBD disable movement if inputfie;d selected

        if (Input.GetKeyDown(KeyCode.Space)) up = true;
        if (Input.GetKeyUp(KeyCode.Space)) up = false;
        if (Input.GetKeyDown(KeyCode.Z)) down = true;
        if (Input.GetKeyUp(KeyCode.Z)) down = false;
        

        if (up)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);

        }       
        if (down)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);

        }
    }

}
