using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HumanScript : MonoBehaviour
{


    //perk points to assign
    public int perkPoints = 100;

    public HelperFunctions.humanAttributes attributes;

    //actual values for this objectd
    public float health;
    public float maxHealth;
    public float attack;
    public float carry;
    public float moveSpeed;
    public float stomachSize;

    public float percToHeal;
    public float percToEat;

    //behaviour variables
    public CurrentState currentState = CurrentState.idle;
    public GameObject targetObject;
    //public float actionTime;
    public float hunger;
    public float FoodInInventory;

    float attackTime = 0.0f;

    public void calculateAttributesFromPerkPoints()
    {
        maxHealth = attributes.healthPP * 10;
        health = maxHealth;
        attack = attributes.attackPP * 5;
        moveSpeed = Mathf.Sqrt(attributes.moveSpeedPP);
        carry = attributes.carryPP * 4;
        stomachSize = attributes.stomachSizePP * 3;
        hunger = stomachSize;
    }



    public void endSimRecord(bool alive)
    {
        attributes.timeSurvived = this.GetComponentInParent<HumanManager>().Home.simulationLife;
        attributes.alive = alive;
        attributes.calculateFitness();
        this.GetComponentInParent<HumanManager>().Home.groupAttributes.addHuman(attributes);

        Destroy(this.gameObject);
    }

    void StateUpdate()
    {

        //only on idle should there be no targetObject
        if (!targetObject && currentState != CurrentState.idle)
        {
            currentState = CurrentState.idle;
        }

        //only on idle should there be no targetObject
        if (currentState == CurrentState.hunting || currentState == CurrentState.fleeing || currentState == CurrentState.fighting)
        {
            if(targetObject.name != "Wolf(Clone)")
            {
                //Debug.Log("strudel");
                currentState = CurrentState.idle;
            }
        }

        switch (currentState)
        {
            //----------------------------------IDLE: Decide on what to do next
            case CurrentState.idle:
                //decide on what to do next
                DecideActivity();
                break;

            //----------------------------------GATHERING FOOD: go to a food source and gather food
            case CurrentState.gatheringFood:


                if (HelperFunctions.goToTargetObject(this.gameObject, targetObject, moveSpeed))
                {

                    //attempting extracting as much food as possible
                    FoodInInventory += targetObject.GetComponent<FoodSource>().extractFood(carry - FoodInInventory);

                    //inventory full
                    if (FoodInInventory >= carry)
                    {
                        //set state to storing food
                        currentState = CurrentState.storingFood;
                        targetObject = this.transform.parent.GetComponent<HumanManager>().Home.gameObject;
                    }
                }


                break;

            //----------------------------------STORING FOOD: go to house and store food
            case CurrentState.storingFood:

                //if arrived at house
                if (HelperFunctions.goToTargetObject(this.gameObject,targetObject,moveSpeed))
                {
                    //store all my food
                    HelperFunctions.spawnText(targetObject.transform.position, "+" + FoodInInventory, IconType.food);
                    targetObject.GetComponent<HomeScript>().storedFood += FoodInInventory;
                    
                    //record food gathered
                    attributes.foodGathered += FoodInInventory;
                    
                    FoodInInventory = 0;
                    currentState = CurrentState.idle;

                }
                break;

            //----------------------------------EATING FOOD: go to house and eat food
            case CurrentState.eating:

                //if arrived at house
                if (HelperFunctions.goToTargetObject(this.gameObject, targetObject, moveSpeed))
                {

                    float FoodEaten = 0;

                    float healthDiff = maxHealth - health;

                    float hungerDiff = stomachSize - hunger;

                    //if there is enough food to fill hunger fully, fill it
                    if(hungerDiff < targetObject.GetComponent<HomeScript>().storedFood)
                    {
                        FoodEaten += hungerDiff;
                        targetObject.GetComponent<HomeScript>().storedFood -= hungerDiff;
                        HelperFunctions.spawnText(transform.position, "+" + hungerDiff, IconType.hunger);
                        //Debug.Log("ate to full cause hunger");
                        //if there is enough food to heal fully, do it
                        if (healthDiff < targetObject.GetComponent<HomeScript>().storedFood)
                        {
                            FoodEaten += healthDiff;
                            //Debug.Log("ate to full cause hp");
                            targetObject.GetComponent<HomeScript>().storedFood -= healthDiff;
                            currentState = CurrentState.idle;
                            HelperFunctions.spawnText(transform.position, "+" + healthDiff, IconType.heart);

                        }
                        else //eat the rest
                        {
                            //Debug.Log("ate the rest of food for hp");
                            FoodEaten += targetObject.GetComponent<HomeScript>().storedFood;
                            HelperFunctions.spawnText(transform.position, "+" + targetObject.GetComponent<HomeScript>().storedFood, IconType.heart);

                            targetObject.GetComponent<HomeScript>().storedFood = 0;
                            currentState = CurrentState.idle;
                        }
                    }
                    else//else just eat all the food left
                    {
                        FoodEaten = targetObject.GetComponent<HomeScript>().storedFood;
                        //Debug.Log("ate the rest of food for hunger");
                        HelperFunctions.spawnText(transform.position, "+" + FoodEaten, IconType.hunger);
                        targetObject.GetComponent<HomeScript>().storedFood = 0;
                        currentState = CurrentState.idle;
                    }

                    HelperFunctions.spawnText(targetObject.transform.position, "-" + FoodEaten, IconType.food);
                    
                }
                break;

            //----------------------------------HUNTING: Go To Enemy
            case CurrentState.hunting:
                //if arrived at targetObject (here enemy)
                if (HelperFunctions.goToTargetObject(this.gameObject, targetObject, moveSpeed))
                {

                }
                if(targetObject.GetComponent<WolfScript>() == null)
                {
                    currentState = CurrentState.idle;
                }
                break;

            //----------------------------------FLEEING:
            case CurrentState.fleeing:

                float fleeingDistance = 5;

                //run direction away from targetObject
                Vector3 direction = this.transform.position - targetObject.transform.position;
                float distance = direction.magnitude;
                direction.Normalize();
                this.transform.position += direction * Time.deltaTime * moveSpeed;

                //if run far enough, back to idle
                if (distance >= fleeingDistance)
                {
                    currentState = CurrentState.idle;
                }

                break;

            //----------------------------------FIGHTING:
            case CurrentState.fighting:

                if (attackTime > 0) //wait for next hit
                {
                    attackTime -= Time.deltaTime;

                }
                else
                {
                    //reset attack time and deal damage to both
                    attackTime = 1;
                    //health -= targetObject.GetComponent<HumanScript>().attack;
                    targetObject.GetComponent<WolfScript>().health -= attack;

                    //wolf died
                    if (targetObject.GetComponent<WolfScript>().health <= 0)
                    {
                        HelperFunctions.spawnText(transform.position, "-" + (attack + targetObject.GetComponent<WolfScript>().health).ToString(), IconType.heart);
                        attributes.damageDealt += attack + targetObject.GetComponent<WolfScript>().health;
                        currentState = CurrentState.idle;
                        //attributes.wolvesKilled += 1;

                        targetObject.GetComponent<WolfScript>().WolfDeath();
                    }
                    else
                    {
                        attributes.damageDealt += attack;
                        HelperFunctions.spawnText(transform.position, "-" + (attack).ToString(), IconType.heart);
                    }
                }
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        percToEat = attributes.eatingTriggerHungerPerc;
        percToHeal = attributes.eatingTriggerHealthPerc;

        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        HungerHealthEatCheck();
        StateUpdate();
        OutOfBoundsFix();
    }

    public void DecideFlight()
    {
        //random float to decide

        float choiceFloat = Random.Range(0.0f, 1.0f);

        if(currentState != CurrentState.fighting && currentState != CurrentState.fleeing)
        {
            //if within the range of flee, set state as flee
            if (choiceFloat < attributes.fleeChance)
            {
                currentState = CurrentState.fleeing;
            }
            else
            {
                //else fight back
                currentState = CurrentState.fighting;
            }
        } 

    }

    void DecideActivity()
    {
        //random float to decide
        float choiceFloat = Random.Range(0.0f, 1.0f);

        //if within the range of hunt, set state as hunting
        if (choiceFloat < attributes.huntChance)
        {
            currentState = CurrentState.hunting;

            //get random wolf that's not dead
            targetObject = HelperFunctions.getRandomTargetObjectInHolder(this.GetComponentInParent<HumanManager>().wolfManager.transform);
        }
        else
        {
            currentState = CurrentState.gatheringFood;

            //set random bush
            targetObject = HelperFunctions.getRandomTargetObjectInHolder(this.GetComponentInParent<HumanManager>().bushManager.transform);

        }
    }

    void HungerHealthEatCheck()
    {
        attributes.timeSurvived += Time.deltaTime;

        //decrement hunger
        hunger -= Time.deltaTime;

        //death case
        if(hunger <= 0 || health <=0)
        {
            endSimRecord(false);
            return;
        }


        //whenever not fighting or fleeing
        if (currentState != CurrentState.fleeing)
        {
            //if there's any food at home
            if (this.GetComponentInParent<HumanManager>().Home.storedFood > 0)
            {
                //check if hungry/hurt enough to go eat
                if (health/maxHealth < attributes.eatingTriggerHealthPerc || hunger/stomachSize < attributes.eatingTriggerHungerPerc)
                {
                    if(currentState == CurrentState.fighting)
                    {
                        currentState = CurrentState.fleeing;
                    }
                    else
                    {
                        currentState = CurrentState.eating;
                        targetObject = this.GetComponentInParent<HumanManager>().Home.gameObject;
                    }
                }
            }
        }
    }

    void OutOfBoundsFix()
    {
        Vector3 localPos = this.transform.localPosition;

        if (Mathf.Abs(localPos.x) > 0.5f)
        {
            localPos.x = Mathf.Abs(localPos.x) * 0.5f / localPos.x;
        }

        if (Mathf.Abs(localPos.y) > 0.5f)
        {
            localPos.y = Mathf.Abs(localPos.y) * 0.5f / localPos.y;
        }

        this.transform.localPosition = localPos;

    }
}
