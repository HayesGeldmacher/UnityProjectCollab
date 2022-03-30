using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Projectile : MonoBehaviour
{
    [Header("Projectile")]
    public float Damage;
    public float Speed;
    public bool SeekPlayer;
    public float SeekStrength;

    [Header("Layer Mask")]
    public LayerMask CanHit;

    public delegate void HitHandler(GameObject hit);
    public event HitHandler OnHit;

    // Start is called before the first frame update
    void Start()
    {
        OnHit += Hit;
    }

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

    private void Hit(GameObject hit)
    {
        if (hit.TryGetComponent<Damageable>(out Damageable d))
            d.Damage(Damage);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        // I think this does stuff with layer masks to check if the thing hit is in it
        if (CanHit == (CanHit | (1 << other.gameObject.layer)))
            OnHit(other.gameObject);
    }
}
