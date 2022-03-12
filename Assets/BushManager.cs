using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BushManager : MonoBehaviour
{

    public GameObject BushRef;

    public void spawnRandomLocBush()
    {
        GameObject go = Instantiate(BushRef, this.transform);
        go.transform.position = this.transform.position + new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), 0f);

    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            spawnRandomLocBush();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
