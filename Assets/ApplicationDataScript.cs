using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationDataScript : MonoBehaviour
{
    public float ElapsedTime = 0f;
    public int generation = 0;
    public GameObject popT;

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

        if(ElapsedTime > 10)
        {
            ElapsedTime = 0;
            Debug.Log(FindObjectsOfType<HomeScript>().Length);
        }
    }

    public void evaluateGenerations()
    {
        foreach (HomeScript child in FindObjectsOfType<HomeScript>())
        {
            for (int i = 0; i < BestGenerationStore; i++)
            {

                if(bestGenerations[i].groupFitness < child.groupAttributes.getGroupFitness())
                {
                    bestGenerations[i] = child.groupAttributes;
                }
            }
        }

            Debug.Log("Best group fitnesses were");
        for (int i = 0; i < BestGenerationStore; i++)
        {
            Debug.Log(bestGenerations[i].groupFitness);
        }
    }
}
