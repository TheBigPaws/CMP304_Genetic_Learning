using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeScript : MonoBehaviour
{
    //TextMesh storedFoodText;
    public float storedFood = 0f;
    public float simulationLife = 0f;
    public float simulationfitness = 0f;
    public Transform runningOutline;
    //public List<HelperFunctions.humanAttributes> humans = new List<HelperFunctions.humanAttributes>();
    public HelperFunctions.HumanGroupAttributes groupAttributes;
    
    bool running = true;

    public HumanManager humanManager;
    public WolfManager wolfManager;
    public BushManager bushManager;

    // Start is called before the first frame update
    void Start()
    {
        groupAttributes.setup();
    }

    // Update is called once per frame
    void Update()
    {
        if (running) 
        {
           simulationLife += Time.deltaTime;

            if (humanManager.transform.childCount == 0)
            {
                running = false;
                runningOutline.GetComponent<SpriteRenderer>().color = Color.red;
                Debug.Log("Simulation ran for " + simulationLife.ToString() + " seconds.");
                printHumanAttributes();


                bool allFinished = true;

                foreach(HomeScript child in FindObjectsOfType<HomeScript>())
                {
                    if (child.running)
                    {
                        allFinished = false;
                    }
                }

                //evaluate generations and start new one
                if (allFinished)
                {
                    FindObjectOfType<ApplicationDataScript>().startNextGeneration();
                }
            }
        }        
        
    }

    void printHumanAttributes()
    {
        for (int i = 0; i < humanManager.HumansToSpawn; i++)
        {
            //add simulation fitness
            simulationfitness += groupAttributes.humans[i].individualFitness;
            if (groupAttributes.humans[i].alive) { simulationfitness += 300; }

            Debug.Log("   Human " + i.ToString() + " attributes were:\n " +
                      "      ATT:"+ groupAttributes.humans[i].attackPP.ToString()+ "  HP:" + groupAttributes.humans[i].healthPP.ToString()+ "  MS:" + groupAttributes.humans[i].moveSpeedPP.ToString()
                      + "  SS:" + groupAttributes.humans[i].stomachSizePP.ToString() + "  CR:" + groupAttributes.humans[i].carryPP.ToString()
                      + "  \nflee chance:" + groupAttributes.humans[i].fleeChance.ToString() + "  hunt chance:" + groupAttributes.humans[i].huntChance.ToString()
                      + "  \nEat trigger HP:" + groupAttributes.humans[i].eatingTriggerHealthPerc.ToString() + "  Eat trigger hunger:" + groupAttributes.humans[i].eatingTriggerHungerPerc.ToString()
                      );
        }
        Debug.Log("simulation fitness was " + simulationfitness.ToString());
    }
}
