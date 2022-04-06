using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    [Space(15)]
    [Header("Health")]
    public float Health;
    private float _health;
    [Space(5)]
    public AudioClip DamageSound;

    [HideInInspector]
    public bool Invincible;

    public delegate void DamageHandler();
    public event DamageHandler OnDamage;

    public delegate void DeathHandler();
    public event DeathHandler OnDeath;

    public virtual void Awake(){
        _health = Health;
    }

    public bool Damage(float damage)
    {
        if(Invincible || damage <= 0) return false;
        _health -= damage;
        if(OnDamage != null)
            OnDamage();
        if (_health <= 0 && OnDeath != null)
            OnDeath();
        return true;
    }

    public bool Heal(float amount){
        if(amount <= 0 ) return false;
        _health += amount;
        if(_health > Health)
            _health = Health;
        return true;
    }
}
