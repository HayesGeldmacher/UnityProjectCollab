using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BaseEnemy : Damageable
{
    
    public List<EnemyAttackInfo> Attacks;


    private Rigidbody _rb;
    private GameObject _player;
    private List<EnemyAttackInfo> _triggeredAttackPool;
    private float _cooldownTimer, _cooldownLength;
    private bool _attacking;

    void Start(){
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        _rb.constraints = RigidbodyConstraints.FreezeRotation;

        
        _player = GameObject.FindGameObjectWithTag("Player");
        _triggeredAttackPool = new List<EnemyAttackInfo>();

        OnDeath += () => Destroy(this.gameObject);
    }

    void Update(){
        UpdateRotation();
        UpdateAttack();
    }

    private void UpdateRotation(){
        Vector3 gravityUp = transform.position.normalized;
		transform.rotation = Quaternion.FromToRotation(transform.up, gravityUp) * transform.rotation;
        _rb.AddForce(transform.position.normalized*-9.81f);
    }

    private void UpdateAttack(){
        _cooldownTimer -= Time.deltaTime;
        if(_attacking || _cooldownTimer > 0)
            return;

        EnemyAttackInfo chosenAttack = ChooseAttack();
        if(chosenAttack == null)
            return;
        SpawnAttack(chosenAttack);
        
    }

    private EnemyAttackInfo ChooseAttack(){
        _triggeredAttackPool.Clear();
        foreach(EnemyAttackInfo a in Attacks){
            float distance = Vector3.Distance(_player.transform.position, transform.position);
            if(a.Trigger.Triggered(distance))
                for(int i = 0; i < a.Trigger.Priority; i++)
                    _triggeredAttackPool.Add(a);
        }
        if(_triggeredAttackPool.Count <= 0)
            return null;
        return _triggeredAttackPool[Random.Range(0,_triggeredAttackPool.Count)];
    }
    private void SpawnAttack(EnemyAttackInfo attack){
        // TODO: do this method right
        Attack attackClone = Instantiate(attack.Attack, transform);
        _attacking = true;
        attackClone.OnAttackFinish += () => {
            Debug.Log("ATTACK FINISHED");
            _attacking = false;
            _cooldownTimer = attack.Cooldown;
        };
    }

    void OnDrawGizmosSelected(){
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

