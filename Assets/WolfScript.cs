using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfScript : MonoBehaviour
{

    public float health = 500;
    public float attack = 50;
    public float moveSpeed = 5;
    
    CurrentState currentState = CurrentState.idle;
    public GameObject targetObject;

    Vector3 wanderEndLocation;
    float waitTime;
    public float attackTime;

    void updateState()
    {
        //update attack check
        

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
                        wanderEndLocation = this.transform.parent.position + new Vector3(Random.Range(-10f, 10f), Random.Range(-2f, 10f), 0f);
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

                if(Vector3.Distance(this.transform.position,targetObject.transform.position) < 0.2f)
                {
                    currentState = CurrentState.fighting;
                }
                else
                {
                    Vector3 direction_ = targetObject.transform.position - this.transform.position;

                    this.transform.position += direction_.normalized * Time.deltaTime * moveSpeed;
                }
                break;

            case CurrentState.fighting:

                if (!targetObject)
                {
                    currentState = CurrentState.idle;
                }

                if (Vector3.Distance(this.transform.position, targetObject.transform.position) > 0.3f)
                {
                    currentState = CurrentState.hunting;
                }

                if (attackTime > 0) //wait for next hit
                {
                    attackTime -= Time.deltaTime;

                }
                else
                {
                    //reset attack time and deal damage to both
                    attackTime = 1;
                    health -= targetObject.GetComponent<HumanScript>().attack;
                    targetObject.GetComponent<HumanScript>().health -= attack;

                    //death cases
                    if (targetObject.GetComponent<HumanScript>().health <= 0)
                    {
                        Destroy(targetObject.GetComponent<HumanScript>().gameObject);
                    } 

                    if (health <= 0)
                    {
                        Destroy(this.gameObject);
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
            
            if(Vector3.Distance(this.transform.position,tempGOholder.transform.position) < 3)
            {
                targetObject = tempGOholder;
                currentState = CurrentState.hunting;
                targetObject.GetComponent<HumanScript>().DecideFlight();
                return;
            }

        }        
    }

    // Start is called before the first frame update
    void Start()
    {
        wanderEndLocation = this.transform.parent.position + new Vector3(Random.Range(-10f, 10f), Random.Range(-2f, 10f), 0f); ;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //CheckForNearbyHumans();
        updateState();
    }
}
