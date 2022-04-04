using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderThingEnemy : BaseEnemy
{
    public float Speed;
    public float SeekRadius;
    public float FleeRadius;
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
        if(_planeDistanceFromPlayer >= SeekRadius)
            speed = Speed;
        else if(_planeDistanceFromPlayer <= FleeRadius)
            speed = -1.33f*Speed;
        else speed = 0;
        
        Vector3 oldVel = _rb.velocity;
        Vector3 newVel =  (transform.right*_towardsPlayer.x + transform.forward*_towardsPlayer.z)*speed;
        _rb.velocity = Vector3.Lerp(oldVel, newVel, .1f);
    }
}
