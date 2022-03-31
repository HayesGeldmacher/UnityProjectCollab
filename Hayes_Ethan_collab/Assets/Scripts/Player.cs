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

    [Header("Dash")]
    public float DashSpeed;
    public float DashTime;
    public float InvincibilityTime;
    public float DashCooldown;

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

    [Header("Animation")]
    public Animator Anim;
    public Transform PlayerModel;
    private bool _isWalking;
    

    // private stuff
    private Rigidbody _rb;
    private float _hInput, _vInput;
    private Vector3 _direction;
    private bool _isDashing;
    private float _dashStartTime, _dashEndTime;
    private bool _fireUp, _fireDown, _fireHold;
    private float _chargeStartTime;
    private float _chargeTime;
    private bool _isCharging;
    private float _coolDownTime;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        
        Cursor.lockState = CursorLockMode.Locked;

        OnDamage += () => Debug.Log($"Player Hit - Health: {Health}");
        OnDeath += () => Destroy(gameObject);
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
        
        bool _dashButton = Input.GetAxisRaw("Dash") > .1;

        // sets the start and end of dashing and invincibility
        if(_dashButton && !_isDashing && Time.time-_dashEndTime >= DashCooldown){
            _isDashing = true;
            Invincible = true;
            _dashStartTime = Time.time;
        }
        if(Time.time-_dashStartTime >= InvincibilityTime){
            Invincible = false;
        }
        if(_isDashing & Time.time-_dashStartTime >= DashTime){
            _isDashing = false;
            _dashEndTime = Time.time;
        }

        // set player velocity
        if(!_isDashing){
            _direction = transform.forward*_vInput*ForwardSpeed+transform.right*_hInput*SidewaysSpeed;
            _rb.velocity = _direction;
        }else{
            _direction = transform.forward*_vInput+transform.right*_hInput;
            _rb.velocity = _direction * DashSpeed;
        }

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
        
        BulletSpawn.LookAt(Vector3.zero);
        // point players feet towards sphere
		Vector3 gravityUp = transform.position.normalized;
		transform.rotation = Quaternion.FromToRotation(transform.up, gravityUp) * transform.rotation;
        // point player model in direction that is walking
        PlayerModel.transform.rotation = Quaternion.FromToRotation(PlayerModel.transform.up, gravityUp) * PlayerModel.transform.rotation;
        Quaternion desiredRotation = Quaternion.FromToRotation(PlayerModel.forward, _direction) * PlayerModel.rotation;
        PlayerModel.transform.rotation = Quaternion.Slerp(PlayerModel.transform.rotation, desiredRotation, .02f);

        // rotate around y axis with mouse movement
        float hLook = Input.GetAxis("Horizontal Look");
        transform.Rotate(Vector3.up, hLook);
        PlayerModel.Rotate(Vector3.up, -hLook);
	}

    void UpdateShooting()
    {
        _coolDownTime -= Time.deltaTime;
        if(Input.GetAxisRaw("Fire") > .5 && !_fireHold){
            _fireDown = true;
            _fireHold = true;
        }
        if(Input.GetAxisRaw("Fire") < .5 && _fireHold){
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
        Vector3 scale = shot.transform.localScale;
        shot.transform.SetParent(null); // detach from player transform
        shot.transform.localScale = scale;
        shot.GetComponent<Projectile>().Damage = ShotDamage; // set damage of shot
        Destroy(shot, ShotDespawnTime); // destroy it for performance reasons
        _coolDownTime = ShotCoolDown; // update cooldown so no new rapid shots
    }

    void FireChargeShot(float timeCharged)
    {
        GameObject shot = Instantiate(ChargeShot, BulletSpawn); // create shot
        Vector3 scale = shot.transform.localScale;
        shot.transform.SetParent(null); // detach from player transform
        shot.transform.localScale = scale;
        // percentage of charge * damage for charged shot
        float damage = timeCharged / MaxChargeTime * ChargeShotDamage;
        shot.GetComponent<Projectile>().Damage = damage;// set damage of shot
        Destroy(shot, ShotDespawnTime);// destroy it for performance reasons
        _coolDownTime = ChargeShotCoolDown;// update cooldown so no new rapid shots
    }

    void UpdateAnimation(){

        _isWalking = (Mathf.Abs(_vInput) > 0.1 || Mathf.Abs(_hInput) > 0.1);

        // if we have more animations in the future, might be useful to use integer instead of bool

        Anim.SetBool("Walking", _isWalking);
    }
}