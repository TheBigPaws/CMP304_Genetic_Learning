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
                //Debug.Log("Simulation ran for " + simulationLife.ToString() + " seconds.");
                //printHumanAttributes();


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


    public void resetSim()
    {
        runningOutline.GetComponent<SpriteRenderer>().color = Color.green;
        bushManager.ResetSim();
        wolfManager.ResetSim();
        running = true;
        simulationLife = 0.0f;
    }
}
