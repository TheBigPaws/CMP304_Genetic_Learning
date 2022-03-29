using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ApplicationDataScript : MonoBehaviour
{
    public float ElapsedTime = 0f;
    public int generation;
    public GameObject popT;

    int simNR = 0;

    public int RandomGenerationsAmount = 10;
    public int BestGenerationStore = 5;
    public int BestHumanStore = 5;


    List<HelperFunctions.HumanGroupAttributes> bestGenerations = new List<HelperFunctions.HumanGroupAttributes>();
    List<HelperFunctions.humanAttributes> bestHumans = new List<HelperFunctions.humanAttributes>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < BestGenerationStore; i++)
        {
            HelperFunctions.HumanGroupAttributes temp = new HelperFunctions.HumanGroupAttributes();
            temp.setup();
            bestGenerations.Add(temp);
        }

        for (int i = 0; i < BestHumanStore; i++)
        {
            HelperFunctions.humanAttributes temp_h = new HelperFunctions.humanAttributes();
            temp_h.individualFitness = 0;
            bestHumans.Add(temp_h);
        }

        generation = 1;
    }

    // Update is called once per frame
    void FixedUpdate()
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
            

            if(generation == 15) { resetSim();return; }

            startNextGeneration();
            generation++;
            FindObjectOfType<UI_script>().GenerationCount.text = "Generation " + generation.ToString();
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
            resetSim();
            Time.timeScale = 1;
        }

    }
    public void evaluateGenerations()
    {

        //HelperFunctions.RecordText("---- sim "+simNR.ToString()+"-----------GENERATION " + generation.ToString());

        foreach (HomeScript child in FindObjectsOfType<HomeScript>())
        {
            child.groupAttributes.CalculateGroupFitness();
            for (int l = 0; l < child.groupAttributes.humans.Count; l++)
            {
                for (int i = 0; i < BestHumanStore; i++)
                {
                    if (bestHumans[i].individualFitness < child.groupAttributes.humans[l].individualFitness)
                    {

                        //Debug.Log("replacing generation rank " + (i + 1).ToString() + "with fitness " + bestGenerations[i].GroupFitness.ToString() + "with fitness " + child.groupAttributes.GroupFitness.ToString());

                        //shift everything to the right 
                        for (int j = BestHumanStore - 1; j > i; j--)
                        {
                            bestHumans[j] = bestHumans[j - 1];
                        }

                        bestHumans[i] = child.groupAttributes.humans[l];

                        break;
                    }
                }


            }


            for (int i = 0; i < BestGenerationStore; i++)
            {

                if (bestGenerations[i].GroupFitness < child.groupAttributes.GroupFitness)
                {


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

        string outputTXT = "";
        string bestHTXT = "";

        //Debug.Log("Best group fitnesses AFTER EVALUATE were");
        for (int i = 0; i < 5; i++)
        {

            outputTXT += bestGenerations[i].GroupFitness.ToString() + " ";
            bestHTXT += bestHumans[i].individualFitness.ToString()+" ";

        }
        
        outputTXT += bestHTXT;

        HelperFunctions.RecordText(outputTXT);
        //Debug.Log("Best human fitnesses were");
        //for (int i = 0; i < BestHumanStore; i++)
        //{
        //
        //    Debug.Log(bestHumans[i].individualFitness);
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
            if (randGenCount < RandomGenerationsAmount)
            {
                child.humanManager.spawnRandomHumans();
                child.runningOutline.GetComponent<SpriteRenderer>().color = Color.yellow;
            }
            else if(randGenCount != 14)
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

            if (randGenCount == 14 && FindObjectOfType<UI_script>().allowSuperGen.isOn)
            {
                HelperFunctions.HumanGroupAttributes superGen = new HelperFunctions.HumanGroupAttributes();
                superGen.humans = bestHumans;
                superGen.shiftAttributes();

                child.humanManager.startNextGeneration(superGen);
                child.runningOutline.GetComponent<SpriteRenderer>().color = Color.blue;
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

        //HelperFunctions.RecordText(bestGenerations[0].GroupFitness.ToString());


    }

    private void resetSim()
    {
        simNR++;

        ElapsedTime = 0f;
        generation = 1;

        bestGenerations.Clear();
        bestHumans.Clear();
        for (int i = 0; i < BestGenerationStore; i++)
        {
            HelperFunctions.HumanGroupAttributes temp = new HelperFunctions.HumanGroupAttributes();
            temp.setup();
            bestGenerations.Add(temp);
        }

        for (int i = 0; i < BestHumanStore; i++)
        {
            HelperFunctions.humanAttributes temp_h = new HelperFunctions.humanAttributes();
            temp_h.individualFitness = 0;
            bestHumans.Add(temp_h);
        }

        foreach (HomeScript child in FindObjectsOfType<HomeScript>())
        {
            child.resetSim();
            child.humanManager.spawnRandomHumans();
        }

        FindObjectOfType<UI_script>().BestFitness.text = ("#1 Generation Fitness: " + bestGenerations[0].GroupFitness);
        FindObjectOfType<UI_script>().BestStats.text = ("Wolves killed: \nFood gathered: \nAverage life span: ");
        FindObjectOfType<UI_script>().GenerationCount.text = "Generation " + generation.ToString();

        HelperFunctions.RecordText("banana");
    }
}


