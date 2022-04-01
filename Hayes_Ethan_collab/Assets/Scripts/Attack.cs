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
        _attackSpawnLookup = new Dictionary<string, AttackSpawn>();
        foreach(AttackSpawn spawn in AttackSpawns)
            _attackSpawnLookup.Add(spawn.Name, spawn);
    }

    void InitCooldowns(){
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

    private void CheckFinished(){
        // TODO: update this to take into account things like lazers and despawn time
        for(int i = 0; i < Projectiles.Length; i++)
            if(Projectiles[i].Repeats != _timesFired[i])
                return;
        OnAttackFinish();
        Destroy(gameObject);
    }

    void UpdateAttackSpawns(){
        foreach(AttackSpawn spawn in AttackSpawns){
            if(spawn.TrackPlayer){
                Vector3 towardPlayer = _player.position - spawn.SpawnTransform.position;
                Quaternion desiredRotation = Quaternion.FromToRotation(spawn.SpawnTransform.forward, towardPlayer)*spawn.SpawnTransform.rotation;
                spawn.SpawnTransform.rotation = Quaternion.RotateTowards(spawn.SpawnTransform.rotation, desiredRotation, spawn.PlayerTrackStrength*Time.deltaTime);
            }else
                spawn.SpawnTransform.Rotate(spawn.RotationSpeed * Time.deltaTime);
        }
    }

    void UpdateProjectiles(){
        for(int i = 0; i < Projectiles.Length; i++)
            if(_projectileCooldowns[i] <= 0 && _timesFired[i]<Projectiles[i].Repeats){
                SpawnProjectile(Projectiles[i]);
                _projectileCooldowns[i] += Projectiles[i].TimeBetweenShots;
                _timesFired[i]++;
            }
            
    }

    void SpawnProjectile(AttackInfo proj){
        foreach(string spawn in proj.AttackSpawnNames){
            GameObject shot = Instantiate(proj.Shot, _attackSpawnLookup[spawn].SpawnTransform);
            if(!proj.LockToSpawn)
                shot.transform.parent = null;
            // TODO: deparent, variable destroy times
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