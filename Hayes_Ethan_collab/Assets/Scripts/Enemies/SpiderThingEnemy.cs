using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderThingEnemy : BaseEnemy
{
    public float Speed;
    public float SightDistance;
    public float SeekRadius;
    public float FleeRadius;
    public Animator anim;
    public Transform spidermodel;


    //Added the prefab for blood explosion
    [Header("BloodEffects")]
    public GameObject bloodExplosion;
    public GameObject bloodSplatter;

    public override void Start()
    {

        base.Start();
       
        //Added onDamage so the spider can register the damage anim, but feel free to change if needed
        //Possibly change rotation of instantiated hurt effect so that it faces straight out the sphere?
        OnDamage += () =>
        {
            anim.SetTrigger("Damaged");
            Instantiate(bloodSplatter, transform.position, transform.rotation);
        };
        //Same goes for OnDeath, change as needed
        OnDeath += () => Instantiate(bloodExplosion, transform.position, transform.rotation);
      
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if(!_attacking)
            UpdateMovement();
        UpdateAnimation();
       
    }

    void UpdateMovement(){
        // TODO: fix bug when you are on opposite pole of enemy
        if(transform.InverseTransformPoint(_player.transform.position).y <= -SightDistance)
            return;

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


    void UpdateAnimation()
    {
        anim.SetBool("Shooting", _attacking);
        anim.SetBool("Stunned", Staggered);

    }
}
