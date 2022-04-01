using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public delegate void AttackFinishHandler();
    public AttackFinishHandler OnAttackFinish;

    public AttackSpawn[] AttackSpawns;
    public AttackInfo[] Projectiles;

    private Transform _player;
    private Dictionary<string, AttackSpawn> _attackSpawnLookup;
    private float[] _projectileCooldowns;
    private int[] _timesFired;
    void Start(){
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        InitLookup();
        InitCooldowns();
        foreach(AttackSpawn spawn in AttackSpawns)
            if(spawn.TrackPlayer)
                spawn.SpawnTransform.LookAt(_player.transform);
    }

    void InitLookup(){
        // creates lookup for attack spawns given their name
        _attackSpawnLookup = new Dictionary<string, AttackSpawn>();
        foreach(AttackSpawn spawn in AttackSpawns)
            _attackSpawnLookup.Add(spawn.Name, spawn);
    }

    void InitCooldowns(){
        // sets the predelay for all projectiles
        _timesFired = new int[Projectiles.Length];

        _projectileCooldowns = new float[Projectiles.Length];
        for(int i = 0; i < Projectiles.Length; i++){
            _projectileCooldowns[i] = Projectiles[i].Predelay;
        }
    }

    void Update(){
        for(int i = 0; i < Projectiles.Length; i++)
            _projectileCooldowns[i] -= Time.deltaTime;
        CheckFinished();
        UpdateAttackSpawns();
        UpdateProjectiles();
    }
    
    public void OverrideAttackSpawns(Dictionary<string, EnemyAttackSpawn> enemyLookup){
        // TODO: if this is ever a problem being O(n^2) then refactor this but this is the only way i could get to work
        foreach(string name in enemyLookup.Keys)
            foreach(AttackSpawn spawn in AttackSpawns)
                if(spawn.Name == name)
                    spawn.SpawnTransform = enemyLookup[name].SpawnTransform;

        InitLookup();
    }

    private void CheckFinished(){
        // TODO: update this to take into account things like lazers and despawn time

        // if all projectiles have fired as many times as their supposed to then call the event
        for(int i = 0; i < Projectiles.Length; i++)
            if(Projectiles[i].Repeats != _timesFired[i])
                return;
        OnAttackFinish();
        Destroy(gameObject);
    }

    void UpdateAttackSpawns(){
        foreach(AttackSpawn spawn in AttackSpawns){
            // tracks the spawn transform to point forward towards player
            if(spawn.TrackPlayer){
                Vector3 towardPlayer = _player.position - spawn.SpawnTransform.position;
                Quaternion desiredRotation = Quaternion.FromToRotation(spawn.SpawnTransform.forward, towardPlayer)*spawn.SpawnTransform.rotation;
                spawn.SpawnTransform.rotation = Quaternion.RotateTowards(spawn.SpawnTransform.rotation, desiredRotation, spawn.PlayerTrackStrength*Time.deltaTime);
            // handles spawn transform rotation when not tracking player
            }else
                spawn.SpawnTransform.Rotate(spawn.RotationSpeed * Time.deltaTime);
        }
    }

    void UpdateProjectiles(){
        // checks to fire all projectiles based on their cool down and how many have already been fired
        for(int i = 0; i < Projectiles.Length; i++)
            if(_projectileCooldowns[i] <= 0 && _timesFired[i]<Projectiles[i].Repeats){
                SpawnProjectile(Projectiles[i]);
                _projectileCooldowns[i] += Projectiles[i].TimeBetweenShots;
                _timesFired[i]++;
            }
            
    }

    void SpawnProjectile(AttackInfo proj){
        // spawn the proj in all of its spawns
        foreach(string name in proj.AttackSpawnNames){
            GameObject shot = Instantiate(proj.Shot, _attackSpawnLookup[name].SpawnTransform);
            if(!proj.LockToSpawn)
                shot.transform.parent = null;
            Destroy(shot, proj.DespawnTime);
        }

    }
}

[System.Serializable]
public class AttackSpawn{
    public string Name;
    public Transform SpawnTransform;
    public bool TrackPlayer;
    public float PlayerTrackStrength;
    public Vector3 RotationSpeed;
}

[System.Serializable]
public class AttackInfo{
    public GameObject Shot;
    public string[] AttackSpawnNames;
    public float Predelay;
    public int Repeats;
    public float TimeBetweenShots;
    public float DespawnTime;
    public bool LockToSpawn;
}