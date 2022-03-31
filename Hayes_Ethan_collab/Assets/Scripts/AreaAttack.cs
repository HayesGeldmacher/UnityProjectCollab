using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AreaAttack : MonoBehaviour
{
    [Header("Area Attack")]
    public float Damage;
    public float TimeBetweenDamages;
    public bool OnlyDamageOnce;
    public float DespawnTime;

    [Header("Layer Mask")]
    public LayerMask CanHit;

    private float _timeLastHit;
    private bool _hasHit;

    public delegate void HitHandler(GameObject hit);
    public event HitHandler OnHit;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, DespawnTime);
        OnHit += Hit;
    }

    private void Hit(GameObject hit){
        _hasHit = true;
        if (hit.TryGetComponent<Damageable>(out Damageable d))
            if(d.Damage(Damage))
                _timeLastHit = Time.time;
    }

    private void OnTriggerStay(Collider other){
        if (CanHit == (CanHit | (1 << other.gameObject.layer))) //if in the mask
            if(Time.time-_timeLastHit >= TimeBetweenDamages && !(OnlyDamageOnce && _hasHit)) // checks for only damage once and time between hits
                OnHit(other.gameObject);
    }
}
