using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // public stuff
    [Header("Movement")]
    public float ForwardSpeed = 10;
    public float SidewaysSpeed = 5;

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
        hInput = Input.GetAxisRaw("Horizontal");
        vInput = Input.GetAxisRaw("Vertical");

        Vector3 direction = Vector3.forward*vInput*ForwardSpeed+Vector3.right*hInput*SidewaysSpeed;
        rb.velocity = transform.TransformDirection(direction);

        if(hInput == 0 && vInput == 0)
            rb.constraints = RigidbodyConstraints.FreezeAll;
        else
            rb.constraints = RigidbodyConstraints.FreezeRotation;

        UpdateRotation();
        rb.AddForce(transform.position.normalized*-Gravity);
    }

    void UpdateRotation()
	{
        float mouseX = Input.GetAxis("Mouse X");
		Vector3 gravityUp = transform.position.normalized;
		transform.rotation = Quaternion.FromToRotation(transform.up, gravityUp) * transform.rotation;
        transform.Rotate(Vector3.up, mouseX);
	}
}