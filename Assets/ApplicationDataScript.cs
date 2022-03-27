using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ApplicationDataScript : MonoBehaviour
{
    public float ElapsedTime = 0f;
    public int generation = 1;
    public GameObject popT;

    public int RandomGenerationsAmount = 2;
    public int BestGenerationStore = 3;

    List<HelperFunctions.HumanGroupAttributes> bestGenerations = new List<HelperFunctions.HumanGroupAttributes>();

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < BestGenerationStore; i++) {
            HelperFunctions.HumanGroupAttributes temp = new HelperFunctions.HumanGroupAttributes();
            temp.setup();
            bestGenerations.Add(temp);
            //bestGenerations.
            //Debug.Log(bestGenerations[i].getGroupFitness());
        }

        
    }
    
    // Update is called once per frame
    void Update()
    {
        ElapsedTime += Time.deltaTime;


        bool allFinished = true;

        foreach (HomeScript child in FindObjectsOfType<HomeScript>())
        {
            if (child.running)
            {
                allFinished = false;
                break;
            }
            //else { Debug.Log(child.groupAttributes.humans.Count); }
        }

        //evaluate generations and start new one
        if (allFinished)
        {
            startNextGeneration();
        }

    }
    public void evaluateGenerations()
    {
        foreach (HomeScript child in FindObjectsOfType<HomeScript>())
        {
            child.groupAttributes.CalculateGroupFitness();
            for (int i = 0; i < BestGenerationStore; i++)
            {
        
                if (bestGenerations[i].GroupFitness < child.groupAttributes.GroupFitness)
                {

                    Debug.Log("replacing generation rank " + (i + 1).ToString() + "with fitness " + bestGenerations[i].GroupFitness.ToString() + "with fitness " + child.groupAttributes.GroupFitness.ToString());

                    //shift everything to the right 
                    for (int j = BestGenerationStore - 1; j > i; j--)
                    {
                        bestGenerations[j] = bestGenerations[j - 1];
                    }

                    bestGenerations[i] = child.groupAttributes;
                    break;
                }
            }
        }

        Debug.Log("Best group fitnesses were");
        for (int i = 0; i < BestGenerationStore; i++)
        {
            Debug.Log(bestGenerations[i].GroupFitness);
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
            }
            else
            {
                int generationToUse = (i - RandomGenerationsAmount) % BestGenerationStore;
                HelperFunctions.HumanGroupAttributes temp = bestGenerations[generationToUse];
                temp.shiftAttributes();
                child.humanManager.startNextGeneration(temp);
            }

            i++;
        }
    }
}
