using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CurrentState { idle, storingFood, gatheringFood, eating, fighting, fleeing, hunting, none };




public static class HelperFunctions
{

    public static void spawnText(Vector3 position, string text, IconType type)
    {
        GameObject a = MonoBehaviour.Instantiate(Transform.FindObjectOfType<ApplicationDataScript>().popT);
        a.transform.position = position;
        a.GetComponent<PopupTextScript>().Setup(text, type);
    }

    //returns true if its arrived
    public static bool goToTargetObject(GameObject subject, GameObject targetObject, float moveSpeed)
    {


        //if at the object, return true
        if (Vector3.Distance(subject.transform.position,targetObject.transform.position)< 0.2f)
        {
            return true;
            
        }
        else
        {
            //if not there, go towards it and return false
            Vector3 direction = targetObject.transform.position - subject.transform.position;
            direction.Normalize();
            subject.transform.position += direction * Time.deltaTime * moveSpeed;
        }
        return false;
    }

    public static GameObject getRandomTargetObjectInHolder(Transform holder)
    {
        return holder.GetChild(Random.Range(0, holder.childCount)).gameObject;
    }

    public struct humanAttributes
    {
        public float timeSurvived;
        public float foodGathered;
        public int wolvesKilled;

        public int healthPP;
        public int attackPP;
        public int carryPP;
        public int moveSpeedPP;
        public int stomachSizePP;

        public float eatingTriggerHungerFlat;
        public float eatingTriggerHealthFlat;
        public float fleeChance;
        public float huntChance;
        public void setRandom(int perkPoints)
        {
            int PPleftToSpend = perkPoints;

            healthPP = 1;
            attackPP = 1;
            carryPP = 1;
            moveSpeedPP = 1;
            stomachSizePP = 1;

            while (PPleftToSpend > 0)
            {
                int targertAttribute = Random.Range(0, 5); //choose which attribute to give points to  
                int ppAdd = Random.Range(1, PPleftToSpend + 1); //choose a random number of points to spend
                PPleftToSpend -= ppAdd;

                switch (targertAttribute) //distribute those points
                {
                    case 0:
                        attackPP += ppAdd;
                        break;
                    case 1:
                        healthPP += ppAdd;
                        break;
                    case 2:
                        moveSpeedPP += ppAdd;
                        break;
                    case 3:
                        carryPP += ppAdd;
                        break;
                    case 4:
                        stomachSizePP += ppAdd;
                        break;

                }
            }


            //set random triggers for eating
            eatingTriggerHealthFlat = Random.Range(0.0f, healthPP * 10);
            eatingTriggerHungerFlat = Random.Range(0.0f, stomachSizePP * 3);

            //set up % chance to gather/hunt and flee/fight
            huntChance = Random.Range(0.0f, 1.0f);
            fleeChance = Random.Range(0.0f, 1.0f);
        }
    }

}
