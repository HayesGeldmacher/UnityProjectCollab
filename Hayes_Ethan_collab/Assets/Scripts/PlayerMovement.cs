using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // public stuff
    [Header("Movement")]
    public float ForwardSpeed = 12;
    public float SidewaysSpeed = 9;

    [Header("Physics")]
    public float Gravity = 9.81f;

    // private stuff
    private Rigidbody rb;
    private float hInput, vInput;
    
    void Start()
    {
        // !!! NEEDS A RIGIDBODY COMPONENT WITH ROTATION CONSTRAINTS !!!
        rb = GetComponent<Rigidbody>();
        
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        UpdatePosition();
        UpdateRotation();
    }

    void UpdatePosition(){
        hInput = Input.GetAxisRaw("Horizontal");
        vInput = Input.GetAxisRaw("Vertical");

        // set player velocity
        Vector3 direction = Vector3.forward*vInput*ForwardSpeed+Vector3.right*hInput*SidewaysSpeed;
        rb.velocity = transform.TransformDirection(direction);

        // locks position when not moving to prevent sliding
        if(hInput == 0 && vInput == 0)
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        else
            rb.constraints = RigidbodyConstraints.FreezeRotation;

        // attract player towards sphere
        rb.AddForce(transform.position.normalized*-Gravity);
    }
    void UpdateRotation()
	{
        // point players feet towards sphere
		Vector3 gravityUp = transform.position.normalized;
		transform.rotation = Quaternion.FromToRotation(transform.up, gravityUp) * transform.rotation;
        // rotate around y axis with mouse movement
        float mouseX = Input.GetAxis("Mouse X");
        transform.Rotate(Vector3.up, mouseX);
	}
}