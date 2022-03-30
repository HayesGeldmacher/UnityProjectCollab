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

    public void Damage(float damage)
    {
        
        Health -= damage;
        if(OnDamage != null)
            OnDamage();
        if (Health <= 0 && OnDeath != null)
            OnDeath();
    }
}
