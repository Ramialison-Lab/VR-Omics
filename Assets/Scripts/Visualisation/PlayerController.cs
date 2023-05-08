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
    public CharacterController _controller;
    public float _speed = 10;
    public float _rotationSpeed = 180;
    public float horizontalSpeed = 2.0F;
    public float verticalSpeed = 2.0F;
    private Vector3 rotation;
    private bool up = false;
    private bool down = false;
    public GameObject IF;
    public GameObject menuCanvas;
    public float speed = 5;
    Vector2 mousepos;
    DataTransferManager dfm;
    SliceCollider sc;

    public float moveSpeed = 1f;

    private bool isMoving = false;
    private Vector3 lastPosition;
    public float zoomSpeed = 1.0f;
    public int  scaleFactor = 10;
    private void LateUpdate()
    {
        mousepos = Input.mousePosition;
    }

    private void Start()
    {
        dfm = GameObject.Find("ScriptHolder").GetComponent<DataTransferManager>();
        sc = GameObject.Find("ScriptHolder").GetComponent<SliceCollider>();
    }

    public void Update()
    {
        if (!GameObject.Find("ScriptHolder").GetComponent<AutoCompleteManager>().InputFocused())
        {
            //KEYINPUT
            Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal") * Time.deltaTime * speed * (1), Input.GetAxisRaw("Vertical") * Time.deltaTime * speed *(1), 0);
            move = this.transform.TransformDirection(move);
            _controller.Move(move * _speed);

            float zoomInput = Input.GetAxis("Mouse ScrollWheel"); // Get the zoom input from the mouse scroll wheel

            if (zoomInput != 0.0f) // Check if there is any zoom input
            {
                Camera.main.fieldOfView -= zoomInput * zoomSpeed; // Adjust the camera's field of view based on the zoom input
                //Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, 0f, 60.0f); // Clamp the camera's field of view to a minimum of 10 and a maximum of 60 degrees
            }

            if (Input.GetKey(KeyCode.Q)) sc.prepareRotation(1);
            if(Input.GetKey(KeyCode.E)) sc.prepareRotation(-1);


            if (Input.GetMouseButtonDown(2))
            {
                isMoving = true;
                lastPosition = Input.mousePosition;

            }
            if (Input.GetMouseButtonUp(2)) // Check if right mouse button is released
            {
                isMoving = false;
            }
            if (isMoving)
            {
                Vector3 delta = Input.mousePosition - lastPosition;
                transform.Translate(-delta.x * moveSpeed, -delta.y * moveSpeed, 0);
                lastPosition = Input.mousePosition;
            }

            if (Input.GetMouseButton(1)) // Check if middle mouse button is pressed
            {
                float rotateAmount = Input.GetAxis("Mouse X") * moveSpeed;
                Camera.main.transform.Rotate(0, -rotateAmount*10,0);
            }

            if (up)
            {
                if(dfm.tomoseq) transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 100 * Time.deltaTime * speed);
                else transform.position = new Vector3(transform.position.x, transform.position.y + 100 * Time.deltaTime * speed, transform.position.z);

            }
            if (down)
            {
                if (dfm.tomoseq) transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 100 * Time.deltaTime * speed);
                else transform.position = new Vector3(transform.position.x, transform.position.y - 100 * Time.deltaTime * speed, transform.position.z);

            }
        }


    }
}