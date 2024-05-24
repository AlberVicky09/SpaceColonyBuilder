using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIUpdateController : MonoBehaviour
{
    public GameControllerScript gameControllerScript;

    public void SetResourcesText() {
        foreach (ResourceEnum resource in Enum.GetValues(typeof(ResourceEnum))) {
            gameControllerScript.uiResourcesTextMap[resource].text = gameControllerScript.resourcesDictionary[resource].ToString();
        }
    }

    public void UpdateResource(ResourceEnum resourceType, int quantity, ResourceOperationEnum operation) {
        //Update resources by operation
        switch (operation) {
            case ResourceOperationEnum.Increase:
                gameControllerScript.resourcesDictionary[resourceType] += quantity;
                break;
            case ResourceOperationEnum.Decrease:
                gameControllerScript.resourcesDictionary[resourceType] -= quantity;
                break;
        }

        //Update screen text
        gameControllerScript.uiResourcesTextMap[resourceType].text = gameControllerScript.resourcesDictionary[resourceType].ToString();
    }

    public IEnumerator DisplayResourceChange(ResourceEnum resourceType, int quantity, ResourceOperationEnum operation) {
        //Display resource text and set value
        gameControllerScript.uiResourcesChangeTextMap[resourceType].canvas.SetAlpha(1.0f);

        if(ResourceOperationEnum.Increase.Equals(operation)) {
            gameControllerScript.uiResourcesChangeTextMap[resourceType].text.color = Constants.RESOURCE_CHANGE_INCREASE_COLOR;
            gameControllerScript.uiResourcesChangeTextMap[resourceType].text.text = "+" + quantity;
        } else {
            gameControllerScript.uiResourcesChangeTextMap[resourceType].text.color = Constants.RESOURCE_CHANGE_DECREASE_COLOR;
            gameControllerScript.uiResourcesChangeTextMap[resourceType].text.text = "-" + quantity;
        }

        //Get origin and destination position for "bounce"
        float timeEllapsed = 0;
        var origin = gameControllerScript.uiResourcesChangeTextMap[resourceType].text.gameObject.transform.position;
        var destination = origin + Constants.RESOURCE_CHANGE_DISPLACE;

        //Move text up
        while (timeEllapsed < Constants.RESOURCE_CHANGE_MOVEMENT_TIME) {
            gameControllerScript.uiResourcesChangeTextMap[resourceType].text.gameObject.transform.position = Vector3.Lerp(origin, destination, timeEllapsed / Constants.RESOURCE_CHANGE_MOVEMENT_TIME);
            timeEllapsed += Constants.RESOURCE_CHANGE_MOVEMENT_LERP_TIME;
            yield return new WaitForSeconds(Constants.RESOURCE_CHANGE_MOVEMENT_LERP_TIME);
        }

        //Fade out text
        timeEllapsed = 0;
        gameControllerScript.uiResourcesChangeTextMap[resourceType].text.CrossFadeAlpha(0.0f, Constants.RESOURCE_CHANGE_MOVEMENT_TIME, true);
        
        //Move text down
        while (timeEllapsed < Constants.RESOURCE_CHANGE_MOVEMENT_TIME) {
            gameControllerScript.uiResourcesChangeTextMap[resourceType].text.gameObject.transform.position = Vector3.Lerp(destination, origin, timeEllapsed / Constants.RESOURCE_CHANGE_MOVEMENT_TIME);
            timeEllapsed += Constants.RESOURCE_CHANGE_MOVEMENT_LERP_TIME;
            yield return new WaitForSeconds(Constants.RESOURCE_CHANGE_MOVEMENT_LERP_TIME);
        }
    }

    public void ConsumeResources() {
        foreach (ResourceEnum resource in Enum.GetValues(typeof(ResourceEnum))) {
            var resourceLoss =
                gameControllerScript.mainBuildingList.Count * CollectionExtensions.GetValueOrDefault(Constants.MAIN_BUILDING_MANTAINING_COST, resource) +
                gameControllerScript.oreGatherersList.Count * CollectionExtensions.GetValueOrDefault(Constants.ORE_GATHERER_COST, resource);
            UpdateResource(resource, resourceLoss, ResourceOperationEnum.Decrease);
            StartCoroutine(DisplayResourceChange(resource, resourceLoss, ResourceOperationEnum.Decrease));
        }
    }
}