using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class GathererBehaviour : MonoBehaviour
{
    private GameControllerScript gameControllerScript;
    private UIUpdateController uiUpdateController;
    private ClickableShip clickableShip;
    [SerializeField] NavMeshAgent agent;
    
    public GameObject objectiveItem;
    public ResourceEnum resourceGatheringType;
    private OreBehaviour currentGatheredOre;
    public int gathererLoad = 0;
    private Dictionary<ResourceEnum, int> loadDictionary;
    public int maxGathererLoad;
    
    public RectTransform canvas;
    public GameObject actionProgress;
    private Image actionProgressImage;
    public GameObject currentAction;
    public Image currentActionImage;

    private void Start() {
        gameControllerScript = GameObject.Find("GameController").GetComponent<GameControllerScript>();
        uiUpdateController = GameObject.Find("GameController").GetComponent<UIUpdateController>();
        clickableShip = GetComponent<ClickableShip>();
        actionProgressImage = actionProgress.GetComponent<Image>();
        currentActionImage = currentAction.GetComponent<Image>();
        
        maxGathererLoad = Constants.DEFAULT_GATHERER_MAX_LOAD;
        loadDictionary = new Dictionary<ResourceEnum, int>();
        foreach (ResourceEnum resource in Enum.GetValues(typeof(ResourceEnum))) {
            loadDictionary.Add(resource, 0);
        }
    }

    private void Update() {
        Utils.LocateMarkerOverGameObject(gameObject, currentAction.activeSelf ? currentAction : actionProgress, canvas);
    }
    
    private void OnTriggerEnter(Collider other) {
        if(ReferenceEquals(other.gameObject, objectiveItem)) {
            currentGatheredOre = other.GetComponent<OreBehaviour>();
            actionProgress.gameObject.SetActive(true);
            currentAction.gameObject.SetActive(false);
            StartCoroutine(GatheringCoroutine());
        }
    }
    
    private void OnTriggerExit(Collider other) {
        if (currentGatheredOre != null && ReferenceEquals(other.gameObject, currentGatheredOre.gameObject)) {
            StopCoroutine(GatheringCoroutine());
            StopCoroutine(DisplayActionProgress(currentGatheredOre.gatheringTimeRequired));
            currentGatheredOre = null;
        }
    }

    public void UpdateDestination() {
        agent.SetDestination(objectiveItem.transform.position);
    }

    public void ReturnToBase(bool isFull) {
        StartCoroutine(CheckReturnToBaseCompleted(isFull));
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

    private IEnumerator CheckReturnToBaseCompleted(bool isFull) {
        var nearestBase = Utils.FindNearestGameObjectInTupleList(gameObject, gameControllerScript.mainBuildingList);
        agent.SetDestination(nearestBase.transform.position + Constants.BASE_RETREAT_OFFSET);
        while (true) {
            // Check if the agent has reached its destination
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.5f) {
                foreach (var resource in loadDictionary.Keys.ToList()) {
                    if (loadDictionary[resource] != 0) {
                        uiUpdateController.UpdateResource(resource, loadDictionary[resource],
                            ResourceOperationEnum.Increase);
                        loadDictionary[resource] = 0;
                    }
                }
                gathererLoad = 0;
                clickableShip.UpdateTexts();
                Debug.Log("Im in base");
                if(isFull) gameControllerScript.CalculateOreForGatherer(gameObject);
                yield break;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator GatheringCoroutine() {
        StartCoroutine(DisplayActionProgress(currentGatheredOre.gatheringTimeRequired));
        while (currentGatheredOre.gatheredTimes < currentGatheredOre.MAXGATHEREDTIMES) {
            yield return new WaitForSeconds(currentGatheredOre.gatheringTimeRequired);
            gathererLoad += Constants.GATHERER_GATHERING_QUANTITY;
            loadDictionary[currentGatheredOre.resourceType] += Constants.GATHERER_GATHERING_QUANTITY;
            currentGatheredOre.gatheredTimes++;
            clickableShip.UpdateTexts();

            if (gathererLoad >= maxGathererLoad) {
                ReturnToBase(true);
                yield break;
            }
        }

        DisplayAction(gameControllerScript.missingAction);
        gameControllerScript.oreListDictionary[currentGatheredOre.resourceType].RemoveAll(item => item.gameObject.Equals(gameObject));
        gameControllerScript.CalculateOreForGatherer(gameObject);
        Destroy(currentGatheredOre.gameObject);
    }
}
