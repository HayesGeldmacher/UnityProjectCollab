using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowingLazerAOE : BasicAOE
{
    public float growthRate;
    public override void Update()
    {
        Vector3 s = transform.localScale;
        // some really weird stuff to make the lazer grow only in one direction and also face towards the attack spawn
        transform.position += transform.up * growthRate * Time.deltaTime;
        transform.localScale = new Vector3(s.x, transform.localPosition.z, s.z);
    }
}
