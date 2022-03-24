using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Shots")]
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


    private Transform BulletSpawn;

    private float _chargeStartTime;
    public float _chargeTime;
    public bool _isCharging;
    private float _coolDownTime;

    // Start is called before the first frame update
    void Start()
    {
        // TODO update this to be more precise
        BulletSpawn = GetComponentInChildren<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateShooting();
    }

    void UpdateShooting(){
        // TODO: fix bug with double/repeated shots when mouse up after Charge timeout
        // TODO: fix bug where charge shot is fired if havent fired in long time

        _chargeTime = Time.time-_chargeStartTime;
        _coolDownTime -= Time.deltaTime;
        // start charging on click
        if(Input.GetMouseButton(0) && !_isCharging && _coolDownTime<0){
            _isCharging = true;
            _chargeStartTime = Time.time;
        }
        // stop charging on mouse up or timer runs out
        if((!Input.GetMouseButton(0) || _chargeTime>MaxChargeTime) && _isCharging){
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
        shot.GetComponent<Shot>().SetDamage(ShotDamage); // set damage of shot
        Destroy(shot, ShotDespawnTime); // destroy it for performance reasons
        _coolDownTime = ShotCoolDown; // update cooldown so no new rapid shots
    }

    void FireChargeShot(float timeCharged){
        GameObject shot = Instantiate(ChargeShot, BulletSpawn).gameObject; // create shot
        shot.transform.SetParent(null); // detach from player transform
        // percentage of charge * damage for charged shot
        float damage = timeCharged/MaxChargeTime * ChargeShotDamage;
        shot.GetComponent<Shot>().SetDamage(damage);// set damage of shot
        Destroy(shot, ShotDespawnTime);// destroy it for performance reasons
        _coolDownTime = ChargeShotCoolDown;// update cooldown so no new rapid shots
    }
}
