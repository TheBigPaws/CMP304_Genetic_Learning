using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeScript : MonoBehaviour
{
    //TextMesh storedFoodText;
    public float storedFood = 0f;
    public float simulationLife = 0f;

    public List<HumanScript> humans;
    bool running = true;

    public HumanManager humanManager;
    public WolfManager wolfManager;
    public BushManager bushManager;

    // Start is called before the first frame update
    void Start()
    {
        //storedFoodText = Instantiate<TextMesh>(new TextMesh());
        //storedFoodText.transform.position = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (running)
        {
           simulationLife += Time.deltaTime;
        }

        //storedFoodText.text = storedFood.ToString();
        
        if(humanManager.transform.childCount == 0)
        {
            running = false;


            //while (wolfManager.transform.childCount > 0)
            //{
            //    Destroy(wolfManager.transform.GetChild(0).gameObject);
            //}
            //while (bushManager.transform.childCount > 0)
            //{
            //    Destroy(bushManager.transform.GetChild(0).gameObject);
            //}
            Debug.Log("Simulation ran for " + simulationLife.ToString() + " seconds.");
            //printHumanAttributes();
        }
        
    }

    void printHumanAttributes()
    {
        for (int i = 0; i < 5; i++)
        {
            Debug.Log("   Human " + i.ToString() + " attributes were:\n " +
                      "      ATT:"+humans[i].attributes.attackPP.ToString()+ "  HP:" + humans[i].attributes.healthPP.ToString()+ "  MS:" +humans[i].attributes.moveSpeedPP.ToString()
                      + "  SS:" + humans[i].attributes.stomachSizePP.ToString() + "  CR:" + humans[i].attributes.carryPP.ToString()
                      );
        }
    }
}
