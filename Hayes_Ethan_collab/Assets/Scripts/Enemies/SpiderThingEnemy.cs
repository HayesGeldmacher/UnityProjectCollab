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
    public GameObject GoreChunk;
    public GameObject Corpse;

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
        OnDeath += () =>
        {
            //Activates and seperates the corpse mesh from the spider before it's destroyed
            //MeshRenderer corpseRenderer = Corpse.transform.GetComponent<MeshRenderer>();
            Corpse.SetActive(true);
            AudioSource scream = Corpse.transform.GetComponent<AudioSource>();
            if (scream)
            {
                scream.Play();
            }
            Instantiate(bloodExplosion, Corpse.transform.position, transform.rotation);
            
            //corpseRenderer.enabled = true;
            Corpse.transform.parent = null;
        };
        
      
    }

    // Update is called once per frame
    public override void Update()
    {
        if(!_attacking)
            UpdateMovement();
        
        base.Update();
        
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

        if (!Staggered)
            transform.LookAt(transform.position+_towardsPlayer);

        _rb.velocity = transform.forward*speed;
    }


    void UpdateAnimation()
    {
        anim.SetBool("Shooting", _attacking);
        anim.SetBool("Stunned", Staggered);

    }
}
