using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float ForwardSpeed, SidewaysSpeed = 10;

    private float hInput, vInput;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        hInput = Input.GetAxis("Horizontal");
        vInput = Input.GetAxis("Vertical");


        

        Vector3 direction = Vector3.forward*vInput*ForwardSpeed+Vector3.right*hInput*SidewaysSpeed;
        transform.Translate(direction*Time.deltaTime);

        UpdateRotation();
    }

    void UpdateRotation()
	{
		Vector3 gravityUp = transform.position.normalized;
		transform.rotation = Quaternion.FromToRotation(transform.up, gravityUp) * transform.rotation;
	}
}