using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanManager : MonoBehaviour
{

    public WolfManager wolfManager;
    public BushManager bushManager;
    public HomeScript Home;
    int humanCount = 5;

    public GameObject HumanRef;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < humanCount; i++)
        {
            Instantiate(HumanRef,this.transform).transform.position = Home.transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
