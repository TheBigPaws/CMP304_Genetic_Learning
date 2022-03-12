using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HumanScript : MonoBehaviour
{
    //HelperFunctions
    //perk points to assign
    int perkPoints = 100;

    public int healthPP = 1;
    public int attackPP = 1;
    public int carryPP = 1;
    public int moveSpeedPP = 1;
    public int stomachSizePP = 1;

    //actual values for this object
    public float health;
    public float attack;
    public float carry;
    public float moveSpeed;
    public float stomachSize;

    //behaviour variables
    public float actionTime;
    public float hunger;
    public float eatingTriggerHungerFlat;
    public GameObject targetObject;
    public float FoodInInventory;
    public CurrentState currentState = CurrentState.idle;
    public float fleeChance;
    public float huntChance;

    void calculateAttributesFromPerkPoints()
    {
        health = healthPP * 10;
        attack = attackPP * 5;
        moveSpeed = Mathf.Sqrt(moveSpeedPP);
        carry = carryPP * 1;
        stomachSize = stomachSizePP * 1;
    }


    public GameObject getRandomTargetObjectofTag(string tag)
    {
        if (tag == "Bush")
        {
            return this.GetComponentInParent<HumanManager>().bushManager.transform.GetChild(Random.Range(0, this.GetComponentInParent<HumanManager>().bushManager.transform.childCount)).gameObject;
        }
        else if (tag == "Wolf")
        {
            return this.GetComponentInParent<HumanManager>().wolfManager.transform.GetChild(Random.Range(0, this.GetComponentInParent<HumanManager>().wolfManager.transform.childCount)).gameObject;
        }
        //GameObject[] targetArr = GameObject.FindGameObjectsWithTag(tag);
        //return targetArr[Random.Range(0, targetArr.Length)];
        return null;
    }

    void setRandomAttributes()
    {
        List<int> indexes = new List<int>();
        indexes.Add(1); //health;
        indexes.Add(2); //attack;
        indexes.Add(3); //carry;
        indexes.Add(4); //moveSpeed;
        indexes.Add(5); //stomachSize;

        int PPleftToSpend = perkPoints;

        for (int i = 0; i < 5; i++)
        {
            int res_idx = Random.Range(0, indexes.Count);
            int ppAdd = Random.Range(0, PPleftToSpend);

            PPleftToSpend -= ppAdd;

            switch (indexes[res_idx])
            {
                case 1:
                    healthPP += ppAdd;
                    break;
                case 2:
                    attackPP += ppAdd;
                    break;
                case 3:
                    carryPP += ppAdd;
                    break;
                case 4:
                    moveSpeedPP += ppAdd;
                    break;
                case 5:
                    stomachSizePP += ppAdd;
                    break;
            }
            indexes.RemoveAt(res_idx);
        }

        huntChance = Random.Range(0.0f, 1.0f);
        fleeChance = Random.Range(0.0f, 1.0f);

        calculateAttributesFromPerkPoints();
    }

    //returns true if its arrived
    //bool goToTargetObject()
    //{
    //
    //    //failsafe if targetObject gets destroyed
    //    if (!targetObject)
    //    {
    //        currentState = CurrentState.idle;
    //        //finishedAction = true;
    //        return false;
    //    }
    //
    //    //go towards target object
    //    if (!isAtTargetObject())
    //    {
    //        Vector3 direction = targetObject.transform.position - this.transform.position;
    //        direction.Normalize();
    //        this.transform.position += direction * Time.deltaTime * moveSpeed;
    //    }
    //    else
    //    {
    //        return true;
    //    }
    //    return false;
    //}
    //
    //bool isAtTargetObject()
    //{
    //
    //
    //    bool retVar = false;
    //
    //    Vector3 translation = targetObject.transform.position - this.transform.position;
    //
    //    if (translation.magnitude < 1)
    //    {
    //        retVar = true;
    //    }
    //
    //    return retVar;
    //}


    
    // Start is called before the first frame update
    void Start()
    {
        setRandomAttributes();
    }

    void StateUpdate()
    {
        switch (currentState)
        {
            case CurrentState.idle:
                //decide on what to do next

                //find food

                targetObject = getRandomTargetObjectofTag("Bush");
                currentState = CurrentState.gatheringFood;
                break;


            case CurrentState.storingFood:

                if (HelperFunctions.goToTargetObject(this.gameObject,targetObject,moveSpeed))
                {
                    if (FoodInInventory > 0)
                    {
                        float storeRate = 1;
                        FoodInInventory -= Time.deltaTime * storeRate;

                        targetObject.GetComponent<HomeScript>().storedFood += Time.deltaTime * storeRate;
                    }
                    else
                    {
                        currentState = CurrentState.idle;
                    }
                }
                break;


            case CurrentState.eating:
                if (HelperFunctions.goToTargetObject(this.gameObject, targetObject, moveSpeed))
                {
                    if (hunger < stomachSize && targetObject.GetComponent<HomeScript>().storedFood > 0)
                    {
                        float eatingRate = 1;

                        hunger += Time.deltaTime * eatingRate;
                        targetObject.GetComponent<HomeScript>().storedFood -= Time.deltaTime * eatingRate;
                    }
                    else
                    {
                        currentState = CurrentState.idle;
                    }
                }
                break;


            case CurrentState.fighting:
                break;

            case CurrentState.fleeing:

                float fleeingDistance = 5;

                //run direction away from targetObject
                Vector3 direction = this.transform.position - targetObject.transform.position;
                float distance = direction.magnitude;
                direction.Normalize();
                this.transform.position += direction * Time.deltaTime * moveSpeed;

                if (distance >= fleeingDistance)
                {
                    currentState = CurrentState.idle;
                }

                break;

            case CurrentState.gatheringFood:

                //food node got fully used
                if (!targetObject)
                {
                    targetObject = getRandomTargetObjectofTag("Bush");
                    currentState = CurrentState.gatheringFood;
                }

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

    // Update is called once per frame
    void Update()
    {
        //goToTargetObject();

    }

    void FixedUpdate()
    {
        //hungerUpdate
        //flee check update
        //
        StateUpdate();
    }

    public void DecideFlight()
    {
        float choiceFloat = Random.Range(0.0f, 1.0f);

        if( choiceFloat < fleeChance)
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

        if (choiceFloat < huntChance)
        {
            currentState = CurrentState.hunting;
        }
        else
        {
            currentState = CurrentState.gatheringFood;
        }
    }
}