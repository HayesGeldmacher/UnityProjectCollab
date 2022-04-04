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
    public int _totalProjectiles;
    public int _numProjectilesSpawned;
    void Awake(){
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        InitLookup();
        InitCooldowns();
        foreach(AttackInfo shot in Projectiles)
            _totalProjectiles += shot.AttackSpawnNames.Length*shot.Repeats;
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
        UpdateAttackSpawns();
    }

    private void CheckFinished(){
        // if all projectiles have fired as many times as their supposed to then call the event
        if(_numProjectilesSpawned != _totalProjectiles)
            return;
        if(OnAttackFinish != null)
            OnAttackFinish();
        Destroy(gameObject);
    }

    void UpdateAttackSpawns(){
        foreach(AttackSpawn spawn in AttackSpawns){
            // tracks the spawn transform to point forward towards player
            if(spawn.TrackPlayer){
                // TODO: fix this because i broke it
                Vector3 towardPlayer = _player.position - spawn.SpawnTransform.position;
                Quaternion desiredRotation = Quaternion.FromToRotation(spawn.SpawnTransform.forward, towardPlayer)*spawn.SpawnTransform.rotation;
                spawn.SpawnTransform.rotation = desiredRotation;
                //spawn.SpawnTransform.rotation = Quaternion.RotateTowards(spawn.SpawnTransform.rotation, desiredRotation, spawn.PlayerTrackStrength*Time.deltaTime);
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

    private IEnumerator DecrementSpawnCount(float time){
        yield return new WaitForSeconds(time);
        _numProjectilesSpawned++;
    }

    void SpawnProjectile(AttackInfo proj){
        // spawn the proj in all of its spawns
        foreach(string name in proj.AttackSpawnNames){
            GameObject shot = Instantiate(proj.Shot, _attackSpawnLookup[name].SpawnTransform);
            if(!proj.LockToSpawn)
                shot.transform.parent = null;
            StartCoroutine(DecrementSpawnCount(proj.DespawnTime));
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
    public int Repeats = 1;
    public float TimeBetweenShots;
    public float DespawnTime = 1;
    public bool LockToSpawn;
}