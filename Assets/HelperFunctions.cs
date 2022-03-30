using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.IO;



public enum CurrentState { idle, storingFood, gatheringFood, eating, fighting, fleeing, hunting, none };



public static class HelperFunctions
{

    public static void spawnText(Vector3 position, string text, IconType type)
    {
        if (Transform.FindObjectOfType<UI_script>().spawnPopupUI.isOn)
        {
            ApplicationDataScript appData = Transform.FindObjectOfType<ApplicationDataScript>();
            GameObject a = MonoBehaviour.Instantiate(appData.popT, appData.transform);
            a.transform.position = position;
            a.GetComponent<PopupTextScript>().Setup(text, type);
        }
    }



    //returns true if its arrived
    public static bool goToTargetObject(GameObject subject, GameObject targetObject, float moveSpeed)
    {


        //if at the object, return true
        if (Vector3.Distance(subject.transform.position, targetObject.transform.position) < 0.2f)
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
        //public float groupFitness;
        public List<HelperFunctions.humanAttributes> humans;
        public float GroupFitness;
        public float CalculateGroupFitness()
        {
            GroupFitness = 0;
            foreach (HelperFunctions.humanAttributes child in humans)
            {
                GroupFitness += child.individualFitness;
                //if (child.alive) { Gfitness += 300; }

            }
            //groupFitness = Gfitness;
            return GroupFitness;
        }
        public void setup()
        {
            GroupFitness = 0;
            humans = new List<humanAttributes>();
            humans.Clear();
        }
        public void addHuman(HelperFunctions.humanAttributes attributes)
        {

            HelperFunctions.humanAttributes temp = attributes;
            humans.Add(temp);
            //humans.pu
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

        public int ID;

        //perk point attributes
        public int healthPP;
        public int attackPP;
        public int carryPP;
        public int moveSpeedPP;
        public int stomachSizePP;

        //behaviour preference
        public float eatingTriggerHungerPerc;
        public float eatingTriggerHealthPerc;
        public float fleeChance;
        public float huntChance;

        //fitness variables
        public float individualFitness;
        public float timeSurvived;
        public float foodGathered;
        public float damageDealt;
        //public int wolvesKilled;

        public string getAttsAsString()
        {
            string retStr = "";
            retStr += healthPP;
            retStr += attackPP;
            retStr += carryPP;
            retStr += moveSpeedPP;
            retStr += stomachSizePP;
            return retStr;
        }

        public void calculateFitness()
        {
            individualFitness = timeSurvived + damageDealt + foodGathered * 2;
            individualFitness /= 10;
            //individualFitness = wolvesKilled;
        }

        public void resetData()
        {
            alive = true;

            individualFitness = 0;
            timeSurvived = 0;
            foodGathered = 0;
            damageDealt = 0;
        }

        public void shuffleAttributes()
        {
            //randomly shuffle these chance attreibutes: find anywhere it can move and cut that in half
            eatingTriggerHealthPerc += Random.Range(-eatingTriggerHealthPerc, 1.0f - eatingTriggerHealthPerc) / 3;
            eatingTriggerHungerPerc += Random.Range(-eatingTriggerHungerPerc, 1.0f - eatingTriggerHungerPerc) / 3;

            huntChance += Random.Range(-huntChance, 1.0f - huntChance) / 3;
            fleeChance += Random.Range(-fleeChance, 1.0f - fleeChance) / 3;

            int respendPP = 0;
            //shuffle attributePoints
            if (healthPP > 5)
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
            ID = Random.Range(0, 1000);

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

    public static HumanGroupAttributes CombineGenerations(HumanGroupAttributes GenerationA, HumanGroupAttributes GenerationB)
    {
        HumanGroupAttributes temp = GenerationA;

        //Debug.Log("GEN A before combine");
        //for (int i = 0; i < GenerationA.humans.Count; i++)
        //{
        //    Debug.Log(GenerationA.humans[i].individualFitness.ToString());
        //}
        //
        //Debug.Log("GEN BB before combine");
        //for (int i = 0; i < GenerationB.humans.Count; i++)
        //{
        //    Debug.Log(GenerationB.humans[i].individualFitness.ToString());
        //}

        switch (GameObject.FindObjectOfType<UI_script>().combType.value)
        {
            case 0:
                int to_replace = Random.Range(0, GenerationA.humans.Count);
                for (int i = 0; i < to_replace; i++)
                {
                    //randomly get some of their humans
                    temp.humans[Random.Range(0, temp.humans.Count)] = GenerationB.humans[Random.Range(0, GenerationB.humans.Count)];

                }
                break;

            case 1:
                //find idx of highest fitn in genB
                float highestFitness = 0;
                int highestIdx = 0;
                for (int j = 0; j < GenerationB.humans.Count; j++)
                {
                    if (GenerationB.humans[j].individualFitness > highestFitness)
                    {
                        highestIdx = j;
                        highestFitness = GenerationB.humans[j].individualFitness;
                    }
                }

                //find idx of lowest fitn in genA
                float lowestFitness_ = 100000;
                int lowestIdx_ = 0;
                for (int j = 0; j < temp.humans.Count; j++)
                {
                    if (temp.humans[j].individualFitness < lowestFitness_)
                    {
                        lowestIdx_ = j;
                        lowestFitness_ = temp.humans[j].individualFitness;
                    }

                }


                temp.humans[lowestIdx_] = GenerationB.humans[highestIdx];
                break;

            case 2:

                int to_replace_ = Random.Range(0, GenerationA.humans.Count);
                for (int i = 0; i < to_replace_; i++)
                {

                    float lowestFitness = 100000;
                    int lowestIdx = 0;
                    for (int j = 0; j < temp.humans.Count; j++)
                    {
                        if (temp.humans[j].individualFitness < lowestFitness)
                        {
                            lowestIdx = j;
                            lowestFitness = temp.humans[j].individualFitness;
                        }
                    }

                    temp.humans[lowestIdx] = GenerationB.humans[Random.Range(0, GenerationB.humans.Count)];
                }

                break;
        }

        //Debug.Log("GEN A after combine");
        //for (int i = 0; i < GenerationA.humans.Count; i++)
        //{
        //    Debug.Log(GenerationA.humans[i].individualFitness.ToString());
        //}


        return temp;
    }

    public static void RecordText(string text)
    {
        StreamWriter file = new StreamWriter(GameObject.FindObjectOfType<UI_script>().combType.options[GameObject.FindObjectOfType<UI_script>().combType.value].text+".txt", true);
        file.WriteLineAsync(text);
        file.Close();


    }

    public static void RecordHumanAtts(humanAttributes atts)
    {

        string recordSTR = "\nHuman Fitness: " + atts.individualFitness.ToString();
        recordSTR += "\nTime Survived: " + atts.timeSurvived.ToString() +"s";
        recordSTR += "\nDamage Dealt: " + atts.damageDealt.ToString();
        recordSTR += "\nFood Gathered: " + atts.foodGathered.ToString();
        recordSTR += "\n--ATTRIBUTES--: ";
        recordSTR += "\nHealth: " + atts.healthPP.ToString();
        recordSTR += "\nAttack: " + atts.attackPP.ToString();
        recordSTR += "\nMove Speed: " + atts.moveSpeedPP.ToString();
        recordSTR += "\nCarry: " + atts.carryPP.ToString();
        recordSTR += "\nStomach Size: " + atts.stomachSizePP.ToString();
        recordSTR += "\nchance(Hunt/Gather): (" + atts.huntChance.ToString() + "/"+ (1.0f - atts.huntChance).ToString() + ")";
        recordSTR += "\nchance(Fight/Flee): (" + (1.0f-atts.fleeChance).ToString() + "/" + atts.fleeChance.ToString() + ")";
        recordSTR += "\nHunger % Eat Trigger: " + atts.eatingTriggerHungerPerc.ToString();
        recordSTR += "\nHealth % Eat Trigger: " + atts.eatingTriggerHealthPerc.ToString();
        RecordText(recordSTR);
    }

}
