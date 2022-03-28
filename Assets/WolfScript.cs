using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfScript : MonoBehaviour
{
    public float health = 500;
    public float attack = 30;
    public float moveSpeed = 5;
    
    public CurrentState currentState = CurrentState.idle;
    public GameObject targetObject;

    float wanderRange;

    Vector3 wanderEndLocation;
    float waitTime;
    public float attackTime;

    void updateState()
    {
        //update attack check
        
        if(currentState != CurrentState.idle)
        {
            if (!targetObject)
            {
                currentState = CurrentState.idle;
                

            }

        }

        switch (currentState)
        {
            case CurrentState.idle:

                

                Vector3 direction = wanderEndLocation - this.transform.position;
                float length = direction.magnitude;

                if(length < 0.2) //if arrived at wander end location
                {
                    if(waitTime > 0) //if not waited long enough, wait
                    {
                        waitTime -= Time.deltaTime;
                    }
                    else //pick new wander end location
                    {
                        wanderEndLocation = this.transform.parent.position + new Vector3(Random.Range(-wanderRange, wanderRange), Random.Range(-wanderRange/5, wanderRange), 0f);
                        waitTime = Random.Range(0f, 3f);
                    }

                }
                else //go towards end wander location
                {
                    direction.Normalize();
                    this.transform.position += direction * Time.deltaTime * moveSpeed;
                }
                CheckForNearbyHumans();
                break;

            case CurrentState.hunting:

                if (HelperFunctions.goToTargetObject(this.gameObject, targetObject.gameObject, moveSpeed))
                {
                    currentState = CurrentState.fighting;
                }
                if(Vector3.Distance(this.transform.position,targetObject.transform.position) > 3)
                {
                    currentState = CurrentState.idle;
                }
                break;

            case CurrentState.fighting:

                if (Vector3.Distance(this.transform.position, targetObject.transform.position) > 0.4f)
                {
                    currentState = CurrentState.hunting;
                }
                
                //if targetobject is not fighting or fleeing, decide to do either
                if(targetObject.GetComponent<HumanScript>().currentState != CurrentState.fighting && targetObject.GetComponent<HumanScript>().currentState != CurrentState.fleeing)
                {
                    targetObject.GetComponent<HumanScript>().DecideFlight();
                    targetObject.GetComponent<HumanScript>().targetObject = this.gameObject;
                }

                if (attackTime > 0) //wait for next hit
                {
                    attackTime -= Time.deltaTime;

                }
                else
                {
                    //reset attack time and deal damage to both
                    attackTime = 1;
                    //health -= targetObject.GetComponent<HumanScript>().attack;
                    targetObject.GetComponent<HumanScript>().health -= attack;

                    //wolf died
                    if (health <= 0)
                    {
                        HelperFunctions.spawnText(transform.position, "-" + (targetObject.GetComponent<HumanScript>().attack + health).ToString(), IconType.heart);

                        targetObject.GetComponent<HumanScript>().currentState = CurrentState.idle;
                        targetObject.GetComponent<HumanScript>().attributes.wolvesKilled += 1;

                        this.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.5f, 0.5f);
                        this.transform.name = "WolfCorpse";
                        //make this object into a food source
                        this.gameObject.AddComponent<FoodSource>();
                        this.gameObject.GetComponent<FoodSource>().FoodAmount = 60;

                        this.GetComponentInParent<WolfManager>().spawnRandomLocWolf();

                        this.gameObject.transform.SetParent(this.GetComponentInParent<WolfManager>().humanManager.bushManager.transform);
                        //spawn a new wolf
                        Destroy(this);
                    }
                    else
                    {
                        HelperFunctions.spawnText(transform.position, "-" + (targetObject.GetComponent<HumanScript>().attack).ToString(), IconType.heart);
                    }

                    //death cases
                    if (targetObject.GetComponent<HumanScript>().health <= 0)
                    {
                        HelperFunctions.spawnText(targetObject.transform.position, "-" + (attack + targetObject.GetComponent<HumanScript>().health).ToString(), IconType.heart);
                        targetObject.GetComponent<HumanScript>().attributes.alive = false;

                        targetObject = null;
                        this.currentState = CurrentState.idle;

                        //write down dead human data
                    }
                    else
                    {
                        HelperFunctions.spawnText(targetObject.transform.position, "-" + (attack).ToString(), IconType.heart);
                    }
                }
                break;
        }
    }

    void CheckForNearbyHumans()
    {
        for (int i = 0; i < this.GetComponentInParent<WolfManager>().humanManager.transform.childCount; i++)
        {

            GameObject tempGOholder = this.GetComponentInParent<WolfManager>().humanManager.transform.GetChild(i).gameObject;
            
            if(Vector3.Distance(this.transform.position,tempGOholder.transform.position) < 3 && tempGOholder.GetComponent<HumanScript>().attributes.alive)
            {
                targetObject = tempGOholder;
                currentState = CurrentState.hunting;
                targetObject.GetComponent<HumanScript>().DecideFlight();
                targetObject.GetComponent<HumanScript>().targetObject = this.gameObject;

            }

        }

    }

    public void WolfDeath()
    {
        this.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.5f, 0.5f);
        this.transform.name = "WolfCorpse";
        //make this object into a food source
        this.gameObject.AddComponent<FoodSource>();
        this.gameObject.GetComponent<FoodSource>().FoodAmount = 60;

        this.GetComponentInParent<WolfManager>().spawnRandomLocWolf();

        this.gameObject.transform.SetParent(this.GetComponentInParent<WolfManager>().humanManager.bushManager.transform);
        //spawn a new wolf
        Destroy(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        wanderRange = transform.parent.parent.localScale.x / 2;
        wanderEndLocation = this.transform.parent.position + new Vector3(Random.Range(-wanderRange, wanderRange), Random.Range(-wanderRange/5, wanderRange), 0f); ;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //CheckForNearbyHumans();
        updateState();
    }
}
