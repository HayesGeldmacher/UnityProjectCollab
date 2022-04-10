using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class selfDestruct : MonoBehaviour
{
    public float countdown;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Destroy(countdown));
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Destroy(float time)
    {
        
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
