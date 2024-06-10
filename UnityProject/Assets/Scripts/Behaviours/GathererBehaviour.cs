using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class GathererBehaviour : MonoBehaviour
{
    private GameControllerScript gameControllerScript;
    [SerializeField] NavMeshAgent agent;
    public GameObject objectiveItem;
    public ResourceEnum resourceGatheringType;
    public RectTransform canvas;
    public GameObject actionProgress;
    private Image actionProgressImage;
    public GameObject currentAction;
    public Image currentActionImage;

    private void OnTriggerEnter(Collider other) {
        if(ReferenceEquals(other.gameObject, objectiveItem)) {
            actionProgress.gameObject.SetActive(true);
            currentAction.gameObject.SetActive(false);
        }
    }
    
    private void Start() {
        gameControllerScript = GameObject.Find("GameController").GetComponent<GameControllerScript>();
        actionProgressImage = actionProgress.GetComponent<Image>();
        currentActionImage = currentAction.GetComponent<Image>();
    }

    private void Update() {
        Utils.LocateMarkerOverGameObject(gameObject, currentAction.activeSelf ? currentAction : actionProgress, canvas);
    }

    public void UpdateDestination()
    {
        agent.SetDestination(objectiveItem.transform.position);
    }

    public void ReturnToBase() {
        var nearestBase = Utils.FindNearestGameObjectInTupleList(gameObject, gameControllerScript.mainBuildingList);
        agent.SetDestination(nearestBase.transform.position + Constants.BASE_RETREAT_OFFSET);
    }

    public void DisplayAction(Sprite displayImage) {
        actionProgress.gameObject.SetActive(false);
        currentAction.gameObject.SetActive(true);
        currentActionImage.sprite = displayImage;
    }

    public IEnumerator DisplayActionProgress(float totalActionTime) {
        var progressTime = 0f;
        currentAction.SetActive(false);
        actionProgress.SetActive(true);
        while (progressTime < totalActionTime) {
            currentActionImage.fillAmount = progressTime / totalActionTime;
            progressTime += Time.deltaTime;
            Utils.LocateMarkerOverGameObject(gameObject, currentAction, canvas);
            yield return new WaitForEndOfFrame();
        }
    }
}
