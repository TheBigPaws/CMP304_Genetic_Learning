using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationDataScript : MonoBehaviour
{
    public float ElapsedTime = 0f;
    public int generation = 0;
    public GameObject popT;

    public int RandomGenerationsAmount = 2;
    public int BestGenerationStore = 3;

    List<HelperFunctions.HumanGroupAttributes> bestGenerations = new List<HelperFunctions.HumanGroupAttributes>();

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < BestGenerationStore; i++) { 
            bestGenerations.Add(new HelperFunctions.HumanGroupAttributes());
        }

        
    }
    
    // Update is called once per frame
    void Update()
    {
        ElapsedTime += Time.deltaTime;

        


    }
    public void evaluateGenerations()
    {
        foreach (HomeScript child in FindObjectsOfType<HomeScript>())
        {
            for (int i = 0; i < BestGenerationStore; i++)
            {

                if(bestGenerations[i].groupFitness < child.groupAttributes.getGroupFitness())
                {
                    for(int j = BestGenerationStore - 1; j > i ; j--)
                    {
                        bestGenerations[j] = bestGenerations[j-1];
                    }
                    bestGenerations[i] = child.groupAttributes;
                    break;
                }
            }
        }

            Debug.Log("Best group fitnesses were");
        for (int i = 0; i < BestGenerationStore; i++)
        {
            Debug.Log((1+i).ToString() +": "+ bestGenerations[i].groupFitness.ToString());
        }
    }
    public void startNextGeneration()
    {
        int i = 0;
        evaluateGenerations();
        foreach (HomeScript child in FindObjectsOfType<HomeScript>())
        {


            child.resetSim();

            //choose whether to use a shifted version of best fitness generations or random
            if (i < RandomGenerationsAmount)
            {
                child.humanManager.spawnRandomHumans();
                //Debug.Log(child.name + " has been reset to random values");
            }
            else
            {
                int generationToUse = (i - RandomGenerationsAmount) % BestGenerationStore;
                HelperFunctions.HumanGroupAttributes temp = bestGenerations[generationToUse];
                temp.shiftAttributes();
                child.humanManager.startNextGeneration(temp);
                //Debug.Log(child.name + " has been set to shifted values of generation #"+generationToUse.ToString());

            }

            i++;
        }
    }
}
