using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider), typeof(AudioSource))]
public class Player : Damageable
{
    // Public
    [Space(15)]
    [Header("Movement")]
    public float Speed;
    [Space(5)]
    public float DashSpeed;
    public float DashTime;
    public float InvincibilityTime;
    public float DashCooldown;
    [Space(5)]
    public AudioClip WalkSound;
    public GameObject DashEffect;
    public AudioClip DashSound;
    [Space(15)]
    
    [Header("Combat")]
    public Transform BulletSpawn;
    public Transform SiphonSpawn;
    [Space(15)]

    [Header("Regular Shot")]
    public GameObject Shot;
    public float ShotDamage;
    public float ShotCooldown;
    public float ShotDespawnTime;
    [Space(5)]
    public GameObject ShotEffect;
    public AudioClip ShotSound;
    [Space(15)]

    [Header("Charged Shot")]
    public GameObject ChargeShot;
    public float ChargeShotDamage;
    public float MinChargeTime;
    public float MaxChargeTime;
    public float ChargeShotCoolDown;
    public float ChargeShotDespawnTime;
    [Space(5)]
    public GameObject ChargingEffect;
    public AudioClip ChargingSound;
    public GameObject ChargeShotEffect;
    public AudioClip ChargeShotSound;
    public GameObject ChargeCooldownEffect;
    public AudioClip ChargeCooldownSound;
    [Space(15)]

    [Header("Siphon")]
    public float LifeStealAmount;
    public float LifeStealTick;
    public float SiphonPredelay;
    public float SiphonPostdelay;
    [Space(5)]
    public AudioClip SiphonHitSound;
    public AudioClip SiphonMissSound;
    [Space(15)]

    [Header("Model")]
    public Animator Anim;
    public Transform PlayerModel;

    // Private
    private Camera _camera;
    private LayerMask _cameraRaycastMask;
    private Vector3 _camerRaycastPoint;
    private Vector3 _siphonHitPoint;
    private LineRenderer _lineRenderer;
    private AudioSource _audio;
    private Rigidbody _rb;
    private Vector3 _direction;
    private GameObject _chargeEffect;
    private float _hInput, _vInput;
    private float _lockedHInput, _lockedVInput;
    private bool _isWalking;
    private bool _isDashing, _dashCooldown;
    private bool _fireDown, _fireUp, _fireHold;
    private bool _secondaryFireDown, _secondaryFireUp, _secondaryFireHold;
    private bool _isCharging, _fireQueued;
    private float _chargeStartTime, _chargeTime;
    private bool _isFiring, _isHeavyFiring;
    private bool _isSiphoning;
    private Damageable _siphonTarget;
    private float _coolDownTime;
    private float _siphonCooldown;
    private bool _startChargeEffects;

    void Start()
    {
        _camera = Camera.main;
        _cameraRaycastMask = LayerMask.GetMask("Enemy", "Bottom Sphere");
        _rb = GetComponent<Rigidbody>();
        _lineRenderer = GetComponent<LineRenderer>();
        _audio = GetComponent<AudioSource>();
        
        OnDamage += () => {
            if(DamageSound) _audio.PlayOneShot(DamageSound);
            Anim.SetTrigger("Damaged");
        };
        OnDeath += () => Destroy(gameObject);
    
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        UpdatePosition();
        UpdateRotation();
        UpdateSiphon();
        UpdateShooting();
        UpdateAnimation();
    }

    void LateUpdate()
    {
        _lineRenderer.SetPosition(0, SiphonSpawn.position);
        Vector3 oldPos = _lineRenderer.GetPosition(1);
        Vector3 newPos = Vector3.Lerp(oldPos, _siphonHitPoint, .05f);
        _lineRenderer.SetPosition(1, newPos);
    }

    private void UpdatePosition()
    {
        _hInput = Input.GetAxisRaw("Horizontal");
        _vInput = Input.GetAxisRaw("Vertical");

        bool dashButton = Input.GetAxisRaw("Dash") > .1;
        bool moving = Mathf.Abs(_hInput)>.1 || Mathf.Abs(_vInput)>.1;

        if(moving && !_isDashing && !_dashCooldown && dashButton)
            StartCoroutine(Dash());

        if(_isDashing){
            _vInput = _lockedVInput;
            _hInput = _lockedHInput;
        }

        // set player velocity based on dashing or not
        if(!_isDashing){
            _direction = (transform.forward*_vInput+transform.right*_hInput)*Speed;
            if(_direction.magnitude > Speed)
                _direction = _direction.normalized*Speed;
            _rb.velocity = _direction;
        }else{
            _direction = (transform.forward*_vInput+transform.right*_hInput).normalized;
            _rb.velocity = _direction * DashSpeed;
        }

        if(_isSiphoning){
            _rb.velocity = Vector3.zero;
        }

        // locks position when not moving to prevent sliding
        if(_hInput == 0 && _vInput == 0)
            _rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        else
            _rb.constraints = RigidbodyConstraints.FreezeRotation;

        // attract player towards sphere
        _rb.AddForce(transform.position.normalized*-9.81f);
    }

    private IEnumerator Dash(){
        if(DashSound) _audio.PlayOneShot(DashSound);
        if(DashEffect) Destroy(Instantiate(DashEffect), DashTime);
        _isDashing = true;
        Invincible = true;
        _lockedHInput = _hInput;
        _lockedVInput = _vInput;
        yield return new WaitForSeconds(InvincibilityTime);
        Invincible = false;
        yield return new WaitForSeconds(DashTime - InvincibilityTime);
        _isDashing = false;
        _dashCooldown = true;
        yield return new WaitForSeconds(DashCooldown);
        _dashCooldown = false;
    }


    private void UpdateRotation()
    {
        // Update Rotation of Bullet and Siphon Spawn;
        Vector3 rotation = _camera.transform.forward+_camera.transform.up*-.1f;
        Physics.Raycast(_camera.transform.position, rotation, out RaycastHit hit, 100f, _cameraRaycastMask);
        _camerRaycastPoint = hit.point;
        BulletSpawn.transform.LookAt(hit.point);
        SiphonSpawn.transform.LookAt(hit.point);

        // Update Player rotation relative to the sphere
        Vector3 gravityUp = transform.position.normalized;
		transform.rotation = Quaternion.FromToRotation(transform.up, gravityUp) * transform.rotation;
        
        // Update model rotation in direction of walking
        if(!_isSiphoning){
            PlayerModel.transform.rotation = Quaternion.FromToRotation(PlayerModel.transform.up, gravityUp) * PlayerModel.transform.rotation;
            Quaternion desiredRotation = Quaternion.FromToRotation(PlayerModel.forward, _direction) * PlayerModel.rotation;
            PlayerModel.transform.rotation = Quaternion.Slerp(PlayerModel.transform.rotation, desiredRotation, .05f);
        }

        // Rotate Camera around y axis with mouse movement
        float hLook = Input.GetAxis("Horizontal Look");
        transform.Rotate(Vector3.up, hLook);
        PlayerModel.Rotate(Vector3.up, -hLook);
    }

    private void UpdateSiphon(){
        _siphonCooldown -= Time.deltaTime;
        // Set up butotn down and button up
        if(Input.GetAxisRaw("Secondary Fire") > .5 && !_secondaryFireHold){
            _secondaryFireDown = true;
            _secondaryFireHold = true;
        }
        if(Input.GetAxisRaw("Secondary Fire") < .5 && _secondaryFireHold){
            _secondaryFireUp = true;
            _secondaryFireHold = false;
        }

        // logic to start and stop siphon state
        if(_secondaryFireDown && !_isCharging && !_isDashing){
            StartCoroutine(SiphonEffects());
            Vector3 dir = _camerRaycastPoint - SiphonSpawn.position;
            Physics.Raycast(SiphonSpawn.position, dir, out RaycastHit hit, 100f, _cameraRaycastMask);
            _siphonHitPoint = hit.point;
            if(hit.collider.gameObject.TryGetComponent<Damageable>(out Damageable d))
                if(d.Staggered && CurrentHealth != Health)
                    StartCoroutine(StartSiphon(d));
        }
        if((!_secondaryFireHold || CurrentHealth == Health) && _isSiphoning){
            StartCoroutine(StopSiphon());
        }

        // heal and do damage;
        if(_isSiphoning && _siphonCooldown <=0){
            _siphonCooldown = LifeStealTick;
            _siphonTarget.Damage(LifeStealAmount);
            Heal(LifeStealAmount);
        }

        _secondaryFireDown = false;
        _secondaryFireUp = false;
    }

    private IEnumerator SiphonEffects(){
        _lineRenderer.SetPosition(1, SiphonSpawn.position);
        _lineRenderer.enabled = true;
        yield return new WaitForSeconds(SiphonPredelay+.01f);

        if(_isSiphoning && SiphonHitSound) _audio.PlayOneShot(SiphonHitSound);
        if(!_isSiphoning && SiphonMissSound) _audio.PlayOneShot(SiphonMissSound);

        while(_isSiphoning){
            yield return null;
        }
        _siphonHitPoint = SiphonSpawn.position;
        yield return new WaitForSeconds(SiphonPostdelay-.1f);
        _lineRenderer.enabled = false;
    }

    private IEnumerator StartSiphon(Damageable d){
        yield return new WaitForSeconds(SiphonPredelay);
        _isSiphoning = true;
        _siphonTarget = d;
        d.OnDeath += () => _isSiphoning = false;
        d.Tethered = true;
        d.Staggered = false;
    }

    private IEnumerator StopSiphon(){
        yield return new WaitForSeconds(SiphonPostdelay);
        _isSiphoning = false;
        _siphonTarget.Tethered = false;
        _siphonTarget.Staggered = false;
    }

    private void UpdateShooting()
    {
        _coolDownTime -= Time.deltaTime;

        if(_isSiphoning) return;

        if(Input.GetAxisRaw("Fire") > .5 && !_fireHold){
            _fireDown = true;
            _fireHold = true;
        }
        if(Input.GetAxisRaw("Fire") < .5 && _fireHold){
            _fireUp = true;
            _fireHold = false;
        }

        // if mouse down on cooldown queue it up, if let go before cooldown unqueue
        if(_fireDown && _coolDownTime > 0)
            _fireQueued = true;
        if(_fireUp && _coolDownTime > 0)
            _fireQueued = false;
        // start charging on click
        if ((_fireDown || _fireQueued) && !_isCharging && !_isDashing && _coolDownTime <= 0)
        {
            _isCharging = true;
            _fireQueued = false;
            _chargeStartTime = Time.time;
        }
        _chargeTime = Time.time - _chargeStartTime;

        // starts effects when charging
        if(_isCharging && !_startChargeEffects && _chargeTime >= MinChargeTime){
            if(ChargingSound){
                _audio.clip = ChargingSound;
                _audio.Play();
            }
            if(ChargingEffect)  _chargeEffect = Instantiate(ChargingEffect, BulletSpawn);
            _startChargeEffects = true;
        }

        // stop charging on mouse up or timer runs out
        if ((_fireUp || _chargeTime > MaxChargeTime) && _isCharging)
        {
            if(_chargeEffect) Destroy(_chargeEffect);
            if(_audio.isPlaying) _audio.Stop();

            _isCharging = false;
            if (_chargeTime > MinChargeTime)
                FireChargeShot(_chargeTime);
            else
                FireShot();
            _startChargeEffects = false;
        }

        _fireUp = false;
        _fireDown = false;
    }

    void FireShot()
    {
        if(ShotSound) _audio.PlayOneShot(ShotSound, .5f);
        if(ShotEffect)  Destroy(Instantiate(ShotEffect, BulletSpawn), 1f);

        _isFiring = true; 
        GameObject shot = Instantiate(Shot, BulletSpawn); // create shot
        Vector3 scale = shot.transform.localScale;
        shot.transform.SetParent(null); // detach from player transform
        shot.transform.localScale = scale;

        shot.GetComponent<Shot>().Damage = ShotDamage; // set damage of shot
        Destroy(shot, ShotDespawnTime); // destroy it for performance reasons
        _coolDownTime = ShotCooldown; // update cooldown so no new rapid shots
        
    }

    void FireChargeShot(float timeCharged)
    {
        if(ChargeShotSound) _audio.PlayOneShot(ChargeShotSound);
        if(ChargeShotEffect) Destroy(Instantiate(ChargeShotEffect, BulletSpawn), 1f);

        _isHeavyFiring = true;
        GameObject shot = Instantiate(ChargeShot, BulletSpawn); // create shot
        Vector3 scale = shot.transform.localScale;
        shot.transform.SetParent(null); // detach from player transform
        shot.transform.localScale = scale;

        // percentage of charge * damage for charged shot
        float damage = timeCharged / MaxChargeTime * ChargeShotDamage;
        shot.GetComponent<Shot>().Damage = damage;// set damage of shot
        Destroy(shot, ShotDespawnTime);// destroy it for performance reasons
        _coolDownTime = ChargeShotCoolDown;// update cooldown so no new rapid shots
    }

    private void UpdateAnimation()
    {
        _isWalking = (Mathf.Abs(_vInput) > 0.1 || Mathf.Abs(_hInput) > 0.1);

       
        Anim.SetBool("Walking", _rb.velocity.magnitude >= .1);
        Anim.SetBool("Dash", _isDashing);
       
        //doesnt enter charge animation when firing in quick succession
        if(_chargeTime > MinChargeTime)
            Anim.SetBool("isCharging", _isCharging);

        if (_isFiring)
        {
             Anim.SetTrigger("Shot");
            _isFiring = false;
        }

        if (_isHeavyFiring)
        {
            Anim.SetTrigger("HeavyShot");
            _isHeavyFiring = false;
        }
    }
}
