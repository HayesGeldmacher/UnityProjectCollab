using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    [Space(15)]
    [Header("Health")]
    public float Health;
    [Space(5)]
    public AudioClip DamageSound;

    [HideInInspector]
    public bool Invincible;

    public delegate void DamageHandler();
    public event DamageHandler OnDamage;

    public delegate void DeathHandler();
    public event DeathHandler OnDeath;

    public bool Damage(float damage)
    {
        if(Invincible) return false;
        Health -= damage;
        if(OnDamage != null)
            OnDamage();
        if (Health <= 0 && OnDeath != null)
            OnDeath();
        return true;
    }
}
