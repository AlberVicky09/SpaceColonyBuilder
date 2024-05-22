using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIUpdateController : MonoBehaviour
{
    public GameControllerScript gameControllerScript;

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

    public IEnumerator DisplayResourceLoss(ResourceEnum resourceType, int quantity) {
        //Display resource loss text and set value
        gameControllerScript.uiResourcesLossTextMap[resourceType].canvas.SetAlpha(1.0f);
        gameControllerScript.uiResourcesLossTextMap[resourceType].text.text = "-" + quantity;

        //Get origin and destination position for "bounce"
        float timeEllapsed = 0;
        var origin = gameControllerScript.uiResourcesLossTextMap[resourceType].text.gameObject.transform.position;
        var destination = origin + Constants.RESOURCE_LOSS_DISPLACE;

        //Move text up
        while (timeEllapsed < Constants.RESOURCE_LOSS_MOVEMENT_TIME) {
            gameControllerScript.uiResourcesLossTextMap[resourceType].text.gameObject.transform.position = Vector3.Lerp(origin, destination, timeEllapsed / Constants.RESOURCE_LOSS_MOVEMENT_TIME);
            timeEllapsed += Constants.RESOURCE_LOSS_MOVEMENT_LERP_TIME;
            yield return new WaitForSeconds(Constants.RESOURCE_LOSS_MOVEMENT_LERP_TIME);
        }

        //Fade out text
        timeEllapsed = 0;
        gameControllerScript.uiResourcesLossTextMap[resourceType].text.CrossFadeAlpha(0.0f, Constants.RESOURCE_LOSS_MOVEMENT_TIME, true);
        
        //Move text down
        while (timeEllapsed < Constants.RESOURCE_LOSS_MOVEMENT_TIME) {
            gameControllerScript.uiResourcesLossTextMap[resourceType].text.gameObject.transform.position = Vector3.Lerp(destination, origin, timeEllapsed / Constants.RESOURCE_LOSS_MOVEMENT_TIME);
            timeEllapsed += Constants.RESOURCE_LOSS_MOVEMENT_LERP_TIME;
            yield return new WaitForSeconds(Constants.RESOURCE_LOSS_MOVEMENT_LERP_TIME);
        }
    }

    public void ConsumeResources() {
        // TODO Calculate how are resources lost depending on buildings and ships
        foreach (ResourceEnum resource in Enum.GetValues(typeof(ResourceEnum))) {
            var resourceLoss =
                gameControllerScript.mainBuildingList.Count * CollectionExtensions.GetValueOrDefault(Constants.MAIN_BUILDING_MANTAINING_COST, resource) +
                gameControllerScript.oreGatherersList.Count * CollectionExtensions.GetValueOrDefault(Constants.ORE_GATHERER_COST, resource);
            UpdateResource(resource, resourceLoss, ResourceOperationEnum.Decrease);
            StartCoroutine(DisplayResourceLoss(resource, resourceLoss));
        }
    }
}
