using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cubeRotate : MonoBehaviour
{
   
    public float zRotation;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.right * zRotation * Time.deltaTime);
    }
}
