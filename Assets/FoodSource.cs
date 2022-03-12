using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSource : MonoBehaviour
{
    public float FoodAmount;
    public bool isBush = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

   public float extractFood()
    {

        float extractRate = 1;
        float extracted = Time.deltaTime * extractRate;

        FoodAmount -= extracted;

        return extracted;

        
    }

    // Update is called once per frame
    void Update()
    {
        

        if (FoodAmount <= 0)
        {
            if (isBush)
            {
                this.GetComponentInParent<BushManager>().spawnRandomLocBush();
            }
            Destroy(this.gameObject);
        }
    }
}
