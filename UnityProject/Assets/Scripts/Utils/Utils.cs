using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public static class Utils {

    public static void GenerateRandomOres(Vector2 centerOfOres) {
        //Generate ores in random position inside initial circle
        for (int i = 0; i < Constants.INITIAL_ORE_NUMBER; i++) {
            var orePos = GenerateNewOrePositionInCircle(centerOfOres);
            //var orePos = GenerateNewOrePositionInSquare();
            var randomResource = OreResources.GetRandomOreResource();
            var instantiatedOre = Object.Instantiate(GameControllerScript.Instance.orePrefab, new Vector3(orePos.x, Constants.ORE_FLOOR_OFFSET, orePos.y), Quaternion.identity);

            instantiatedOre.name = randomResource.ToString() + orePos.ToString();

            instantiatedOre.GetComponent<OreBehaviour>().SetResourceType(randomResource);
            instantiatedOre.GetComponent<Renderer>().material.color = Constants.ORE_COLOR_MAP[randomResource];
            instantiatedOre.GetComponent<Renderer>().material.SetFloat("_Glossiness", Constants.ORE_SMOOTHNESS_MAP[randomResource]);
            instantiatedOre.GetComponent<Renderer>().material.SetFloat("_Metallic", Constants.ORE_METALLIC_MAP[randomResource]);
            
            GameControllerScript.Instance.oreListDictionary[randomResource].Add(new ResourceTuple(instantiatedOre, false));
        }

        GameControllerScript.Instance.numberOfOres += Constants.INITIAL_ORE_NUMBER;
    }

    private static Vector2 GenerateNewOrePositionInCircle(Vector2 centerOfOres) {
        //Generates a new valid ore position within range
        var valid = false;
        Vector2 pos = new Vector2();
        while (!valid) {
            pos = Random.insideUnitCircle * Constants.ORE_GENERATION_DISTANCE_RANGE + centerOfOres;
            valid = CheckIfValidOrePosition(pos);
        }
        return pos;
    }

    private static Vector2 GenerateNewOrePositionInSquare() {
        var valid = false;
        Vector2 pos = new Vector2();
        float posX, posY;
        while (!valid) {
            posX = Random.Range(Constants.MIN_X_WORLD_POSITION, Constants.MAX_X_WORLD_POSITION);
            posY = Random.Range(Constants.MIN_Y_WORLD_POSITION, Constants.MAX_Y_WORLD_POSITION);
            pos = new Vector2(posX, posY);
            valid = CheckIfValidOrePosition(pos);
        }
        return pos;
    }

    private static bool CheckIfValidOrePosition(Vector2 pos) {
        var valid = true;
        Vector2 currentOrePos;
        foreach (ResourceEnum resource in Enum.GetValues(typeof(ResourceEnum))) {
            for (int i = 0; i < GameControllerScript.Instance.oreListDictionary[resource].Count; i++) {
                currentOrePos = new Vector2(
                    GameControllerScript.Instance.oreListDictionary[resource][i].gameObject.transform.position.x, 
                    GameControllerScript.Instance.oreListDictionary[resource][i].gameObject.transform.position.y);
                    
                if (Vector2.Distance(pos, currentOrePos) < 15) { valid = false; break; }
            }
            if(!valid) { break; }
        }
        return valid;
    }

    public static Vector3 CalculateRandomPositionAroundBase(GameObject baseGO) {
        // Choose a random angle in radians
        float angle = Random.Range(0f, Mathf.PI * 2f);

        // Get the offset at a fixed distance around the base
        Vector3 offset = new Vector3(
            Mathf.Cos(angle),
            0f,
            Mathf.Sin(angle)
        ) * Constants.BASE_RETREAT_OFFSET;

        return baseGO.transform.position + offset;
    }
    
    public static List<Vector3> CalculateWaypointsForBuilding(Vector3 buildingOrigin, int numberOfWaypoints, float radius) {
        var waypoints = new List<Vector3>();

        // Calculate waypoints in a circle around the center object
        for (int i = 0; i < numberOfWaypoints; i++)
        {
            var angle = i * Mathf.PI * 2 / numberOfWaypoints;
            var newPos = new Vector3(Mathf.Cos(angle) * Constants.WAYPOINTS_RADIUS, buildingOrigin.y, Mathf.Sin(angle) * Constants.WAYPOINTS_RADIUS);
            waypoints.Add(buildingOrigin + newPos);
        }

        return waypoints;
    }
    
    public static int FindNearestGameObjectPositionInList(GameObject agent, List<GameObject> objectivesList) {
        return FindNearestGameObjectPositionInList(agent,
            objectivesList.Select(o => o.transform.position).ToList());
    }
    
    public static int FindNearestGameObjectPositionInList(GameObject agent, List<Vector3> objectivesList) {
        //Set nearest ore for gatherer
        int nearestObjective = -1;
        var nearestDistance = float.MaxValue;
        var currentDistance = float.MinValue;
        for (int i = 0; i < objectivesList.Count; i++) {
            currentDistance = Vector3.Distance(agent.transform.position, objectivesList[i]);
            if (currentDistance < nearestDistance) {
                nearestObjective = i;
                nearestDistance = currentDistance;
            }
        }

        return nearestObjective;
    }

    public static GameObject FindNearestGameObjectInList(GameObject agent, List<ResourceTuple> objectivesList) {
        //Set nearest ore for gatherer
        int nearestObjective = -1;
        var nearestDistance = float.MaxValue;
        float currentDistance;
        for(int i = 0; i < objectivesList.Count; i++) {
            if (!objectivesList[i].isBeingGathered) {
                currentDistance = Vector3.Distance(agent.transform.position, objectivesList[i].gameObject.transform.position);
                if (currentDistance < nearestDistance) {
                    nearestObjective = i;
                    nearestDistance = currentDistance;
                }
            }
        };
        if (nearestObjective != -1) {
            objectivesList[nearestObjective].isBeingGathered = true;
            return objectivesList[nearestObjective].gameObject; 
        }

        return null;
    }

    public static void MarkObjectiveAsUnGathered(GameObject gameObject, List<ResourceTuple> objectivesList) {
        foreach (var objective in objectivesList) {
            if(ReferenceEquals(gameObject, objective.gameObject)) {
                objective.isBeingGathered = false;
                return;
            }
        }
    }

    public static void LocateMarkerOverGameObject(GameObject go, GameObject marker, float offSet, RectTransform canvas) {
        // Offset position above object bbox (in world space)
        float offsetPosY = go.transform.position.y + offSet;

        // Final position of marker above GO in world space
        Vector3 offsetPos = new Vector3(go.transform.position.x, offsetPosY, go.transform.position.z);

        // Calculate *screen* position (note, not a canvas/recttransform position)
        Vector2 canvasPos;
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(offsetPos);

        // Convert screen position to Canvas / RectTransform space <- leave camera null if Screen Space Overlay
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, screenPoint, null, out canvasPos);

        // Set
        marker.transform.localPosition = canvasPos;
    }

    public static bool DetectObjective(GameObject objective, Transform agentPosition,
            float detectionDistance, ref GameObject objectiveObject) {
        if (Vector3.Distance(objective.transform.position, agentPosition.position) <= detectionDistance) {
            //Store the objective
            objectiveObject = objective;
            return true;
        }
        return false;
    }

    public static bool DetectObjective(List<GameObject> objectiveList, Transform agentPosition,
            float detectionDistance, ref GameObject objectiveObject) {
        //If there is an enemy in range, go against him
        foreach (var objective in objectiveList) {
            if (DetectObjective(objective, agentPosition, detectionDistance, ref objectiveObject)) {
                return true;
            };
        }
        return false;
    }
    
    public static void MoveToNextWayPoint(ref int currentWaypointIndex, List<Vector3> waypointList, NavMeshAgent agent) {
        // Move to the next waypoint
        currentWaypointIndex = (currentWaypointIndex + 1) % waypointList.Count;
        agent.SetDestination(waypointList[currentWaypointIndex]);
    }
    
    public static bool CheckEnoughResources(Dictionary<ResourceEnum, int> currentResources, Dictionary<ResourceEnum, int> propCosts) {
        foreach (var propCost in propCosts) {
            if (currentResources[propCost.Key] < propCost.Value) {
                Debug.Log("Not enough of" + currentResources[propCost.Key] + " < " + propCost.Value);
                return false;
            }
        }
        return true;
    }
    
    public static void RemoveOre(ResourceEnum oreType, GameObject oreToRemove, Vector2 centerOfOres) {
        //Destroy ore
        Object.Destroy(oreToRemove);
        //Remove from list
        GameControllerScript.Instance.oreListDictionary[oreType].RemoveAll(item => item.gameObject.Equals(oreToRemove));
        //Check if we need to create more ores
        GameControllerScript.Instance.numberOfOres--;
        if (GameControllerScript.Instance.numberOfOres <= 3) {
            GenerateRandomOres(centerOfOres);
        }
    }

    public static void SetAnchorPresets(RectTransform rt, AnchorPresets preset) {
        switch (preset) {
            case AnchorPresets.TopLeft:
                rt.anchorMin = new Vector2(0, 1);
                rt.anchorMax = new Vector2(0, 1);
                rt.pivot = new Vector2(0, 1);
                break;
            case AnchorPresets.TopCenter:
                rt.anchorMin = new Vector2(0.5f, 1);
                rt.anchorMax = new Vector2(0.5f, 1);
                rt.pivot = new Vector2(0.5f, 1);
                break;
            case AnchorPresets.TopRight:
                rt.anchorMin = new Vector2(1, 1);
                rt.anchorMax = new Vector2(1, 1);
                rt.pivot = new Vector2(1, 1);
                break;
            case AnchorPresets.MiddleLeft:
                rt.anchorMin = new Vector2(0, 0.5f);
                rt.anchorMax = new Vector2(0, 0.5f);
                rt.pivot = new Vector2(0, 0.5f);
                break;
            case AnchorPresets.MiddleCenter:
                rt.anchorMin = new Vector2(0.5f, 0.5f);
                rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                break;
            case AnchorPresets.MiddleRight:
                rt.anchorMin = new Vector2(1, 0.5f);
                rt.anchorMax = new Vector2(1, 0.5f);
                rt.pivot = new Vector2(1, 0.5f);
                break;
            case AnchorPresets.BottomLeft:
                rt.anchorMin = new Vector2(0, 0);
                rt.anchorMax = new Vector2(0, 0);
                rt.pivot = new Vector2(0, 0);
                break;
            case AnchorPresets.BottomCenter:
                rt.anchorMin = new Vector2(0.5f, 0);
                rt.anchorMax = new Vector2(0.5f, 0);
                rt.pivot = new Vector2(0.5f, 0);
                break;
            case AnchorPresets.BottomRight:
                rt.anchorMin = new Vector2(1, 0);
                rt.anchorMax = new Vector2(1, 0);
                rt.pivot = new Vector2(1, 0);
                break;
            case AnchorPresets.StretchAll:
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.pivot = new Vector2(0.5f, 0.5f);
                break;
        }
    }
    
    public static string ReadFile(string fileName) {
        string filePath = Path.Combine(Application.persistentDataPath, fileName + ".json");

        if (!File.Exists(filePath)) {
            Debug.Log("File " + filePath + " not found");
            return Constants.FILE_NOT_FOUND;
        }

        return File.ReadAllText(filePath);
    }
    
    public static void WriteFile(string fileName, string fileContent) {
        string filePath = Path.Combine(Application.persistentDataPath, fileName + ".json");
        File.WriteAllText(filePath, fileContent);
        Debug.Log("Saved to: " + filePath);
    }

    public static void DeleteSaveFile(string fileName) {
        string filePath = Path.Combine(Application.persistentDataPath, fileName + ".json");
        try {File.Delete(filePath);} catch { Debug.Log("Save file not deleted"); }
    }
}