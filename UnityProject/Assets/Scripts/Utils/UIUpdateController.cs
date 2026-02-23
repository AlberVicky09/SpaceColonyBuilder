using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIUpdateController : MonoBehaviour {

    public Dictionary<ResourceEnum, Vector3> resourcesInitialPositions;
    private Dictionary<ResourceEnum, Coroutine> resourceChangeCoroutine;
    private Dictionary<ResourceEnum, bool> missingResourcesFlags;

    private void Start() {
        resourceChangeCoroutine = new Dictionary<ResourceEnum, Coroutine>();
        missingResourcesFlags = new Dictionary<ResourceEnum, bool>();
        Dictionary<ResourceOperationEnum, Coroutine> auxDictionary;
        foreach (ResourceEnum resource in Enum.GetValues(typeof(ResourceEnum))) {
            auxDictionary = new Dictionary<ResourceOperationEnum, Coroutine>();
            foreach (ResourceOperationEnum op in Enum.GetValues(typeof(ResourceOperationEnum))) {
                auxDictionary.Add(op, null);
            }
            resourceChangeCoroutine[resource] = null;
            missingResourcesFlags.Add(resource, false);
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
                if (quantity >= maxPossibleIncrease) {
                    GameControllerScript.Instance.resourcesDictionary[resourceType] = GameControllerScript.Instance.resourcesLimit;
                    GameControllerScript.Instance.uiResourcesTextMap[resourceType].color = Color.yellow;
                    limitedQuantity = maxPossibleIncrease;
                } else {
                    GameControllerScript.Instance.resourcesDictionary[resourceType] += quantity;
                    GameControllerScript.Instance.uiResourcesTextMap[resourceType].color = Color.white;
                }
                
                missingResourcesFlags[resourceType] = false;
                break;
            
            case ResourceOperationEnum.Decrease:
                //If its more than current quantity, remove until 0
                if (quantity > GameControllerScript.Instance.resourcesDictionary[resourceType]) {
                    limitedQuantity = GameControllerScript.Instance.resourcesDictionary[resourceType];
                    GameControllerScript.Instance.resourcesDictionary[resourceType] = 0;
                } else {
                    GameControllerScript.Instance.resourcesDictionary[resourceType] -= quantity;
                }

                if (GameControllerScript.Instance.resourcesDictionary[resourceType] == 0) {
                    GameControllerScript.Instance.uiResourcesTextMap[resourceType].color = Constants.MISSING_RESOURCE_COLOR;
                    
                    //If already in risk, lose the game
                    if (missingResourcesFlags[resourceType]) {
                        GameControllerScript.Instance.missionController.DisplayEndGameCanvas(Constants.LOSE_GAME_TEXT);
                    //Else, alert the player
                    } else {
                        GameControllerScript.Instance.ActivateAlertCanvas("Missing " + resourceType + "\nObtain some or the base will be destroyed!");
                        missingResourcesFlags[resourceType] = true;
                    }
                } else {
                    GameControllerScript.Instance.uiResourcesTextMap[resourceType].color = Color.white;
                }
                
                break;
        }

        //Update screen text
        GameControllerScript.Instance.uiResourcesTextMap[resourceType].text = GameControllerScript.Instance.resourcesDictionary[resourceType].ToString();
        
        //Display update
        if (resourceChangeCoroutine[resourceType] != null) {
            StopCoroutine(resourceChangeCoroutine[resourceType]);
        }
        
        resourceChangeCoroutine[resourceType] =
            StartCoroutine(DisplayResourceChange(resourceType, limitedQuantity, operation));
        
        GameControllerScript.Instance.missionController.CheckResourceMission(resourceType, GameControllerScript.Instance.resourcesDictionary[resourceType]);
    }

    public IEnumerator DisplayResourceChange(ResourceEnum resourceType, int quantity, ResourceOperationEnum operation) {
        //Set quantity and color
        if(ResourceOperationEnum.Increase.Equals(operation)) {
            GameControllerScript.Instance.uiResourcesChangeTextMap[resourceType].color = Constants.GREEN_COLOR;
            GameControllerScript.Instance.uiResourcesChangeTextMap[resourceType].text = "+" + quantity;
        } else {
            GameControllerScript.Instance.uiResourcesChangeTextMap[resourceType].color = Constants.RED_COLOR;
            GameControllerScript.Instance.uiResourcesChangeTextMap[resourceType].text = "-" + quantity;
        }
        
        //Display it and fade it out
        float currentTime = 0f;
        while (GameControllerScript.Instance.uiResourcesTextMap[resourceType].alpha < 1f) {
            Debug.Log("Fading in");
            currentTime += Time.deltaTime;
            GameControllerScript.Instance.uiResourcesChangeTextMap[resourceType].alpha =
                Mathf.Lerp(0, 1, currentTime / Constants.RESOURCE_CHANGE_MOVEMENT_TIME);
            yield return null;
        }

        currentTime = 0f;
        while (GameControllerScript.Instance.uiResourcesChangeTextMap[resourceType].alpha > 0) {
            currentTime += Time.deltaTime;
            GameControllerScript.Instance.uiResourcesChangeTextMap[resourceType].alpha =
                Mathf.Lerp(1, 0, currentTime / Constants.RESOURCE_CHANGE_MOVEMENT_TIME);
            yield return null;
        }
    } 
    
    public IEnumerator DisplayResourceChange_Moving(ResourceEnum resourceType, int quantity, ResourceOperationEnum operation) {
        //Display resource text and set value
        var changeText = GameControllerScript.Instance
            .uiResourcesChangeTextMap[resourceType];

        // HARD RESET (prevents drifting)
        var origin = resourcesInitialPositions[resourceType];
        changeText.transform.position = origin;
        GameControllerScript.Instance.uiResourcesChangeTextMap[resourceType].alpha = 1f;
        
        if(ResourceOperationEnum.Increase.Equals(operation)) {
            GameControllerScript.Instance.uiResourcesChangeTextMap[resourceType].color = Constants.GREEN_COLOR;
            GameControllerScript.Instance.uiResourcesChangeTextMap[resourceType].text = "+" + quantity;
        } else {
            GameControllerScript.Instance.uiResourcesChangeTextMap[resourceType].color = Constants.RED_COLOR;
            GameControllerScript.Instance.uiResourcesChangeTextMap[resourceType].text = "-" + quantity;
        }

        //Get origin and destination position for "bounce"
        float timeEllapsed = 0;
        var destination = origin + Screen.height * Constants.RESOURCE_CHANGE_DISPLACE;
        
        //Move text up
        while (timeEllapsed < Constants.RESOURCE_CHANGE_MOVEMENT_TIME) {

            timeEllapsed += Time.unscaledDeltaTime;

            changeText.rectTransform.position = Vector3.Lerp(
                origin,
                destination,
                timeEllapsed / Constants.RESOURCE_CHANGE_MOVEMENT_TIME
            );

            yield return null;
        }
        
        //Move text down
        timeEllapsed = 0f;
        while (timeEllapsed < Constants.RESOURCE_CHANGE_MOVEMENT_TIME) {

            timeEllapsed += Time.unscaledDeltaTime;

            changeText.rectTransform.position = Vector3.Lerp(
                destination,
                origin,
                timeEllapsed / Constants.RESOURCE_CHANGE_MOVEMENT_TIME
            );
            yield return null;
        }

        GameControllerScript.Instance.uiResourcesChangeTextMap[resourceType].alpha = 0f;
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

            if (resourceLoss != 0) {
                UpdateResource(resource, resourceLoss, ResourceOperationEnum.Decrease);
            }
        }
    }

    public void UpdateMAXResources_TESTONLY(ResourceOperationEnum operationType) {
        foreach (ResourceEnum resource in Enum.GetValues(typeof(ResourceEnum))) {
            UpdateResource(resource, GameControllerScript.Instance.resourcesLimit, operationType);
        }
    }
}