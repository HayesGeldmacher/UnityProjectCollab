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
    [Space(5)]
    public float StaggerDamage;
    public float StaggerWindow;
    public float StaggerTime;
    [Space(5)]
    public GameObject StaggerEffect;
    public AudioClip StaggerSound;

    [HideInInspector]
    public float CurrentHealth;
    public bool Invincible;
    [HideInInspector]
    public bool Staggered;
    [HideInInspector]
    public bool Tethered;

    private float _staggerDamageCounter;



    public delegate void DamageHandler();
    public event DamageHandler OnDamage;

    public delegate void DeathHandler();
    public event DeathHandler OnDeath;

    public virtual void Awake(){
        CurrentHealth = Health;
    }

    private IEnumerator Stagger(){
        Debug.Log("staggered");
        if(StaggerSound) AudioSource.PlayClipAtPoint(StaggerSound, transform.position);
        if(StaggerEffect) Destroy(Instantiate(StaggerEffect), StaggerTime);
        Staggered = true;
        yield return new WaitForSeconds(StaggerTime);
        Staggered = false;
        _staggerDamageCounter = 0;
    }

    private IEnumerator AddToDamageCounter(float damage){
        _staggerDamageCounter += damage;
        if(_staggerDamageCounter >= StaggerDamage)
            StartCoroutine(Stagger());
        yield return new WaitForSeconds(StaggerWindow);
        if(_staggerDamageCounter >= damage)
            _staggerDamageCounter -= damage;
    }

    public bool Damage(float damage)
    {
        if(Invincible || damage <= 0) return false;
        CurrentHealth -= damage;
        StartCoroutine(AddToDamageCounter(damage));
        if(OnDamage != null)
            OnDamage();
        if (CurrentHealth <= 0 && OnDeath != null)
            OnDeath();
        return true;
    }

    public bool Heal(float amount){
        if(amount <= 0 ) return false;
        CurrentHealth += amount;
        if(CurrentHealth > Health)
            CurrentHealth = Health;
        return true;
    }
}
