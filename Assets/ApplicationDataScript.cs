using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationDataScript : MonoBehaviour
{
    public float ElapsedTime = 0f;
    public GameObject popT;
    // Start is called before the first frame update
    void Start()
    {
     
    }
    
    // Update is called once per frame
    void Update()
    {
        ElapsedTime += Time.deltaTime;

        
    }


}
