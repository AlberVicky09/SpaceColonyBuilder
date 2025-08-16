using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;

public class EnemyBaseController : MonoBehaviour {

    public static EnemyBaseController Instance;
    public GameObject mainEnemyBase;
    public Dictionary<ResourceEnum, int> enemyResourcesDictionary;
    public Dictionary<ResourceEnum, int> enemyGathererObjectiveCount;
    public Dictionary<PropsEnum, List<GameObject>> enemyPropsList;
    private List<ResourceEnum> resourcePrefferenceList;
    private static Vector3 shipGenerationPlace = Constants.ENEMY_CENTER + new Vector3(-5f, 0f, -5f);
    private Vector3 floorCenter = new(75f, -0.1f, 0);
    private PropsEnum currentObjectiveProp = PropsEnum.Gatherer;
    private const float TIME_TO_CHECK_FOR_GENERATOR = 1.5f;
    private float timeSinceLastCheckForGenerator;
    public List<Vector3> waypoints;

    void Awake() { Instance = this; }

    public void ActivateEnemyBase() {
        //Initialize resources dictionaries
        enemyResourcesDictionary = new Dictionary<ResourceEnum, int>();
        enemyGathererObjectiveCount = new Dictionary<ResourceEnum, int>();
        foreach (ResourceEnum resource in Enum.GetValues(typeof(ResourceEnum))) {
            enemyResourcesDictionary.Add(resource, 0);
            enemyGathererObjectiveCount.Add(resource, 0);
        }

        enemyGathererObjectiveCount = new Dictionary<ResourceEnum, int>();
        resourcePrefferenceList = new List<ResourceEnum>();
        
        //Initialize prop dictionary
        enemyPropsList = new Dictionary<PropsEnum, List<GameObject>>();
        foreach (PropsEnum propType in Enum.GetValues(typeof(PropsEnum))) {
            enemyPropsList.Add(propType, new List<GameObject>());
        }
        enemyPropsList[PropsEnum.MainBuilding].Add(gameObject);
        
        //Add floor and bake its navigation
        var floor = Instantiate(GameControllerScript.Instance.floorPrefab, floorCenter, Quaternion.identity);
        floor.GetComponent<NavMeshSurface>().BuildNavMesh();
        floor.GetComponent<MeshRenderer>().enabled = false;
        
        //Add main building
        mainEnemyBase = Instantiate(GameControllerScript.Instance.enemyBasePrefab, Constants.ENEMY_CENTER, Quaternion.identity);
        enemyPropsList[PropsEnum.MainBuilding].Add(mainEnemyBase);
        
        //Add waypoints for fighters to scout
        waypoints = Utils.CalculateWaypointsForBuilding(Constants.ENEMY_CENTER, Constants.numberOfWaypoints, Constants.WAYPOINTS_RADIUS);

        //Add asteroids on this side and add them to gameControllerList
        Utils.GenerateRandomOres(Constants.ENEMY_CENTER);
        
        //Add initial gatherer
        GenerateProp(PropsEnum.Gatherer);
        
        //Activate "update" function
        StartCoroutine(GeneratorCheck());
    }

    private IEnumerator GeneratorCheck() {
        while (true) {
            Debug.Log("Enemy resources " +
                      string.Join(", ", enemyResourcesDictionary.Select(kvp => $"{kvp.Key}={kvp.Value}")));
            if (Utils.CheckEnoughResources(enemyResourcesDictionary,
                    Constants.PROP_CREATION_PRICES[currentObjectiveProp])) {
                Debug.Log("Enemy prop gonna be created: " + currentObjectiveProp);
                GenerateProp(currentObjectiveProp);
            }
            yield return new WaitForSeconds(TIME_TO_CHECK_FOR_GENERATOR);
        }
    }
    
    public void CalculateOreForGatherer(GameObject oreGatherer) {
        //Finds nearest ore of specified type
        var gathererBehaviour = oreGatherer.GetComponent<EnemyGathererBehaviour>();
        
        //Get desired ore type
        resourcePrefferenceList = CalculateDesiredOre();
        GameObject nearestOre = null;
        
        //Find nearest ore in preference order
        FindOreProcess: 
            foreach (var prefferedResource in resourcePrefferenceList) {
                if (nearestOre == null) {
                    nearestOre = Utils.FindNearestGameObjectInList(oreGatherer, GameControllerScript.Instance.oreListDictionary[prefferedResource]);
                    gathererBehaviour.resourceGatheringType = prefferedResource;
                }
            }

        //If null, generate more ores around enemy base
        if (nearestOre == null) {
            Utils.GenerateRandomOres(Constants.ENEMY_CENTER);
            goto FindOreProcess;
        }
        
        gathererBehaviour.objectiveItem = nearestOre;
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
        Debug.Log("Enemy resources " + string.Join(", ", enemyResourcesDictionary.Select(kvp => $"{kvp.Key}={kvp.Value}")));
    }

    private GameObject GenerateProp(PropsEnum propToGenerate) {
        switch (propToGenerate) {
            case PropsEnum.Gatherer:
                var gatherer = Instantiate(GameControllerScript.Instance.enemyGathererPrefab, shipGenerationPlace, Quaternion.identity);
                enemyPropsList[PropsEnum.Gatherer].Add(gatherer);
                CalculateOreForGatherer(gatherer);
                Debug.Log("Enemy gatherer generated");
                return gatherer;
            case PropsEnum.FoodGenerator:
                //TODO Where to place?
                var generator = Instantiate(GameControllerScript.Instance.enemyGathererPrefab, CalculateGeneratorLocation(), Quaternion.identity);
                enemyPropsList[PropsEnum.FoodGenerator].Add(generator);
                Debug.Log("Enemy generator generated");
                return generator;
            case PropsEnum.BasicFighter:
                var fighter = Instantiate(GameControllerScript.Instance.enemyFighterPrefab, shipGenerationPlace, Quaternion.identity);
                enemyPropsList[PropsEnum.BasicEnemy].Add(fighter);
        
                //If we have more than 3, go to attack base, else scout automatically
                if (enemyPropsList[PropsEnum.BasicEnemy].Count > 3) {
                    Debug.Log("Enemy fighter in attack mode");
                    foreach (var enemy in enemyPropsList[PropsEnum.BasicEnemy]) {
                        enemy.GetComponent<EnemyFighterBehaviour>().AttackPlayerBase();
                    }
                }

                Debug.Log("Enemy fighter generated");
                return fighter;
        }
        return null;
    }

    private List<ResourceEnum> CalculateDesiredOre() {
        //Get desired resources
        var resourceList = Constants.ENEMY_RESOURCE_PREFFERENCE[currentObjectiveProp];
        var duplicatedList = new List<ResourceEnum>(resourceList);
        
        //Move back resources already had
        foreach (var wantedResource in resourceList) {
            //Remove it from its position and add it in the back of the list
            if (enemyResourcesDictionary[wantedResource] > Constants.PROP_CREATION_PRICES[currentObjectiveProp][wantedResource]) {
                duplicatedList.Remove(wantedResource);
                duplicatedList.Add(wantedResource);
            }
        }
        
        //Add non desired resources
        foreach (var unwantedResource in Constants.UNUSUED_ENEMY_RESOURCE_PREFFERENCE[currentObjectiveProp]) {
            duplicatedList.Add(unwantedResource);
        }

        return duplicatedList;
    }

    private Vector3 CalculateGeneratorLocation() {
        //TODO Slowly surround main base
        return Constants.ENEMY_CENTER + new Vector3(5f, 0f, 5f);
    }
}
