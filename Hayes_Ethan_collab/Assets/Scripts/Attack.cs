using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public AttackSpawn[] AttackSpawns;
    public AttackInfo[] Projectiles;
    public AttackTrigger Triggers;


    private Dictionary<string, AttackSpawn> _attackSpawnLookup;
    private float[] _projectileCooldowns;
    private int[] _timesFired;
    void Start(){
        InitLookup();
        InitCooldowns();
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
        UpdateAttackSpawns();
        UpdateProjectiles();
    }

    void UpdateAttackSpawns(){
        foreach(AttackSpawn spawn in AttackSpawns){
            if(spawn.TrackPlayer){

            }else
                spawn.SpawnTransform.Rotate(spawn.RotationSpeed * Time.deltaTime);
        }
    }

    void UpdateProjectiles(){
        if(!Triggered())
            return;
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

    public bool Triggered(){
        // TODO: trigger logic
        return true;
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

[System.Serializable]
public class AttackTrigger{
    public bool AlwaysTriggered;
    public bool Proximity;
    public float MinTriggerRadius;
    public float MaxTriggerRadius;
    public bool Triggered(float distance){
        // TODO: huge bug with triggering attacks
        if(AlwaysTriggered)
            return true;
        if(Proximity)
            return MinTriggerRadius <= distance && distance <= MaxTriggerRadius;
        return false;
    }
}