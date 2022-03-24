using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
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
    public float _chargeTime;
    public bool _isCharging;
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
        _chargeTime = Time.time-_chargeStartTime;
        _coolDownTime -= Time.deltaTime;
        // start charging on click
        if(Input.GetMouseButton(0) && !_isCharging && _coolDownTime<0){
            _isCharging = true;
            _chargeStartTime = Time.time;
        }
        // stop charging on mouse up or timer runs out
        if((!Input.GetMouseButton(0) || _chargeTime>MaxChargeTime)&& _isCharging){
            _isCharging = false;
            if(_chargeTime>MinChargeTime)
                FireChargeShot(_chargeTime);
            else
                FireShot();
        }
    }

    void FireShot(){
        // TODO
        _coolDownTime = ShotCoolDown;
    }

    void FireChargeShot(float timeCharged){
        // percentage of charge * damage for charged shot
        float damage = timeCharged/MaxChargeTime * ChargeShotDamage;

        // TODO
        _coolDownTime = ChargeShotCoolDown;
    }
}
