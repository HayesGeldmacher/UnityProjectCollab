using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Player : Damageable
{
    // public stuff
    [Header("Movement")]
    public float ForwardSpeed = 12;
    public float SidewaysSpeed = 9;
    public float Gravity = 9.81f;

    [Header("Shots")]
    public Transform BulletSpawn;
    public GameObject Shot;
    public GameObject ChargeShot;
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

    // animation stuff
    private bool _isWalking;

    // private stuff
    private Rigidbody _rb;
    private float _hInput, _vInput;
    private bool _fireUp, _fireDown, _fireHold;
    private float _chargeStartTime;
    private float _chargeTime;
    private bool _isCharging;
    private float _coolDownTime;

    public override void Start()
    {
        base.Start();
        _rb = GetComponent<Rigidbody>();
        
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        UpdatePosition();
        UpdateRotation();
        UpdateShooting();
        UpdateAnimation();
    }

    void UpdatePosition(){
        _hInput = Input.GetAxisRaw("Horizontal");
        _vInput = Input.GetAxisRaw("Vertical");

        // set player velocity
        Vector3 direction = Vector3.forward*_vInput*ForwardSpeed+Vector3.right*_hInput*SidewaysSpeed;
        _rb.velocity = transform.TransformDirection(direction);

        // locks position when not moving to prevent sliding
        if(_hInput == 0 && _vInput == 0)
            _rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        else
            _rb.constraints = RigidbodyConstraints.FreezeRotation;

        // attract player towards sphere
        _rb.AddForce(transform.position.normalized*-Gravity);
    }

    void UpdateRotation()
	{
        // point players feet towards sphere
		Vector3 gravityUp = transform.position.normalized;
		transform.rotation = Quaternion.FromToRotation(transform.up, gravityUp) * transform.rotation;
        // rotate around y axis with mouse movement
        float mouseX = Input.GetAxis("Mouse X");
        transform.Rotate(Vector3.up, mouseX);
	}

    void UpdateShooting()
    {
        _coolDownTime -= Time.deltaTime;
        if(Input.GetAxisRaw("Fire1") > .5 && !_fireHold){
            _fireDown = true;
            _fireHold = true;
        }
        if(Input.GetAxisRaw("Fire1") < .5 && _fireHold){
            _fireUp = true;
            _fireHold = false;
        }

        // start charging on click
        if (_fireDown && !_isCharging && _coolDownTime < 0)
        {
            _isCharging = true;
            _chargeStartTime = Time.time;
        }
        _chargeTime = Time.time - _chargeStartTime;
        // stop charging on mouse up or timer runs out
        if ((_fireUp || _chargeTime > MaxChargeTime) && _isCharging)
        {
            _isCharging = false;
            if (_chargeTime > MinChargeTime)
                FireChargeShot(_chargeTime);
            else
                FireShot();
        }

        _fireUp = false;
        _fireDown = false;
    }

    void FireShot()
    {
        GameObject shot = Instantiate(Shot, BulletSpawn); // create shot
        shot.transform.SetParent(null); // detach from player transform
        shot.GetComponent<Projectile>().Damage = ShotDamage; // set damage of shot
        Destroy(shot, ShotDespawnTime); // destroy it for performance reasons
        _coolDownTime = ShotCoolDown; // update cooldown so no new rapid shots
    }

    void FireChargeShot(float timeCharged)
    {
        GameObject shot = Instantiate(ChargeShot, BulletSpawn); // create shot
        shot.transform.SetParent(null); // detach from player transform
        // percentage of charge * damage for charged shot
        float damage = timeCharged / MaxChargeTime * ChargeShotDamage;
        shot.GetComponent<Projectile>().Damage = damage;// set damage of shot
        Destroy(shot, ShotDespawnTime);// destroy it for performance reasons
        _coolDownTime = ChargeShotCoolDown;// update cooldown so no new rapid shots
    }

    void UpdateAnimation(){
        _isWalking = !(_hInput == 0 && _vInput == 0);
    }
}