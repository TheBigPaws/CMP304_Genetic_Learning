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
            bestGenerations.Add(new HelperFunctions.HumanGroupAttributes());
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
        }

        //evaluate generations and start new one
        if (allFinished)
        {
            startNextGeneration();
        }

    }
    public void evaluateGenerations()
    {
        //go thru all homes in scene
        foreach (HomeScript child in FindObjectsOfType<HomeScript>())
        {
            for (int i = 0; i < BestGenerationStore; i++)
            {
                //if given home is better than one of stored values
                if(bestGenerations[i].getGroupFitness() < child.groupAttributes.getGroupFitness())
                {
                    //shift everything to the right
                    for(int j = BestGenerationStore - 1; j > i ; j--)
                    {
                        bestGenerations[j] = bestGenerations[j-1];
                    }
                    //assign new bestGenValue
                    bestGenerations[i] = child.groupAttributes;
                    break;
                }
            }
        }

            Debug.Log("Best group fitnesses were");
        for (int i = 0; i < BestGenerationStore; i++)
        {
            Debug.Log((1+i).ToString() +": "+ bestGenerations[i].getGroupFitness().ToString());
        }
    }
    public void startNextGeneration()
    {
        int i = 0;
        generation++;
        //FindObjectOfType<UI_script>().GenerationCount.text = "Generation " + generation.ToString();
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
