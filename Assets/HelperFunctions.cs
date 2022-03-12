using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CurrentState { idle, goingToEnemy, storingFood, gatheringFood, eating, fighting, fleeing, hunting };




public static class HelperFunctions
{
    //returns true if its arrived
    public static bool goToTargetObject(GameObject subject, GameObject targetObject, float moveSpeed)
    {


        //go towards target object
        if (Vector3.Distance(subject.transform.position,targetObject.transform.position)< 0.2f)
        {
            return true;
            
        }
        else
        {
            Vector3 direction = targetObject.transform.position - subject.transform.position;
            direction.Normalize();
            subject.transform.position += direction * Time.deltaTime * moveSpeed;
        }
        return false;
    }


}
