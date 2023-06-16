/*
* Copyright (c) 2023 Murdoch Children's Research Institute, Parkville, Melbourne
* author: Denis Bienroth
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"),
* to deal in the Software without restriction, including without limitation
* the rights to use, copy, modify, merge, publish, distribute, sublicense,
* and/or sell copies of the Software, and to permit persons to whom the Software
* is furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in all
* copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
* INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
* PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
* HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
* CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE
* OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    //Access Variables
    AutoCompleteManager acm;
    DataTransferManager dfm;
    SliceCollider sc;
    public CharacterController _controller;

    //Bools
    private bool moveForward = false;
    private bool moveBackward = false;
    private bool up = false;
    private bool down = false;

    //Movement variables
    [SerializeField] private float _speed = 10;
    [SerializeField] private float speed = 5;
    [SerializeField] private float moveSpeedCamera = 5f;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float zoomSpeed = 10.0f;
    [SerializeField] private float sensitivity = 2.0f;
    private float xRotation = 0.0f;
    private float yRotation = 0.0f;


    private void Start()
    {
        dfm = GameObject.Find("ScriptHolder").GetComponent<DataTransferManager>();
        sc = GameObject.Find("ScriptHolder").GetComponent<SliceCollider>();
        acm = GameObject.Find("ScriptHolder").GetComponent<AutoCompleteManager>();
    }

    public void Update()
    {
        if (acm.InputFocused())
        {
            return;
        }

        // Movement
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        Vector3 move = new Vector3(horizontalInput, verticalInput, 0) * Time.deltaTime * speed;
        move = transform.TransformDirection(move);
        _controller.Move(move * _speed);

        // Zoom
        float zoomInput = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(zoomInput) > 0.0f)
        {
            Camera.main.transform.position += Camera.main.transform.forward * zoomInput * zoomSpeed;
        }

        // Rotation
        if (Input.GetKey(KeyCode.Q))
        {
            sc.prepareRotation(1);
        }
        if (Input.GetKey(KeyCode.E))
        {
            sc.prepareRotation(-1);
        }
        if (Input.GetMouseButton(1))
        {
            rotateCamera();
            float rotateAmount = Input.GetAxis("Mouse X") * moveSpeed;
            Camera.main.transform.Rotate(0, -rotateAmount * 10, 0);
        }

        // Up/Down movement
        if (up)
        {
            float upSpeed = dfm.tomoseq ? 100 : 0;
            transform.position += Vector3.up * Time.deltaTime * speed * upSpeed;
        }
        if (down)
        {
            float downSpeed = dfm.tomoseq ? -100 : 0;
            transform.position += Vector3.up * Time.deltaTime * speed * downSpeed;
        }

        // Move forward/backward
        if (moveForward)
        {
            transform.position += transform.up * moveSpeedCamera * Time.deltaTime;
        }
        else if (moveBackward)
        {
            transform.position -= transform.up * moveSpeedCamera * Time.deltaTime;
        }
    }

    private void rotateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        xRotation += mouseY;
        yRotation += mouseX;

        xRotation = Mathf.Clamp(xRotation, -90.0f, 90.0f);

        //transform.rotation = Quaternion.Euler(xRotation, yRotation, 0.0f);
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
    }
}