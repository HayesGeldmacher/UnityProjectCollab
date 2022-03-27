using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Damageable
{
    public Transform BulletSpawn;

    [Header("Movement")]
    public float MovementSpeed;
    public float PlayerTargetRadius;

    [Header("Attacks")]
    public GameObject Projectile;
    public float DespawnTime;
    public float CooldownTime;

    private float _cooldown;
    private GameObject _player;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        BulletSpawn.transform.LookAt(_player.transform);
        _cooldown -= Time.deltaTime;
        if (_cooldown <= 0)
            FireProjectile();
    }

    void FireProjectile()
    {
        GameObject proj = Instantiate(Projectile, BulletSpawn);
        proj.transform.SetParent(null);
        Destroy(proj, DespawnTime);
        _cooldown = CooldownTime;
    }
}
