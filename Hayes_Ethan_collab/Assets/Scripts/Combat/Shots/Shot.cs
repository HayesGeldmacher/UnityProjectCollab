using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class Shot : MonoBehaviour
{
    public float Damage;
    public bool DestroyOnHit;

    [Header("Layer Mask")]
    public LayerMask CanHit;

    public delegate void HitHandler(GameObject hit);
    public HitHandler OnHit;

    private void Hit(GameObject other){
        if(other.TryGetComponent<Damageable>(out Damageable d))
            d.Damage(Damage);
        if(DestroyOnHit)
            Destroy(gameObject);
    }

    protected bool InLayerMask(GameObject obj){
        return CanHit == (CanHit | (1 << obj.gameObject.layer));
    }

    public virtual void Awake(){
        OnHit += Hit;
    }
}
