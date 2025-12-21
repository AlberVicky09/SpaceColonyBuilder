using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public abstract class GathererBehaviour : ActionUIController_v2
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

    private Coroutine gatheringCoroutine;
    private bool isGatheringStopping;
    
    private void OnTriggerEnter(Collider other) {
        if(ReferenceEquals(other.gameObject, objectiveItem)) {
            currentGatheredOre = other.GetComponent<OreBehaviour>();
            currentClickableOre = other.GetComponent<ClickableOre>();
            totalProgressTime = currentGatheredOre.GATHERING_TIME_REQUIRED;
            gatheringCoroutine = StartCoroutine(GatheringCoroutine());
        }
    }
    
    private void OnTriggerExit(Collider other) {
        if (currentGatheredOre != null && ReferenceEquals(other.gameObject, currentGatheredOre.gameObject)) {
            if (null != gatheringCoroutine) {
                isGatheringStopping = true;
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
        if (null != gatheringCoroutine) {
            isGatheringStopping = true;
            StopCoroutine(GatheringCoroutine());
            gatheringCoroutine = null;
        }
        StartCoroutine(CheckReturnToBaseCompleted(isRetreating));
    }

    protected IEnumerator CheckReturnToBaseCompleted(bool isRetreating) {
        var nearestBase = GetNearestBase();
        var calculatePositionAroundBase = Utils.CalculateRandomPositionAroundBase(nearestBase);
        agent.SetDestination(calculatePositionAroundBase);
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
                try { currentClickableOre.UpdateTexts(); } catch (Exception) {}

                if (isRetreating) {
                    DisplayAction(GameControllerScript.Instance.stopActionSprite);
                } else {
                    CalculateOreForGatherer();
                }
                yield break;
            }
            yield return new WaitForSeconds(0.25f);
        }
    }
    
    private IEnumerator GatheringCoroutine() {
        DisplayProgress();
        while (currentGatheredOre.gatheredTimes < currentGatheredOre.MAXGATHEREDTIMES) {
            yield return new WaitForSeconds(currentGatheredOre.GATHERING_TIME_REQUIRED);
            //Ensure coroutine hasnt been stopped
            if (isGatheringStopping) {
                isGatheringStopping = false;
                yield break;
            }
            
            progressTime = 0f;
            gathererLoad = Mathf.Clamp(gathererLoad + Constants.GATHERER_GATHERING_QUANTITY, 0, maxGathererLoad);
            //TODO Cheating
            //gathererLoad = Constants.INITIAL_RESOURCES_LIMIT;
            loadDictionary[currentGatheredOre.resourceType] += gathererLoad;
            currentGatheredOre.gatheredTimes++;
            clickableGatherer.UpdateTexts();
            currentClickableOre.UpdateTexts();

            //If gatherer is full, exit coroutine
            if (gathererLoad >= maxGathererLoad) {
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
