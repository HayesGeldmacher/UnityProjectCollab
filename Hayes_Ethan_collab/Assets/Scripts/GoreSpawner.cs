using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoreSpawner : MonoBehaviour
{

    public GameObject chunk;
    
    // Start is called before the first frame update
    void Start()
    {
        SpawnGore(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void SpawnGore()
    {
       
        //spawns random nuumber of gore chunks upon being created
        int ChunkNumber = Random.Range(2, 5);
        for(int i = 0; i < ChunkNumber; i++)
        {
           Instantiate(chunk, transform.position, transform.rotation);

        }
        
    }
}
