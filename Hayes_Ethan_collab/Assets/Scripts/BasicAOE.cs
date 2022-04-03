using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BasicAOE : Shot
{
    [Header("Area Attack")]
    public float TimeBetweenDamages;
    public bool OnlyDamageOnce;

    private Dictionary<GameObject, float> _timeLastHit;
    private bool _hasHit;

    void Start(){
        _timeLastHit = new Dictionary<GameObject, float>();
    }

    void Update(){
        foreach(GameObject key in _timeLastHit.Keys)
            _timeLastHit[key] -= Time.time;
    }

    private void OnTriggerStay(Collider other){
        float now = Time.time;
        if(_timeLastHit.TryGetValue(other.gameObject, out float timeLastHit)){
            if(OnlyDamageOnce)
                return;
            if(now-timeLastHit >= TimeBetweenDamages){
                _timeLastHit[other.gameObject] = now;
                OnHit(other.gameObject);
            }
        }else{
            _timeLastHit.Add(other.gameObject, now);
            OnHit(other.gameObject);
        }
        
    }
}
