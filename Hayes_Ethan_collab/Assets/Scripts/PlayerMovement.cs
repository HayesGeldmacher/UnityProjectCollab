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
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        hInput = Input.GetAxis("Horizontal");
        vInput = Input.GetAxis("Vertical");

        Vector3 direction = Vector3.forward*vInput*ForwardSpeed+Vector3.right*hInput*SidewaysSpeed;
        transform.Translate(direction*Time.deltaTime);

        UpdateRotation();
        //rb.AddForce(transform.position.normalized*-Gravity);
    }

    void UpdateRotation()
	{
        float mouseX = Input.GetAxis("Mouse X");
		Vector3 gravityUp = transform.position.normalized;
		transform.rotation = Quaternion.FromToRotation(transform.up, gravityUp) * transform.rotation;
        transform.Rotate(Vector3.up, mouseX);
	}
}