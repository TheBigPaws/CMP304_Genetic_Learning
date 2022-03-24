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

    public struct HumanGroupAttributes
    {
        public float groupFitness;
        public List<HelperFunctions.humanAttributes> humans;
        public float getGroupFitness()
        {
            float Gfitness = 0;

            foreach(HelperFunctions.humanAttributes child in humans)
            {
                Gfitness += child.individualFitness;
                if (child.alive) { Gfitness += 300; }

            }
            groupFitness = Gfitness;
            return Gfitness;
        }
        public void setup()
        {
            humans = new List<humanAttributes>();
        }
        public void addHuman(HelperFunctions.humanAttributes attributes)
        {
            humans.Add(attributes);
        }
        public void shiftAttributes()
        {
            foreach (HelperFunctions.humanAttributes child in humans)
            {
                child.shuffleAttributes();

            }
        }
    }

    public struct humanAttributes
    {
        public bool alive;

        public float individualFitness;
        public float timeSurvived;
        public float foodGathered;
        public int wolvesKilled;

        public int healthPP;
        public int attackPP;
        public int carryPP;
        public int moveSpeedPP;
        public int stomachSizePP;

        public float eatingTriggerHungerPerc;
        public float eatingTriggerHealthPerc;
        public float fleeChance;
        public float huntChance;

        public void resetData()
        {
        alive = true;

        individualFitness = 0;
        timeSurvived = 0;
        foodGathered = 0;
        wolvesKilled = 0;
    }

        public void shuffleAttributes()
        {
            //randomly shuffle these chance attreibutes: find anywhere it can move and cut that in half
            eatingTriggerHealthPerc += Random.Range(-eatingTriggerHealthPerc, 1.0f - eatingTriggerHealthPerc) / 2;
            eatingTriggerHungerPerc += Random.Range(-eatingTriggerHungerPerc, 1.0f - eatingTriggerHungerPerc) / 2;

            huntChance += Random.Range(-huntChance, 1.0f- huntChance)/2;
            fleeChance += Random.Range(-fleeChance, 1.0f- fleeChance)/2;

            int respendPP = 0;
            //shuffle attributePoints
            if(healthPP > 5)
            {
                healthPP -= 5;
                respendPP += 5;
            }
            if (attackPP > 5)
            {
                attackPP -= 5;
                respendPP += 5;
            }
            if (stomachSizePP > 8)
            {
                stomachSizePP -= 5;
                respendPP += 5;
            }
            if (moveSpeedPP > 5)
            {
                moveSpeedPP -= 5;
                respendPP += 5;
            }
            if (carryPP > 5)
            {
                carryPP -= 5;
                respendPP += 5;
            }
            spendPoints(respendPP);
            resetData();
        }

        void spendPoints(int pointsToSpend)
        {
            int PPleftToSpend = pointsToSpend;

            while (PPleftToSpend > 0)
            {
                int targertAttribute = Random.Range(0, 5); //choose which attribute to give points to  
                int ppAdd = Random.Range(1, Random.Range(1, PPleftToSpend + 2)); //choose a random number of points to spend
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
        }

        public void setRandom(int perkPoints)
        {
            resetData();
            int PPleftToSpend = perkPoints;

            healthPP = 1;
            attackPP = 1;
            carryPP = 1;
            moveSpeedPP = 1;
            stomachSizePP = 5;

            spendPoints(perkPoints);
      

            //set random triggers for eating
            eatingTriggerHealthPerc = Random.Range(0.0f, 1.0f);
            eatingTriggerHungerPerc = Random.Range(0.0f, 1.0f);

            //set up % chance to gather/hunt and flee/fight
            huntChance = Random.Range(0.0f, 1.0f);
            fleeChance = Random.Range(0.0f, 1.0f);
        }
    }

}
