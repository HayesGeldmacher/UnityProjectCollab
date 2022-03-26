using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Balance")]
    public float Damage;
    public float Speed;

    [Header("Engine")]
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
        transform.Translate(Vector3.forward * Speed * Time.deltaTime);
    }

    private void Hit(GameObject hit)
    {
        Damageable d = hit.GetComponent<Damageable>();
        if (d != null)
            d.Damage(Damage);
        Debug.Log($"HIT-{hit.gameObject.name}");
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        // I think this does stuff with layer masks to check if the thing hit is in it
        if (CanHit == (CanHit | (1 << other.gameObject.layer)))
            OnHit(other.gameObject);
    }
}
