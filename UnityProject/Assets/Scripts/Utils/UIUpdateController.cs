using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIUpdateController : MonoBehaviour {

    public Dictionary<ResourceEnum, Vector3> resourcesInitialPositions;
    private Dictionary<ResourceEnum, Dictionary<ResourceOperationEnum, Coroutine>> resourcesChangeCoroutines;

    private void Start() {
        resourcesChangeCoroutines = new Dictionary<ResourceEnum, Dictionary<ResourceOperationEnum, Coroutine>>();
        Dictionary<ResourceOperationEnum, Coroutine> auxDictionary;
        foreach (ResourceEnum resource in Enum.GetValues(typeof(ResourceEnum))) {
            auxDictionary = new Dictionary<ResourceOperationEnum, Coroutine>();
            foreach (ResourceOperationEnum op in Enum.GetValues(typeof(ResourceOperationEnum))) {
                auxDictionary.Add(op, null);
            }
            resourcesChangeCoroutines.Add(resource, auxDictionary);
        }
    }

    public void SetResourcesText() {
        foreach (ResourceEnum resource in Enum.GetValues(typeof(ResourceEnum))) {
            GameControllerScript.Instance.uiResourcesTextMap[resource].text = GameControllerScript.Instance.resourcesDictionary[resource].ToString();
        }
    }

    public void UpdateResource(ResourceEnum resourceType, int quantity, ResourceOperationEnum operation) {
        //Update resources by operation
        int limitedQuantity = quantity;
        switch (operation) {
            case ResourceOperationEnum.Increase:
                //If is more than limit, increase until limit
                var maxPossibleIncrease = GameControllerScript.Instance.resourcesLimit -
                                          GameControllerScript.Instance.resourcesDictionary[resourceType];
                
                if (maxPossibleIncrease > quantity) {
                    GameControllerScript.Instance.resourcesDictionary[resourceType] = GameControllerScript.Instance.resourcesLimit;
                    GameControllerScript.Instance.uiResourcesTextMap[resourceType].color = Color.yellow;
                    limitedQuantity = maxPossibleIncrease;
                } else {
                    GameControllerScript.Instance.resourcesDictionary[resourceType] += quantity;
                    GameControllerScript.Instance.uiResourcesTextMap[resourceType].color = Color.white;
                }
                
                GameControllerScript.Instance.uiResourcesTextMap[resourceType].color = 
                    GameControllerScript.Instance.resourcesLimit == GameControllerScript.Instance.resourcesDictionary[resourceType] ?
                        Color.yellow : Color.white;
                break;
            case ResourceOperationEnum.Decrease:
                //If its more than current quantity, remove until 0
                if (quantity > GameControllerScript.Instance.resourcesDictionary[resourceType]) {
                    limitedQuantity = GameControllerScript.Instance.resourcesDictionary[resourceType];
                    GameControllerScript.Instance.resourcesDictionary[resourceType] = 0;
                    GameControllerScript.Instance.uiResourcesTextMap[resourceType].color = Color.grey;

                } else {
                    GameControllerScript.Instance.resourcesDictionary[resourceType] -= quantity;
                }

                GameControllerScript.Instance.uiResourcesTextMap[resourceType].color = 
                    GameControllerScript.Instance.resourcesDictionary[resourceType] == 0 ?
                        Color.grey : Color.white;
                break;
        }

        //Update screen text
        GameControllerScript.Instance.uiResourcesTextMap[resourceType].text = GameControllerScript.Instance.resourcesDictionary[resourceType].ToString();
        
        //Display update
        try { StopCoroutine(resourcesChangeCoroutines[resourceType][operation]); } catch { }

        resourcesChangeCoroutines[resourceType][operation] = StartCoroutine(DisplayResourceChange(resourceType, limitedQuantity, operation));
        
        GameControllerScript.Instance.missionController.CheckResourceMission(resourceType, GameControllerScript.Instance.resourcesDictionary[resourceType]);
    }

    public IEnumerator DisplayResourceChange(ResourceEnum resourceType, int quantity, ResourceOperationEnum operation) {
        //Display resource text and set value
        GameControllerScript.Instance.uiResourcesTextMap[resourceType].transform.position = resourcesInitialPositions[resourceType];
        GameControllerScript.Instance.uiResourcesChangeTextMap[resourceType].canvas.SetAlpha(1.0f);

        if(ResourceOperationEnum.Increase.Equals(operation)) {
            GameControllerScript.Instance.uiResourcesChangeTextMap[resourceType].text.color = Constants.GREEN_COLOR;
            GameControllerScript.Instance.uiResourcesChangeTextMap[resourceType].text.text = "+" + quantity;
        } else {
            GameControllerScript.Instance.uiResourcesChangeTextMap[resourceType].text.color = Constants.RED_COLOR;
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
            foreach (var prop in BuildableProps.RetrieveBuildableProps()) {
                resourceLoss += GameControllerScript.Instance.propDictionary[prop].Count
                                * Constants.PROPS_MANTAINING_COST[prop]
                                    .GetValueOrDefault(resource, Constants.DEFAULT_MISSING_RESOURCE_VALUE);
            }
            Debug.Log("Resource loss = " + resource + " [" + resourceLoss + "]");
            UpdateResource(resource, resourceLoss, ResourceOperationEnum.Decrease);
        }
    }
}