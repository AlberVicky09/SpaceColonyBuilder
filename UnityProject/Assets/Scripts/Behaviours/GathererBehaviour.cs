using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public abstract class GathererBehaviour : MonoBehaviour
{
    protected ClickableOre currentClickableOre;
    [SerializeField] protected ClickableGatherer clickableGatherer;
    [SerializeField] protected NavMeshAgent agent;
    
    public GameObject objectiveItem;
    public ResourceEnum resourceGatheringType;
    protected OreBehaviour currentGatheredOre;
    public int gathererLoad = 0;
    public Dictionary<ResourceEnum, int> loadDictionary;
    public int maxGathererLoad;
    
    public RectTransform canvas;
    public GameObject actionProgress;
    protected float gatheredProgressTime = 0f;
    public Image actionProgressImage;
    public GameObject currentAction;
    public Image currentActionImage;

    private Coroutine gatheringCoroutine;

    private void Update() {
        if (currentAction.activeSelf) {
            Utils.LocateMarkerOverGameObject(gameObject, currentAction, 5f, canvas);
        } else {
            actionProgressImage.fillAmount = gatheredProgressTime;
            gatheredProgressTime += Time.deltaTime;
            Utils.LocateMarkerOverGameObject(gameObject, actionProgress, 5f, canvas);
        }
    }
    
    private void OnTriggerEnter(Collider other) {
        if(ReferenceEquals(other.gameObject, objectiveItem)) {
            currentGatheredOre = other.GetComponent<OreBehaviour>();
            currentClickableOre = other.GetComponent<ClickableOre>();
            actionProgress.gameObject.SetActive(true);
            currentAction.gameObject.SetActive(false);
            gatheringCoroutine = StartCoroutine(GatheringCoroutine());
        }
    }
    
    private void OnTriggerExit(Collider other) {
        if (currentGatheredOre != null && ReferenceEquals(other.gameObject, currentGatheredOre.gameObject)) {
            if (null != gatheringCoroutine) {
                StopCoroutine(GatheringCoroutine());
                gatheringCoroutine = null;
            }
            DisplayAction(GameControllerScript.Instance.oreListImage[resourceGatheringType].sprite);
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

    protected IEnumerator CheckReturnToBaseCompleted(bool isFull) {
        var nearestBase = GetNearestBase();
        agent.SetDestination(nearestBase.transform.position + Constants.BASE_RETREAT_OFFSET);
        while (true) {
            // Check if the agent has reached its destination
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.5f) {
                //Add resources if it had any
                foreach (var resource in loadDictionary.Keys.ToList()) {
                    if (loadDictionary[resource] != 0) {
                        UpdateResource(resource, loadDictionary[resource]);
                        loadDictionary[resource] = 0;
                    }
                }
                gathererLoad = 0;
                clickableGatherer.UpdateTexts();
                if (isFull) CalculateOreForGatherer();
                yield break;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    private IEnumerator GatheringCoroutine() {
        while (currentGatheredOre.gatheredTimes < currentGatheredOre.MAXGATHEREDTIMES) {
            yield return new WaitForSeconds(currentGatheredOre.gatheringTimeRequired);
            gatheredProgressTime = 0f;
            gathererLoad = Mathf.Clamp(gathererLoad + Constants.GATHERER_GATHERING_QUANTITY, 0, maxGathererLoad);
            loadDictionary[currentGatheredOre.resourceType] += Constants.GATHERER_GATHERING_QUANTITY;
            currentGatheredOre.gatheredTimes++;
            clickableGatherer.UpdateTexts();
            currentClickableOre.UpdateTexts();

            if (gathererLoad == maxGathererLoad) {
                ReturnToBase(true);
                Utils.MarkObjectiveAsUnGathered(currentGatheredOre.gameObject,
                    GameControllerScript.Instance.oreListDictionary[resourceGatheringType]);
                yield break;
            }
        }

        if (currentGatheredOre.gatheredTimes == currentGatheredOre.MAXGATHEREDTIMES) {
            RemoveCompletedOre();
            Destroy(currentGatheredOre.gameObject);
            currentGatheredOre = null;
        }

        DisplayAction(GameControllerScript.Instance.returningToBaseAction);
        GameControllerScript.Instance.oreListDictionary[currentGatheredOre.resourceType].RemoveAll(item => item.gameObject.Equals(gameObject));
        CalculateOreForGatherer();
        Destroy(currentGatheredOre.gameObject);
    }
    
    protected abstract void UpdateResource(ResourceEnum resource, int quantity);
    protected abstract void CalculateOreForGatherer();
    protected abstract GameObject GetNearestBase();
    protected abstract void RemoveCompletedOre();
}
