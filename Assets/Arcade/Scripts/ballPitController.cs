using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ballPitController : MonoBehaviour
{

    public GameObject bouncyBalls;

    public GameObject bouncyPit;

    private bool bnought = false;

    public int numbOfBalls = 9;

    public int[] spawnRange = {-2,-1};

    public float spawnheight = 0;

    public float spawnwidth = -2;

    private float spawnTimer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        spawnBalls();
        
    }

    void spawnBalls()
    {

        if(bnought == false)
        {
            
            for( int i = 0; i < numbOfBalls; i+=1)
            {
                for(int j = 0;  j < numbOfBalls; j+=1)
                {
                    Vector3 randomSpawnPosition = new Vector3(transform.position.x + j, transform.position.y + spawnheight, transform.position.z + i);
                    Instantiate(bouncyBalls, randomSpawnPosition, Quaternion.identity);
                    if(spawnheight == 8)
                {
                        spawnheight = 0;
                    }
                    spawnheight = spawnheight+2;
                }

            }
            
        }
        bnought = true;
    }
}
