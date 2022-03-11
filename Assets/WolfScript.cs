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
    float attackTime;

    void updateState()
    {
        switch (currentState)
        {
            case CurrentState.idle:

                Vector3 direction = wanderEndLocation - this.transform.position;
                float length = direction.magnitude;

                if(length < 0.5)
                {
                    if(waitTime > 0)
                    {
                        waitTime -= Time.deltaTime;
                    }
                    else
                    {
                        wanderEndLocation = this.transform.parent.position + new Vector3(Random.Range(-10f, 10f), Random.Range(-2f, 10f), 0f);
                        waitTime = Random.Range(0f, 3f);
                    }

                }
                else
                {
                    direction.Normalize();
                    this.transform.position += direction * Time.deltaTime * moveSpeed;
                }
                
                break;

            case CurrentState.fighting:

                if (!targetObject)
                {
                    currentState = CurrentState.idle;
                }

                if(attackTime > 0)
                {
                    attackTime -= Time.deltaTime;
                }
                else
                {
                    attackTime = 1;
                    health -= targetObject.GetComponent<HumanScript>().attack;
                    targetObject.GetComponent<HumanScript>().health -= attack;

                    if (health <= 0)
                    {
                        Destroy(this.gameObject);
                    }

                    if (targetObject.GetComponent<HumanScript>().health <= 0)
                    {
                        Destroy(targetObject.GetComponent<HumanScript>().gameObject);
                    }
                }
                break;
        }
    }

    void CheckForNearbyHumans()
    {

        for (int i = 0; i < this.GetComponentInParent<WolfManager>().humanManager.transform.childCount; i++)
        {

            //this.GetComponentInParent<WolfManager>().humanManager.transform.GetChild(i);

        }

        //return targetArr[Random.Range(0, targetArr.Length)];
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
