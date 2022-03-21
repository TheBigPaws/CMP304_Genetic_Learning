using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfManager : MonoBehaviour
{
    public int WolfToSpawn = 5;

    public HumanManager humanManager;

    public GameObject WolfRef;

    public void spawnRandomLocWolf()
    {
        Instantiate(WolfRef, this.transform).transform.position = this.transform.position + new Vector3(Random.Range(-10f, 10f), Random.Range(-2f, 10f), 0f);
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < WolfToSpawn; i++)
        {
            spawnRandomLocWolf();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
