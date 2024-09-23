using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIUpdateController : MonoBehaviour
{

    public void SetResourcesText() {
        foreach (ResourceEnum resource in Enum.GetValues(typeof(ResourceEnum))) {
            GameControllerScript.Instance.uiResourcesTextMap[resource].text = GameControllerScript.Instance.resourcesDictionary[resource].ToString();
        }
    }

    public void UpdateResource(ResourceEnum resourceType, int quantity, ResourceOperationEnum operation) {
        //Update resources by operation
        switch (operation) {
            case ResourceOperationEnum.Increase:
                GameControllerScript.Instance.resourcesDictionary[resourceType] += quantity;
                break;
            case ResourceOperationEnum.Decrease:
                GameControllerScript.Instance.resourcesDictionary[resourceType] -= quantity;
                break;
        }

        //Update screen text
        GameControllerScript.Instance.uiResourcesTextMap[resourceType].text = GameControllerScript.Instance.resourcesDictionary[resourceType].ToString();
        
        //Display update
        StartCoroutine(GameControllerScript.Instance.UIUpdateController.DisplayResourceChange(resourceType, quantity, operation));
        
        GameControllerScript.Instance.missionController.CheckResourceMission(resourceType, GameControllerScript.Instance.resourcesDictionary[resourceType]);
    }

    public IEnumerator DisplayResourceChange(ResourceEnum resourceType, int quantity, ResourceOperationEnum operation) {
        //Display resource text and set value
        GameControllerScript.Instance.uiResourcesChangeTextMap[resourceType].canvas.SetAlpha(1.0f);

        if(ResourceOperationEnum.Increase.Equals(operation)) {
            GameControllerScript.Instance.uiResourcesChangeTextMap[resourceType].text.color = Constants.RESOURCE_CHANGE_INCREASE_COLOR;
            GameControllerScript.Instance.uiResourcesChangeTextMap[resourceType].text.text = "+" + quantity;
        } else {
            GameControllerScript.Instance.uiResourcesChangeTextMap[resourceType].text.color = Constants.RESOURCE_CHANGE_DECREASE_COLOR;
            GameControllerScript.Instance.uiResourcesChangeTextMap[resourceType].text.text = "-" + quantity;
        }

        //Get origin and destination position for "bounce"
        float timeEllapsed = 0;
        var origin = GameControllerScript.Instance.uiResourcesChangeTextMap[resourceType].text.gameObject.transform.position;
        var destination = origin + Constants.RESOURCE_CHANGE_DISPLACE;

        //Move text up
        while (timeEllapsed < Constants.RESOURCE_CHANGE_MOVEMENT_TIME) {
            GameControllerScript.Instance.uiResourcesChangeTextMap[resourceType].text.gameObject.transform.position = Vector3.Lerp(origin, destination, timeEllapsed / Constants.RESOURCE_CHANGE_MOVEMENT_TIME);
            timeEllapsed += Constants.RESOURCE_CHANGE_MOVEMENT_LERP_TIME;
            yield return new WaitForSeconds(Constants.RESOURCE_CHANGE_MOVEMENT_LERP_TIME);
        }

        //Fade out text
        timeEllapsed = 0;
        GameControllerScript.Instance.uiResourcesChangeTextMap[resourceType].text.CrossFadeAlpha(0.0f, Constants.RESOURCE_CHANGE_MOVEMENT_TIME, true);
        
        //Move text down
        while (timeEllapsed < Constants.RESOURCE_CHANGE_MOVEMENT_TIME) {
            GameControllerScript.Instance.uiResourcesChangeTextMap[resourceType].text.gameObject.transform.position = Vector3.Lerp(destination, origin, timeEllapsed / Constants.RESOURCE_CHANGE_MOVEMENT_TIME);
            timeEllapsed += Constants.RESOURCE_CHANGE_MOVEMENT_LERP_TIME;
            yield return new WaitForSeconds(Constants.RESOURCE_CHANGE_MOVEMENT_LERP_TIME);
        }
    }

    public void ConsumeResources() {
        foreach (ResourceEnum resource in Enum.GetValues(typeof(ResourceEnum))) {
            int resourceLoss = 0;
            //Get mantaining cost of each prop * number of them
            foreach (var prop in GameControllerScript.Instance.propDictionary) {
                resourceLoss += GameControllerScript.Instance.propDictionary[prop.Key].Count * Constants.PROPS_MANTAINING_COST[prop.Key][resource];
            }
            UpdateResource(resource, resourceLoss, ResourceOperationEnum.Decrease);
        }
    }
}