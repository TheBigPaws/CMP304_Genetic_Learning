using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodSource : MonoBehaviour
{
    public float FoodAmount;
    public bool isBush = false;

    public Transform numberT;

    // Start is called before the first frame update
    void Start()
    {
        
    }

   public float extractFood(float request)
    {
        float food_ret = FoodAmount;

        if(request < FoodAmount)
        {
            FoodAmount -= request;
            HelperFunctions.spawnText(transform.position, "+" + request, IconType.food);

            return request;
        }

        //return all that's left in this node
        FoodAmount = 0;
        HelperFunctions.spawnText(transform.position, "+" + food_ret, IconType.food);
        return food_ret;
    }

    // Update is called once per frame
    void Update()
    {
        

        if (FoodAmount <= 0)
        {
            if (this.transform.parent.childCount < 5)
            {
                this.GetComponentInParent<BushManager>().spawnRandomLocBush();
            }
            Destroy(this.gameObject);
        }
    }
}
