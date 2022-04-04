using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderThingEnemy : BaseEnemy
{
    public float Speed;
    public override void Start()
    {
     base.Start();   
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if(!_attacking)
            UpdateMovement();
    }

    void UpdateMovement(){
        float speed;
        if(_planeDistanceFromPlayer >= 5)
            speed = Speed;
        else
            speed = -Speed;
        
        Vector3 oldVel = _rb.velocity;
        Vector3 newVel =  (transform.right*_towardsPlayer.x + transform.forward*_towardsPlayer.z)*speed;
        _rb.velocity = Vector3.Lerp(oldVel, newVel, .1f);
    }
}
