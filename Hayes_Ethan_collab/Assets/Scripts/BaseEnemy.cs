using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BaseEnemy : Damageable
{
    public List<EnemyAttackSpawn> AttackSpawns;
    public List<EnemyAttackInfo> Attacks;


    private Dictionary<string, EnemyAttackSpawn> _spawnLookup;
    private Rigidbody _rb;
    private GameObject _player;
    private List<EnemyAttackInfo> _triggeredAttackPool;
    private float _cooldownTimer, _cooldownLength;
    private bool _attacking;

    void Start(){
        InitLookup();

        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        _rb.constraints = RigidbodyConstraints.FreezeRotation;

        
        _player = GameObject.FindGameObjectWithTag("Player");
        _triggeredAttackPool = new List<EnemyAttackInfo>();

        OnDeath += () => Destroy(this.gameObject);
    }

    void InitLookup(){
        // creates the lookup table to find AttackSpawns from name
        _spawnLookup = new Dictionary<string, EnemyAttackSpawn>();
        foreach(EnemyAttackSpawn spawn in AttackSpawns)
            _spawnLookup.Add(spawn.Name, spawn);
    }

    void Update(){
        UpdateRotation();
        UpdateAttack();
    }

    private void UpdateRotation(){
        // points feet towards ground
        Vector3 gravityUp = transform.position.normalized;
		transform.rotation = Quaternion.FromToRotation(transform.up, gravityUp) * transform.rotation;
        _rb.AddForce(transform.position.normalized*-9.81f);
    }

    private void UpdateAttack(){
        _cooldownTimer -= Time.deltaTime;
        if(_attacking || _cooldownTimer > 0)
            return;
        // choses attack based on which are triggered and their priorities
        EnemyAttackInfo chosenAttack = ChooseAttack();
        // sometimes no attacks are triggered so just return
        if(chosenAttack == null)
            return;
        SpawnAttack(chosenAttack);
    }

    private EnemyAttackInfo ChooseAttack(){
        _triggeredAttackPool.Clear();
        // check each attack to see if its triggered, then add to attackpool as many times as priority
        foreach(EnemyAttackInfo a in Attacks){
            float distance = Vector3.Distance(_player.transform.position, transform.position);
            if(a.Trigger.Triggered(distance))
                for(int i = 0; i < a.Trigger.Priority; i++)
                    _triggeredAttackPool.Add(a);
        }
        // no attacks are triggered
        if(_triggeredAttackPool.Count <= 0)
            return null;
        // return random attack from pool
        return _triggeredAttackPool[Random.Range(0,_triggeredAttackPool.Count)];
    }
    private void SpawnAttack(EnemyAttackInfo info){
        Transform parent = transform;
        Attack attackClone = Instantiate(info.Attack, parent);
        _attacking = true;
        // this event is called when the attack finishes, it updates attacking flag and starts cooldown timer
        attackClone.OnAttackFinish += () => {
            _attacking = false;
            _cooldownTimer = info.Cooldown;
        };
        attackClone.OverrideAttackSpawns(_spawnLookup);
    }

    void OnDrawGizmosSelected(){
        // draws spheres in editor to view trigger radiussies
        Gizmos.color = new Color(0,0,1,.2f);
        foreach(EnemyAttackInfo attack in Attacks){
            if(!attack.Trigger.Proximity)
                continue;
            Gizmos.DrawSphere(transform.position, attack.Trigger.MinTriggerRadius);
            Gizmos.DrawSphere(transform.position, attack.Trigger.MaxTriggerRadius);
        }
    }
}

[System.Serializable]
public class AttackTrigger{
    public int Priority = 1;
    public bool AlwaysTriggered;
    public bool Proximity;
    public float MinTriggerRadius;
    public float MaxTriggerRadius;
    public bool Triggered(float distance){
        if(AlwaysTriggered)
            return true;
        if(Proximity)
            return MinTriggerRadius <= distance && distance <= MaxTriggerRadius;
        return false;
    }
}

[System.Serializable]
public class EnemyAttackInfo{
    public Attack Attack;
    public float Cooldown;
    public AttackTrigger Trigger;
}

[System.Serializable]
public class EnemyAttackSpawn{
    public string Name;
    public Transform SpawnTransform;
}

