using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanManager : MonoBehaviour
{

    public WolfManager wolfManager;
    public BushManager bushManager;
    public HomeScript Home;
    public int HumansToSpawn = 5;

    public GameObject HumanRef;

    // Start is called before the first frame update
    void Start()
    {
        spawnRandomHumans();
    }

    public void startNextGeneration(HelperFunctions.HumanGroupAttributes groupToSpawn)
    {
        for (int i = 0; i < HumansToSpawn; i++)
        {
            GameObject go = Instantiate(HumanRef, this.transform);
            go.transform.position = Home.transform.position;
            go.GetComponent<HumanScript>().attributes = groupToSpawn.humans[i];
            go.GetComponent<HumanScript>().attributes.resetData();
            go.GetComponent<HumanScript>().calculateAttributesFromPerkPoints();
            Debug.Log(go.GetComponent<HumanScript>().attributes.individualFitness);
            Debug.Log(go.GetComponent<HumanScript>().attributes.wolvesKilled);

        }
    }


    public void spawnRandomHumans()
    {
        for (int i = 0; i < HumansToSpawn; i++)
        {
            GameObject go = Instantiate(HumanRef, this.transform);
            go.transform.position = Home.transform.position;
            go.GetComponent<HumanScript>().attributes.setRandom(go.GetComponent<HumanScript>().perkPoints);
            go.GetComponent<HumanScript>().calculateAttributesFromPerkPoints();

        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
