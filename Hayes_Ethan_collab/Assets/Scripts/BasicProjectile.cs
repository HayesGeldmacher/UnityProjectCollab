using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BasicProjectile : Shot
{
    [Header("Projectile")]
    public float Speed;
    public bool SeekPlayer;
    public float SeekStrength;

    // Update is called once per frame
    void Update()
    {
        UpdatePosition();
    }

    void UpdatePosition()
    {
        if(SeekPlayer){
            Vector3 targetPoint = GameObject.FindGameObjectWithTag("Player").transform.position - transform.position;
            Quaternion desiredRotation = Quaternion.FromToRotation(transform.forward, targetPoint) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, SeekStrength/100);
        }

        transform.Translate(Vector3.forward * Speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        OnHit(other.gameObject);
    }
}
