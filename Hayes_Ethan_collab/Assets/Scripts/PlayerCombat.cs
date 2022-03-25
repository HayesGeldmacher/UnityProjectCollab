using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Shots")]
    public Transform BulletSpawn;
    public Transform Shot;
    public Transform ChargeShot;
    public float ShotDespawnTime = 5;

    [Header("Damage")]
    public float ShotDamage;
    public float ChargeShotDamage;

    [Header("Charge Time")]
    public float MinChargeTime;
    public float MaxChargeTime;

    [Header("Cool Down")]
    public float ShotCoolDown;
    public float ChargeShotCoolDown;


    private float _chargeStartTime;
    private float _chargeTime;
    private bool _isCharging;
    private float _coolDownTime;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateShooting();
    }

    void UpdateShooting(){
        _coolDownTime -= Time.deltaTime;
        
        // start charging on click
        if(Input.GetMouseButtonDown(0) && !_isCharging && _coolDownTime<0){
            _isCharging = true;
            _chargeStartTime = Time.time;
        }
        _chargeTime = Time.time - _chargeStartTime;
        // stop charging on mouse up or timer runs out
        if ((Input.GetMouseButtonUp(0) || _chargeTime>MaxChargeTime) && _isCharging){
            _isCharging = false;
            if(_chargeTime>MinChargeTime)
                FireChargeShot(_chargeTime);
            else
                FireShot();
        }
    }

    void FireShot(){
        GameObject shot = Instantiate(Shot, BulletSpawn).gameObject; // create shot
        shot.transform.SetParent(null); // detach from player transform
        shot.GetComponent<Projectile>().SetDamage(ShotDamage); // set damage of shot
        Destroy(shot, ShotDespawnTime); // destroy it for performance reasons
        _coolDownTime = ShotCoolDown; // update cooldown so no new rapid shots
    }

    void FireChargeShot(float timeCharged){
        GameObject shot = Instantiate(ChargeShot, BulletSpawn).gameObject; // create shot
        shot.transform.SetParent(null); // detach from player transform
        // percentage of charge * damage for charged shot
        float damage = timeCharged/MaxChargeTime * ChargeShotDamage;
        shot.GetComponent<Projectile>().SetDamage(damage);// set damage of shot
        Destroy(shot, ShotDespawnTime);// destroy it for performance reasons
        _coolDownTime = ChargeShotCoolDown;// update cooldown so no new rapid shots
    }
}
