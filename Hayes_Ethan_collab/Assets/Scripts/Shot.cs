using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class Shot : MonoBehaviour
{
    public float Damage;

    [Header("Layer Mask")]
    public LayerMask CanHit;

    public delegate void HitHandler(GameObject hit);
    public HitHandler OnHit;

    private void Hit(GameObject other){
        if (CanHit == (CanHit | (1 << other.gameObject.layer)))
            if(other.TryGetComponent<Damageable>(out Damageable d))
                d.Damage(Damage);
    }

    public virtual void Awake(){
        OnHit += Hit;
    }
}
