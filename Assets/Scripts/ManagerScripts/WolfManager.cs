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
        float spawnRange = transform.parent.localScale.x / 2;
        Instantiate(WolfRef, this.transform).transform.position = this.transform.position + new Vector3(Random.Range(-spawnRange, spawnRange), Random.Range(-spawnRange/5, spawnRange), 0f);
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < WolfToSpawn; i++)
        {
            spawnRandomLocWolf();
        }
    }

    public void ResetSim()
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
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
