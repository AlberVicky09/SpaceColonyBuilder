using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class GathererBehaviour : MonoBehaviour
{
    private ClickableShip clickableShip;
    private ClickableOre currentClickableOre;
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
        Utils.LocateMarkerOverGameObject(gameObject, currentAction.activeSelf ? currentAction : actionProgress, 5f, canvas);
    }
    
    private void OnTriggerEnter(Collider other) {
        if(ReferenceEquals(other.gameObject, objectiveItem)) {
            currentGatheredOre = other.GetComponent<OreBehaviour>();
            currentClickableOre = other.GetComponent<ClickableOre>();
            actionProgress.gameObject.SetActive(true);
            currentAction.gameObject.SetActive(false);
            StartCoroutine(GatheringCoroutine());
        }
    }
    
    private void OnTriggerExit(Collider other) {
        if (currentGatheredOre != null && ReferenceEquals(other.gameObject, currentGatheredOre.gameObject)) {
            StopCoroutine(GatheringCoroutine());
            DisplayAction(GameControllerScript.Instance.goingToAction);
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

    private IEnumerator CheckReturnToBaseCompleted(bool isFull) {
        var nearestBase = Utils.FindNearestGameObjectInTupleList(gameObject, GameControllerScript.Instance.propDictionary[PropsEnum.MainBuilding]);
        agent.SetDestination(nearestBase.transform.position + Constants.BASE_RETREAT_OFFSET);
        while (true) {
            // Check if the agent has reached its destination
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.5f) {
                foreach (var resource in loadDictionary.Keys.ToList()) {
                    if (loadDictionary[resource] != 0) {
                        GameControllerScript.Instance.uiUpdateController.UpdateResource(resource, loadDictionary[resource],
                            ResourceOperationEnum.Increase);
                        loadDictionary[resource] = 0;
                    }
                }
                gathererLoad = 0;
                clickableShip.UpdateTexts();
                Debug.Log("Im in base");
                if(isFull) GameControllerScript.Instance.CalculateOreForGatherer(gameObject);
                yield break;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator GatheringCoroutine() {
        while (currentGatheredOre.gatheredTimes < currentGatheredOre.MAXGATHEREDTIMES) {
            yield return new WaitForSeconds(currentGatheredOre.gatheringTimeRequired);
            gathererLoad += Constants.GATHERER_GATHERING_QUANTITY;
            loadDictionary[currentGatheredOre.resourceType] += Constants.GATHERER_GATHERING_QUANTITY;
            currentGatheredOre.gatheredTimes++;
            clickableShip.UpdateTexts();
            currentClickableOre.UpdateTexts();

            if (gathererLoad >= maxGathererLoad) {
                ReturnToBase(true);
                Utils.MarkObjectiveAsUnGathered(currentGatheredOre.gameObject,
                    GameControllerScript.Instance.oreListDictionary[resourceGatheringType]);
                yield break;
            }
        }

        if (currentGatheredOre.gatheredTimes == currentGatheredOre.MAXGATHEREDTIMES) {
            GameControllerScript.Instance.RemoveOre();
            Destroy(currentGatheredOre.gameObject);
            currentGatheredOre = null;
        }

        DisplayAction(GameControllerScript.Instance.missingAction);
        GameControllerScript.Instance.oreListDictionary[currentGatheredOre.resourceType].RemoveAll(item => item.gameObject.Equals(gameObject));
        GameControllerScript.Instance.CalculateOreForGatherer(gameObject);
        Destroy(currentGatheredOre.gameObject);
    }
}
