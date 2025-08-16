using UnityEngine;

public class EnemyGathererBehaviour : GathererBehaviour
{
    private void Start() {
        base.Start();
        
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
        return EnemyBaseController.Instance.mainEnemyBase;
    }

    protected override void RemoveCompletedOre() {
        Utils.RemoveOre(EnemyBaseController.Instance.gameObject.transform.position);
    }
}
