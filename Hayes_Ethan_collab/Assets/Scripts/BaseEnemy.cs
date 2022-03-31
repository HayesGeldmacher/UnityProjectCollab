using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : Damageable
{
    public List<Attack> Attacks;

    private GameObject _player;
    private List<Attack> _triggeredAttackPool;
    private Attack _chosenAttack;
    private float _cooldownTimer;

    void Start(){
        _player = GameObject.FindGameObjectWithTag("Player");
        _triggeredAttackPool = new List<Attack>();

        OnDeath += () => Destroy(this.gameObject);
    }

    void Update(){
        UpdateAttack();
    }

    private void UpdateAttack(){
        // if(_chosenAttack != null && _chosenAttack.TrackPlayerStrength != 0){
        //     Quaternion currentRotation = _chosenAttack.AttackSpawn.rotation;
        //     Vector3 direction = _player.transform.position - transform.position;
        //     Quaternion desiredRotation = Quaternion.FromToRotation(_chosenAttack.AttackSpawn.forward, direction) * _chosenAttack.AttackSpawn.rotation;
            
        //     _chosenAttack.AttackSpawn.rotation = Quaternion.Slerp(currentRotation, desiredRotation, _chosenAttack.TrackPlayerStrength/100);
        // }
            

        // _cooldownTimer -= Time.deltaTime;
        // if(_cooldownTimer > 0)
        //     return;

        // _triggeredAttackPool.Clear();

        // float distance = Vector3.Distance(_player.transform.position, transform.position);
        // foreach(Attack attack in Attacks)
        //     if(attack.Trigger.Triggered(distance))
        //         for(int i = 0; i < attack.Priority; i++)
        //             _triggeredAttackPool.Add(attack);
        //     else
        //         Debug.Log("no trigger");

        // if(_triggeredAttackPool.Count != 0) {
        //     _chosenAttack = Attacks[Random.Range(0,Attacks.Count)];
        //     if(_chosenAttack.PointTowardsPlayer)
        //         _chosenAttack.AttackSpawn.LookAt(_player.transform);
        //     GameObject a = Instantiate(_chosenAttack.AttackPrefab, _chosenAttack.AttackSpawn);
        //     if(_chosenAttack.TrackPlayerStrength == 0)
        //         a.transform.SetParent(null);
        //     _cooldownTimer = _chosenAttack.Cooldown;
        // }
    }

    // void OnDrawGizmosSelected(){
    //     Gizmos.color = new Color(1,0,0,.2f);
    //     foreach(Attack attack in Attacks) {
    //         Gizmos.DrawSphere(transform.position, attack.Trigger.MinTriggerRadius);
    //         Gizmos.DrawSphere(transform.position, attack.Trigger.MaxTriggerRadius);
    //     }
    // }
}


/**

CURRENT SYSTEM FOR TRIGGERING ATTACKS

AttackPrefab - the prefab for ur attack duh

AttackSpawn - where to instantiate the prefab

PointTowardsPlayer - if checked, points the AttackSpawn transform towards player before attacking

Cooldown - how many seconds has to wait after completing this attack

Priority - if multiple attacks can be triggered to fire, this determines which one is fired.
           attack with 2 priority is twice more likely to fire than attack with 1 priority.
           attack with 3 priority is thrice more likely to fire than attack with 3 priority.
           etc.

TRIGGERS

AlwaysTriggered - always available to use this attack

Proximity - this attack can only be used if the player is within the min and max radius specified.

**/



