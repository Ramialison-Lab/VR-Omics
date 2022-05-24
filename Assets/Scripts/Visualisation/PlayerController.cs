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
    private bool qKey;
    private bool eKey;
    public GameObject menuCanvas;
    public float speed = 1.5f;
    public void Update()
    {
        if (!GameObject.Find("ScriptHolder").GetComponent<AutoCompleteManager>().InputFocused())
        {
            //KEYINPUT
            Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal") * Time.deltaTime * speed, 0, Input.GetAxisRaw("Vertical") * Time.deltaTime * speed);
            move = this.transform.TransformDirection(move);
            _controller.Move(move * _speed);

            //TBD disable movement if inputfie;d selected
            if (Input.GetKeyDown(KeyCode.Space)) up = true;
            if (Input.GetKeyUp(KeyCode.Space)) up = false;
            if (Input.GetKeyDown(KeyCode.Z)) down = true;
            if (Input.GetKeyUp(KeyCode.Z)) down = false;

            // TBD set key q or e in cube.dragobject.cs and based on datasetname find the cube to rotate
            if (Input.GetKeyDown(KeyCode.Q)) qKey = true;
            if (Input.GetKeyUp(KeyCode.Q)) qKey = false;
        }

        if (qKey && !menuCanvas.GetComponent<MenuCanvas>().locked)
        {
            GameObject.Find("ScriptHolder").GetComponent<SliceCollider>().prepareRotation(1);
        }
        if (eKey && !menuCanvas.GetComponent<MenuCanvas>().locked)
        {
            GameObject.Find("ScriptHolder").GetComponent<SliceCollider>().prepareRotation(0);
        }

        if (Input.GetKeyDown(KeyCode.E)) eKey = true;
        if (Input.GetKeyUp(KeyCode.E)) eKey = false;

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
