using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;

public class EnemyBaseController : MonoBehaviour {

    public static EnemyBaseController Instance;
    public Dictionary<ResourceEnum, int> enemyResourcesDictionary;
    public Dictionary<ResourceEnum, int> enemyGathererObjectiveCount;
    private List<ResourceEnum> resourcePrefferenceList;
    private Dictionary<ResourceEnum, int> resourcePrefferenceDictionary;
    private Vector3 floorCenter = new(75f, -0.1f, 0);
    private PropsEnum currentObjectiveProp = PropsEnum.EnemyGatherer;
    private const float TIME_TO_CHECK_FOR_GENERATOR = 1.5f;
    private float timeSinceLastCheckForGenerator;
    public NavMeshSurface originalFloor;
    public List<Vector3> waypoints;

    void Awake() { Instance = this; }

    public void ActivateEnemyBase() {
        //Initialize resources dictionaries
        enemyResourcesDictionary = new Dictionary<ResourceEnum, int>();
        enemyGathererObjectiveCount = new Dictionary<ResourceEnum, int>();
        resourcePrefferenceDictionary = new Dictionary<ResourceEnum, int>();
        foreach (ResourceEnum resource in Enum.GetValues(typeof(ResourceEnum))) {
            enemyResourcesDictionary.Add(resource, 0);
            enemyGathererObjectiveCount.Add(resource, 0);
            resourcePrefferenceDictionary.Add(resource, 0);
        }

        resourcePrefferenceList = new List<ResourceEnum>();
        
        //Add floor and bake its navigation
        var floor = Instantiate(GameControllerScript.Instance.floorPrefab, floorCenter, Quaternion.identity);
        originalFloor.GetComponent<MeshRenderer>().enabled = true;
        originalFloor.GetComponent<NavMeshSurface>().BuildNavMesh();
        floor.GetComponent<NavMeshSurface>().BuildNavMesh();
        originalFloor.GetComponent<MeshRenderer>().enabled = false;
        floor.GetComponent<MeshRenderer>().enabled = false;
        
        //Add main building
        var enemyBaseGO = Instantiate(GameControllerScript.Instance.enemyBasePrefab, Constants.ENEMY_CENTER, Quaternion.identity);
        enemyBaseGO.name = "EnemyBase";
        GameControllerScript.Instance.propDictionary[PropsEnum.EnemyBase].Add(enemyBaseGO);
        //Add waypoints for fighters to scout
        waypoints = Utils.CalculateWaypointsForBuilding(Constants.ENEMY_CENTER, Constants.numberOfWaypoints, Constants.WAYPOINTS_RADIUS);

        //Add asteroids on this side and add them to gameControllerList
        Utils.GenerateRandomOres(Constants.ENEMY_CENTER);
        
        //Add initial gatherer
        GenerateProp(PropsEnum.EnemyGatherer);

        //Activate "update" function
        StartCoroutine(GeneratorCheck());
    }

    private IEnumerator GeneratorCheck() {
        while (true) {
            if (Utils.CheckEnoughResources(enemyResourcesDictionary,
                    Constants.PROP_CREATION_PRICES[currentObjectiveProp])) {
                //Remove spent resources
                foreach (var (type, cost) in Constants.PROP_CREATION_PRICES[currentObjectiveProp]) {
                    UpdateResource(type, cost, ResourceOperationEnum.Decrease);
                }
                GenerateProp(currentObjectiveProp);
            }
            yield return new WaitForSeconds(TIME_TO_CHECK_FOR_GENERATOR);
        }
    }
    
    public void CalculateOreForGatherer(GameObject oreGatherer) {
        var gathererBehaviour = oreGatherer.GetComponent<EnemyGathererBehaviour>();
        
        //Remove from current gathering map if it was gathering
        if (!gathererBehaviour.isFirstTimeGathering) {
            resourcePrefferenceDictionary[gathererBehaviour.resourceGatheringType]--;
        }
        
        //Get desired ore type in prefference order
        resourcePrefferenceList = CalculateDesiredOre();
        GameObject nearestOre = null;
        
        //Find nearest ore in preference order
        FindOreProcess: 
            foreach (var prefferedResource in resourcePrefferenceList) {
                nearestOre = Utils.FindNearestGameObjectInList(oreGatherer, GameControllerScript.Instance.oreListDictionary[prefferedResource]);
                gathererBehaviour.resourceGatheringType = prefferedResource;
                break;
            }

        //If null, generate more ores around enemy base
        if (nearestOre == null) {
            Utils.GenerateRandomOres(Constants.ENEMY_CENTER);
            goto FindOreProcess;
        }
        
        //Display current ore action
        resourcePrefferenceDictionary[gathererBehaviour.resourceGatheringType]++;
        gathererBehaviour.isFirstTimeGathering = false;
        gathererBehaviour.objectiveItem = nearestOre;
        gathererBehaviour.DisplayAction(GameControllerScript.Instance.resourceSpriteDictionary[gathererBehaviour.resourceGatheringType]);
        gathererBehaviour.UpdateDestination();
    }

    public void UpdateResource(ResourceEnum resourceType, int quantity, ResourceOperationEnum operation) {
        //Update resources by operation
        switch (operation) {
            case ResourceOperationEnum.Increase:
                //If is more than limit, increase until limit
                var maxPossibleIncrease = Constants.INITIAL_RESOURCES_LIMIT - enemyResourcesDictionary[resourceType];
                enemyResourcesDictionary[resourceType] = maxPossibleIncrease < quantity ?
                    GameControllerScript.Instance.resourcesLimit : quantity;
                break;
            case ResourceOperationEnum.Decrease:
                //If its more than current quantity, remove until 0
                enemyResourcesDictionary[resourceType] = quantity > enemyResourcesDictionary[resourceType] ? 
                    0 : enemyResourcesDictionary[resourceType] - quantity;
                break;
        }
    }

    private GameObject GenerateProp(PropsEnum propToGenerate) {
        var calculatePositionAroundBase = Utils.CalculateRandomPositionAroundBase(GameControllerScript.Instance.propDictionary[PropsEnum.EnemyBase][0]);
        switch (propToGenerate) {
            case PropsEnum.EnemyGatherer:
                var gatherer = Instantiate(GameControllerScript.Instance.enemyGathererPrefab, calculatePositionAroundBase, Quaternion.identity);
                GameControllerScript.Instance.propDictionary[PropsEnum.EnemyGatherer].Add(gatherer);
                CalculateOreForGatherer(gatherer);
                if (GameControllerScript.Instance.propDictionary[PropsEnum.EnemyGatherer].Count >= 3) {
                    currentObjectiveProp = PropsEnum.EnemyFighter;
                }
                return gatherer;
            case PropsEnum.EnemyFighter:
                var fighter = Instantiate(GameControllerScript.Instance.enemyFighterPrefab, calculatePositionAroundBase, Quaternion.identity);
                fighter.name = "EnemyFighter";
                GameControllerScript.Instance.propDictionary[PropsEnum.EnemyFighter].Add(fighter);
                //If we have more than 3, go to attack base, else scout automatically
                if (GameControllerScript.Instance.propDictionary[PropsEnum.EnemyFighter].Count > 3) {
                    foreach (var enemy in GameControllerScript.Instance.propDictionary[PropsEnum.EnemyFighter]) {
                        enemy.GetComponent<EnemyFighterBehaviour>().StartChasingBase();
                    }
                }
                return fighter;
        }
        return null;
    }

    private List<ResourceEnum> CalculateDesiredOre() {
        //Get desired resources
        var resourceList = Constants.PROP_CREATION_PRICES[currentObjectiveProp]
            .OrderByDescending(kvp => kvp.Value)
            .ToList();
        var duplicatedList = new List<ResourceEnum>(resourceList.Select(kvp => kvp.Key));
        
        //Move back resources already had or already being gathered
        foreach (var wantedResource in resourceList) {
            //Remove it from its position and add it in the back of the list
            if (enemyResourcesDictionary[wantedResource.Key] > Constants.PROP_CREATION_PRICES[currentObjectiveProp][wantedResource.Key]
                || resourcePrefferenceDictionary[wantedResource.Key] > 0) {
                duplicatedList.Remove(wantedResource.Key);
                duplicatedList.Add(wantedResource.Key);
            }
        }
        

        //Add non desired resources (we remove the ones already in resourceList
        Enum.GetValues(typeof(ResourceEnum))
            .Cast<ResourceEnum>()
            .Except(resourceList.Select(kvp => kvp.Key))
            .ToList()
            .ForEach(unwantedResource => duplicatedList.Add(unwantedResource));

        return duplicatedList;
    }
}
