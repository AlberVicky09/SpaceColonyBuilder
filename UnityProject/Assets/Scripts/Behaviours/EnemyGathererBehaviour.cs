using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGathererBehaviour : GathererBehaviour {
    public bool isFirstTimeGathering = true;
    
    private void Start() {
        maxGathererLoad = Constants.DEFAULT_GATHERER_MAX_LOAD;
        loadDictionary = new Dictionary<ResourceEnum, int>();
        foreach (ResourceEnum resource in Enum.GetValues(typeof(ResourceEnum))) {
            loadDictionary.Add(resource, 0);
        }
        
        EnemyBaseController.Instance.CalculateOreForGatherer(gameObject);
    }

    protected override void UpdateResource(ResourceEnum resource, int quantity) {
        Debug.Log("Storing in ENEMY resources " + quantity + " of " + resource);
        EnemyBaseController.Instance.UpdateResource(resource, quantity, ResourceOperationEnum.Increase);
    }

    protected override void CalculateOreForGatherer() {
        EnemyBaseController.Instance.CalculateOreForGatherer(gameObject);
    }
    
    protected override GameObject GetNearestBase() {
        return GameControllerScript.Instance.propDictionary[PropsEnum.EnemyBase][0];
    }

    protected override void RemoveCompletedOre(ResourceEnum oreType, GameObject oreToRemove) {
        Utils.RemoveOre(oreType, oreToRemove, EnemyBaseController.Instance.gameObject.transform.position);
    }
}
