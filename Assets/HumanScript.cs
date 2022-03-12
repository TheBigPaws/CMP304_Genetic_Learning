using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HumanScript : MonoBehaviour
{
    //HelperFunctions
    //perk points to assign
    public int perkPoints = 100;

    public HelperFunctions.humanAttributes attributes;

    //actual values for this object
    public float health;
    public float maxHealth;
    public float attack;
    public float carry;
    public float moveSpeed;
    public float stomachSize;

    //behaviour variables
    public CurrentState currentState = CurrentState.idle;
    public GameObject targetObject;
    public float actionTime;
    public float hunger;
    public float FoodInInventory;


    public void calculateAttributesFromPerkPoints()
    {
        maxHealth = attributes.healthPP * 10;
        health = maxHealth;
        attack = attributes.attackPP * 5;
        moveSpeed = Mathf.Sqrt(attributes.moveSpeedPP);
        carry = attributes.carryPP * 1;
        stomachSize = attributes.stomachSizePP * 3;
        hunger = stomachSize;
    }



    void StateUpdate()
    {

        //food node got fully used
        if (!targetObject && currentState != CurrentState.idle)
        {
            currentState = CurrentState.idle;
            //targetObject = HelperFunctions.getRandomTargetObjectInHolder(this.GetComponentInParent<HumanManager>().bushManager.transform);
        }

        switch (currentState)
        {
            case CurrentState.idle:
                //decide on what to do next
                DecideActivity();
                break;


            case CurrentState.storingFood:

                //if arrived at targetObject (here house)
                if (HelperFunctions.goToTargetObject(this.gameObject,targetObject,moveSpeed))
                {
                    //if there is still food in my inventory to store
                    if (FoodInInventory > 0)
                    {
                        float storeRate = 1;

                        //transfer it from my inventory to the house
                        FoodInInventory -= Time.deltaTime * storeRate;
                        targetObject.GetComponent<HomeScript>().storedFood += Time.deltaTime * storeRate;
                    }
                    else
                    {
                        //if there's no more food to store,
                        currentState = CurrentState.idle;
                    }
                }
                break;


            case CurrentState.eating:

                //if arrived at targetObject (here house)
                if (HelperFunctions.goToTargetObject(this.gameObject, targetObject, moveSpeed))
                {
                    //if stomach or health aren't full and there is still food to eat
                    if ((hunger < stomachSize || health < maxHealth)&& targetObject.GetComponent<HomeScript>().storedFood > 0)
                    {
                        float eatingRate = 1;
                        //eat and replenish health and hunger
                        hunger += Time.deltaTime * eatingRate;
                        targetObject.GetComponent<HomeScript>().storedFood -= Time.deltaTime * eatingRate;
                        if(maxHealth > health)
                        {
                            health += Time.deltaTime * eatingRate;
                        }
                    }
                    else
                    {
                        currentState = CurrentState.idle;
                    }
                }
                break;


            case CurrentState.hunting:
                //if arrived at targetObject (here enemy)
                if (HelperFunctions.goToTargetObject(this.gameObject, targetObject, moveSpeed))
                {

                }

                    break;

            case CurrentState.fleeing:

                float fleeingDistance = 5;

                //run direction away from targetObject
                Vector3 direction = this.transform.position - targetObject.transform.position;
                float distance = direction.magnitude;
                direction.Normalize();
                this.transform.position += direction * Time.deltaTime * moveSpeed;

                //if i've run far enough
                if (distance >= fleeingDistance)
                {
                    currentState = CurrentState.idle;
                }

                break;

            case CurrentState.gatheringFood:


                if (HelperFunctions.goToTargetObject(this.gameObject, targetObject, moveSpeed))
                {
                    //extract food
                    FoodInInventory += targetObject.GetComponent<FoodSource>().extractFood();

                    //inventory full
                    if (FoodInInventory >= carry)
                    {
                        //finishedAction = false;
                        currentState = CurrentState.storingFood;
                        targetObject = this.transform.parent.GetComponent<HumanManager>().Home.gameObject;
                    }
                }


                break;

            case CurrentState.goingToEnemy:
                if (HelperFunctions.goToTargetObject(this.gameObject, targetObject, moveSpeed))
                {
                    //finishedAction = false;
                    currentState = CurrentState.fighting;
                    //targetObject.GetComponent
                }
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        //goToTargetObject();

    }

    void FixedUpdate()
    {
        HungerHealthEatCheck();
        StateUpdate();
    }

    public void DecideFlight()
    {
        float choiceFloat = Random.Range(0.0f, 1.0f);

        if( choiceFloat < attributes.fleeChance)
        {
            currentState = CurrentState.fleeing;
        }
        else
        {
            currentState = CurrentState.fighting;
        }
    }

    void DecideActivity()
    {
        float choiceFloat = Random.Range(0.0f, 1.0f);

        if (choiceFloat < attributes.huntChance)
        {
            currentState = CurrentState.hunting;

            //get random wolf that's not dead
            targetObject = HelperFunctions.getRandomTargetObjectInHolder(this.GetComponentInParent<HumanManager>().wolfManager.transform);
            while (!targetObject.GetComponent<WolfScript>())
            {
                targetObject = HelperFunctions.getRandomTargetObjectInHolder(this.GetComponentInParent<HumanManager>().wolfManager.transform);
            }
        }
        else
        {
            currentState = CurrentState.gatheringFood;

            //if there are any dead wolves, gather those (more food)
            for(int i = 0; i < this.GetComponentInParent<HumanManager>().wolfManager.transform.childCount; i++)
            {
                if (this.GetComponentInParent<HumanManager>().wolfManager.transform.GetChild(i).GetComponent<FoodSource>())
                {
                    targetObject = this.GetComponentInParent<HumanManager>().wolfManager.transform.GetChild(i).gameObject;
                    return;
                }
            }

            //set random bush
            targetObject = HelperFunctions.getRandomTargetObjectInHolder(this.GetComponentInParent<HumanManager>().bushManager.transform);

        }
    }

    void HungerHealthEatCheck()
    {
        //decrement hunger
        hunger -= Time.deltaTime;

        if(hunger < 0)
        {
            Destroy(this.gameObject);
        }

        //if health is too low, start fleeing
        if(currentState == CurrentState.fighting && health < attributes.eatingTriggerHealthFlat)
        {
            currentState = CurrentState.fleeing;
        }

        if(currentState != CurrentState.fighting && currentState != CurrentState.fleeing)
        {
            //check if hungry/hurt enough to go eat
            if (health < attributes.eatingTriggerHealthFlat || hunger < attributes.eatingTriggerHungerFlat)
            {
                currentState = CurrentState.eating;
                targetObject = this.GetComponentInParent<HumanManager>().Home.gameObject;
            }
        }
        
    }
}
