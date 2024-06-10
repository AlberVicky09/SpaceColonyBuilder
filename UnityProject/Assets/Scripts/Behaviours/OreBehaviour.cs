using System.Collections;
using System.Linq;
using UnityEngine;

public class OreBehaviour : MonoBehaviour
{
    public GameControllerScript gameControllerScript;
    public UIUpdateController uiUpdateController;
    public ResourceEnum resourceType;

    private float gatheringTimeRequired = 3.5f;
    private int gatheredTimes;
    private GathererBehaviour gathererBehaviour;

    private void Start() {
        gameControllerScript = GameObject.Find("GameController").GetComponent<GameControllerScript>();
        uiUpdateController = GameObject.Find("GameController").GetComponent<UIUpdateController>();
    }

    private void OnTriggerEnter(Collider other) {
        if (ReferenceEquals(other.GetComponent<GathererBehaviour>().objectiveItem, gameObject)) {
            StartCoroutine(GatheringCoroutine(other.gameObject));
        }
    }

    private void OnTriggerExit(Collider other) {
        StopCoroutine(GatheringCoroutine(other.gameObject));
    }

    private IEnumerator GatheringCoroutine(GameObject gatherer) {
        gathererBehaviour = gatherer.GetComponent<GathererBehaviour>();
        StartCoroutine(gathererBehaviour.DisplayActionProgress(gatheringTimeRequired));
        while (gatheredTimes < Constants.MAX_GATHERING_TIMES) {
            yield return new WaitForSeconds(gatheringTimeRequired);
            uiUpdateController.UpdateResource(resourceType, 3, ResourceOperationEnum.Increase);
            gatheredTimes++;
        }

        gathererBehaviour.DisplayAction(gameControllerScript.missingAction);
        gameControllerScript.oreListDictionary[resourceType].RemoveAll(item => item.gameObject.Equals(gameObject));
        gameControllerScript.CalculateOreForGatherer(gatherer);
        Destroy(this.gameObject);
    }
}
