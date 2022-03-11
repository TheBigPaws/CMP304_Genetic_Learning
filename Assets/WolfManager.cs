using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfManager : MonoBehaviour
{

    public HumanManager humanManager;

    public GameObject WolfRef;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            Instantiate(WolfRef, this.transform).transform.position = this.transform.position + new Vector3(Random.Range(-10f, 10f), Random.Range(-2f, 10f), 0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
