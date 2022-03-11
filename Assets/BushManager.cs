using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BushManager : MonoBehaviour
{

    public GameObject BushRef;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            Instantiate(BushRef, this.transform).transform.position = this.transform.parent.position + new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), -1f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
