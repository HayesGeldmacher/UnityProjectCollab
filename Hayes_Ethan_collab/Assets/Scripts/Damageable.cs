using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    public float Health;

    public delegate void DamageHandler(float damage);
    public event DamageHandler OnDamage;

    public delegate void DeathHandler(GameObject obj);
    public event DeathHandler OnDeath;

    // Start is called before the first frame update
    public virtual void Start()
    {
        OnDeath += Death;
        OnDamage += Damaged;
    }

    void Death(GameObject obj)
    {
        Destroy(obj);
    }

    void Damaged(float damage)
    {
        Debug.Log($"{name} - {Health}");
    }

    public void Damage(float damage)
    {
        OnDamage(damage);
        Health -= damage;
        if (Health <= 0)
            OnDeath(gameObject);
    }
}
