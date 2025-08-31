using UnityEngine;

public class PlayerGathererBehaviour : GathererBehaviour
{
    protected override void UpdateResource(ResourceEnum resource, int quantity) {
        Debug.Log("Storing in PLAYER resources " + quantity + " of " + resource);
        GameControllerScript.Instance.uiUpdateController.UpdateResource(resource, quantity,
            ResourceOperationEnum.Increase);
    }

    protected override void CalculateOreForGatherer() {
        GameControllerScript.Instance.CalculateOreForGatherer(gameObject);
    }

    protected override GameObject GetNearestBase() {
        return GameControllerScript.Instance.propDictionary[PropsEnum.MainBuilding][0];
    }

    protected override void RemoveCompletedOre(ResourceEnum oreType, GameObject oreToRemove) {
        Utils.RemoveOre(oreType, oreToRemove, GameControllerScript.Instance.gameObject.transform.position);
    }
}