using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class GathererBehaviour : MonoBehaviour
{
    private GameControllerScript gameControllerScript;
    [SerializeField] NavMeshAgent agent;
    public Transform objectiveItem;
    public ResourceEnum resourceGatheringType;
    public Image actionProgress;
    public Image currentAction;

    public float currentFill = 0f;

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Ore") {
            var resource = other.GetComponent<OreBehaviour>().resourceType;
            DisplayAction(gameControllerScript.oreListImage[resource]);
            actionProgress.fillAmount = currentFill;
            var worldPos = Camera.main.WorldToScreenPoint(transform.position);
            actionProgress.gameObject.SetActive(true);
            currentAction.gameObject.SetActive(true);
            var screenPos = new Vector2(worldPos.x / Camera.main.pixelWidth, worldPos.y / Camera.main.pixelHeight);
            actionProgress.transform.position = screenPos;
            currentAction.transform.position = screenPos;
        }
    }

    private void OnTriggerExit(Collider other) {
        actionProgress.gameObject.SetActive(false);
        currentAction.gameObject.SetActive(false);
    }

    private void Start() {
        gameControllerScript = GameObject.Find("GameController").GetComponent<GameControllerScript>();
    }

    public void UpdateDestination()
    {
        agent.SetDestination(objectiveItem.position);
    }

    public void ReturnToBase() {
        var nearestBase = Utils.FindNearestInList(gameObject, gameControllerScript.mainBuildingList);
        agent.SetDestination(nearestBase.transform.position + Constants.BASE_RETREAT_OFFSET);
    }

    public void DisplayAction(Sprite displayImage) {
        currentAction.sprite = displayImage;
    }
}
