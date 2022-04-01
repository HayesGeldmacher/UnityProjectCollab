using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : Damageable
{
    
    public List<EnemyAttackInfo> Attacks;

    private GameObject _player;
    private List<EnemyAttackInfo> _triggeredAttackPool;
    private float _cooldownTimer, _cooldownLength;
    private bool _attacking;

    void Start(){
        _player = GameObject.FindGameObjectWithTag("Player");
        _triggeredAttackPool = new List<EnemyAttackInfo>();

        OnDeath += () => Destroy(this.gameObject);
    }

    void Update(){
        UpdateAttack();
    }

    private void UpdateAttack(){
        _cooldownTimer -= Time.deltaTime;
        if(_attacking || _cooldownTimer > 0)
            return;

        _triggeredAttackPool.Clear();
        foreach(EnemyAttackInfo a in Attacks){
            if(a.Trigger.Triggered(0f))
                for(int i = 0; i < a.Trigger.Priority; i++)
                    _triggeredAttackPool.Add(a);

        EnemyAttackInfo chosenAttack = _triggeredAttackPool[Random.Range(0,_triggeredAttackPool.Count)];
        SpawnAttack(chosenAttack);
        }
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
}

[System.Serializable]
public class AttackTrigger{
    public int Priority = 1;
    public bool AlwaysTriggered;
    public bool Proximity;
    public float MinTriggerRadius;
    public float MaxTriggerRadius;
    public bool Triggered(float distance){
        return true;
        // // TODO: huge bug with triggering attacks
        // if(AlwaysTriggered)
        //     return true;
        // if(Proximity)
        //     return MinTriggerRadius <= distance && distance <= MaxTriggerRadius;
        // return false;
    }
}

[System.Serializable]
public class EnemyAttackInfo{
    public Attack Attack;
    public float Cooldown;
    public AttackTrigger Trigger;
}

