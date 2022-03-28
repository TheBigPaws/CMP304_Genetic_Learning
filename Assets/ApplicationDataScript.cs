using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationDataScript : MonoBehaviour
{
    public float ElapsedTime = 0f;
    public int generation;
    public GameObject popT;

    public int RandomGenerationsAmount = 10;
    public int BestGenerationStore = 5;

    List<HelperFunctions.HumanGroupAttributes> bestGenerations = new List<HelperFunctions.HumanGroupAttributes>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < BestGenerationStore; i++)
        {
            HelperFunctions.HumanGroupAttributes temp = new HelperFunctions.HumanGroupAttributes();
            temp.setup();
            bestGenerations.Add(temp);
        }

        generation = 1;
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
            generation++;
            FindObjectOfType<UI_script>().GenerationCount.text = "Generation " + generation.ToString();
            startNextGeneration();

            switch (generation)
            {
                case 2:
                    RandomGenerationsAmount = 7;
                    break;
                case 4:
                    RandomGenerationsAmount = 5;
                    break;
                case 7:
                    RandomGenerationsAmount = 2;
                    break;

            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Application.LoadLevel(Application.loadedLevel);
            Time.timeScale = 1;
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

                    //Debug.Log("replacing generation rank " + (i + 1).ToString() + "with fitness " + bestGenerations[i].GroupFitness.ToString() + "with fitness " + child.groupAttributes.GroupFitness.ToString());

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

        //Debug.Log("Best group fitnesses AFTER EVALUATE were");
        //for (int i = 0; i < BestGenerationStore; i++)
        //{
        //
        //    Debug.Log(bestGenerations[i].GroupFitness);
        //}
    }
    public void startNextGeneration()
    {


        //compare fitnesses of simulations
        evaluateGenerations();




        int randGenCount = 0;
        foreach (HomeScript child in FindObjectsOfType<HomeScript>())
        {
            child.resetSim();

            //choose whether to use a shifted version of best fitness generations or random
            if (randGenCount < RandomGenerationsAmount)
            {
                child.humanManager.spawnRandomHumans();
            }
            else
            {

                int genAidx = 0;
                int genBidx = 0;

                while (genAidx == genBidx)
                {
                    genAidx = Random.Range(0, bestGenerations.Count);
                    genBidx = Random.Range(0, bestGenerations.Count);
                }

                HelperFunctions.HumanGroupAttributes temp = HelperFunctions.CombineGenerations(bestGenerations[genAidx], bestGenerations[genBidx]);

                temp.shiftAttributes();

                child.humanManager.startNextGeneration(temp);

            }

            randGenCount++;


        }


        //ui tings
        int wolvesKilled = 0;
        float foodGathered = 0;
        float AverageLifeSpan = 0;

        for (int l = 0; l < bestGenerations[0].humans.Count; l++)
        {
            wolvesKilled += bestGenerations[0].humans[l].wolvesKilled;
            foodGathered += bestGenerations[0].humans[l].foodGathered;
            AverageLifeSpan += bestGenerations[0].humans[l].timeSurvived;
        }

        AverageLifeSpan /= bestGenerations[0].humans.Count;

        FindObjectOfType<UI_script>().BestFitness.text = ("#1 Generation Fitness: " + bestGenerations[0].GroupFitness);
        FindObjectOfType<UI_script>().BestStats.text = ("Wolves killed: " + wolvesKilled.ToString() + "\nFood gathered: " + foodGathered.ToString() + "\nAverage life span: " + AverageLifeSpan.ToString());

        HelperFunctions.RecordText(bestGenerations[0].GroupFitness.ToString());
        

    }
}
