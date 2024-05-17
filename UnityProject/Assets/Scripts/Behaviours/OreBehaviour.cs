using System.Collections;
using UnityEngine;

public class OreBehaviour : MonoBehaviour
{
    public GameControllerScript gameControllerScript;
    public UIUpdateController uiUpdateController;
    public ResourceEnum resourceType;
    private int gatheredTimes = 0;

    private void Start() {
        gameControllerScript = GameObject.Find("GameController").GetComponent<GameControllerScript>();
        uiUpdateController = GameObject.Find("GameController").GetComponent<UIUpdateController>();
    }

    private void OnTriggerEnter(Collider other) { StartCoroutine(GatheringCoroutine(other.gameObject)); }

    private void OnTriggerExit(Collider other) { StopCoroutine(GatheringCoroutine(other.gameObject)); }

    private IEnumerator GatheringCoroutine(GameObject gatherer)
    {
        yield return new WaitForSeconds(2);
        while (gatheredTimes < Constants.MAX_GATHERING_TIMES) {
            uiUpdateController.UpdateResource(resourceType, 3, ResourceOperationEnum.Increase);
            gatheredTimes++;
            yield return new WaitForSeconds(2);
        }

        gameControllerScript.oreListDictionary[resourceType].Remove(gameObject);
        gameControllerScript.CalculateOreForGatherer(gatherer);
        Destroy(this.gameObject);
    }
}
