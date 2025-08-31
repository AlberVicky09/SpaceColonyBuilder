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
    protected float gatheredProgressTime = 0f;
    
    public RectTransform canvas;
    public GameObject actionProgress;
    public Image actionProgressImage;
    public GameObject currentAction;
    public Image currentActionImage;

    private Coroutine gatheringCoroutine;

    private void Update() {
        if (currentAction.activeSelf) {
            Utils.LocateMarkerOverGameObject(gameObject, currentAction, 5f, canvas);
        } else {
            actionProgressImage.fillAmount = gatheredProgressTime / currentGatheredOre.gatheringTimeRequired;
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
            currentGatheredOre = null;
        }
    }

    public void UpdateDestination() {
        agent.SetDestination(objectiveItem.transform.position);
    }

    public void ReturnToBase(bool isRetreating) {
        StartCoroutine(CheckReturnToBaseCompleted(isRetreating));
    }

    public void DisplayAction(Sprite displayImage) {
        actionProgress.gameObject.SetActive(false);
        currentAction.gameObject.SetActive(true);
        currentActionImage.sprite = displayImage;
    }

    protected IEnumerator CheckReturnToBaseCompleted(bool isRetreating) {
        var nearestBase = GetNearestBase();
        agent.SetDestination(nearestBase.transform.position + Constants.BASE_RETREAT_OFFSET);
        DisplayAction(GameControllerScript.Instance.returningToBaseSprite);
        
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
                if (isRetreating) {
                    DisplayAction(GameControllerScript.Instance.stopActionSprite);
                } else {
                    CalculateOreForGatherer();
                }
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

            //If gatherer is full, exit coroutine
            if (gathererLoad == maxGathererLoad) {
                ReturnToBase(false);
                Utils.MarkObjectiveAsUnGathered(currentGatheredOre.gameObject,
                    GameControllerScript.Instance.oreListDictionary[resourceGatheringType]);
                yield break;
            }
        }

        //Destroy empty ore
        RemoveCompletedOre(currentGatheredOre.resourceType, currentGatheredOre.gameObject);
        currentGatheredOre = null;
        
        //Look for another ore of the same type
        CalculateOreForGatherer();
    }
    
    protected abstract void UpdateResource(ResourceEnum resource, int quantity);
    protected abstract void CalculateOreForGatherer();
    protected abstract GameObject GetNearestBase();
    protected abstract void RemoveCompletedOre(ResourceEnum oreType, GameObject oreToRemove);
}
