using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    public float Health;

    public delegate void DamageHandler();
    public event DamageHandler OnDamage;

    public delegate void DeathHandler();
    public event DeathHandler OnDeath;

    // Start is called before the first frame update
    void Start()
    {
        OnDeath += Death;
        OnDamage += ()=> { }; // add empty method so the event can be used
    }

    void Death()
    {
        Destroy(gameObject);
    }

    public void Damage(float damage)
    {
        OnDamage();
        Health -= damage;
        if (Health <= 0)
            OnDeath();
    }
}
