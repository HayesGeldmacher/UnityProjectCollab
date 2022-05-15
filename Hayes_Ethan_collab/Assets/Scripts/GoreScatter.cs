using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoreScatter : MonoBehaviour
{
    public Rigidbody rigid;
    public float HorizontalSpeed;
    public float VerticalSpeed;
    //Possibly add system to randomly change the mesh to other chunk meshes!
   
    // Start is called before the first frame update
    void Start()
    {
        Explosion();
        
    }

    // Update is called once per frame
    void Update()
    {
        
        //Gravity, attracts piece to the center of the world
        Vector3 TowardsCore = new Vector3(0, 0, 0) - transform.position;
        rigid.AddForce(TowardsCore * 0.1f);
        

    }
    
    void Explosion()
    {
       //Sends the piece flying in a random direction, before gravity brings it back in void Update
        Vector3 AwayFromCore = transform.position - new Vector3(0, 0, 0);
        float RandomZ = Random.Range(0, VerticalSpeed);
        float RandomX = Random.Range(-HorizontalSpeed, HorizontalSpeed);
        float RandomY = Random.Range(-HorizontalSpeed, HorizontalSpeed);
        //rigid.AddForce(new Vector3(AwayFromCore.x * RandomX, AwayFromCore.y * RandomY, AwayFromCore.z * RandomZ));
        //rigid.AddForce(AwayFromCore * RandomZ);
        rigid.AddForce(AwayFromCore.x * RandomX, AwayFromCore.y * RandomY, AwayFromCore.z * RandomZ);
    }
}
