using UnityEngine;

public class PlayerGathererBehaviour : GathererBehaviour
{
    protected override void UpdateResource(ResourceEnum resource, int quantity) {
        GameControllerScript.Instance.uiUpdateController.UpdateResource(resource, quantity,
            ResourceOperationEnum.Increase);
    }

    protected override OreFindingcases CalculateOreForGatherer() {
        return GameControllerScript.Instance.CalculateOreForGatherer(gameObject);
    }

    protected override GameObject GetNearestBase() {
        return GameControllerScript.Instance.propDictionary[PropsEnum.MainBuilding][0];
    }

    public override bool CheckIfResourceIsAtMaximum() {
        return GameControllerScript.Instance.resourcesDictionary[resourceGatheringType]
               == GameControllerScript.Instance.resourcesLimit;
    }
    
    protected override void RemoveCompletedOre(ResourceEnum oreType, GameObject oreToRemove) {
        Utils.RemoveOre(oreType, oreToRemove, GameControllerScript.Instance.gameObject.transform.position);
    }
}