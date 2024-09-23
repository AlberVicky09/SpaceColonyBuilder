using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class Utils {

    public static GameObject FindNearestGameObjectInTupleList(GameObject agent, List<GameObject> objectivesList) {
        //Set nearest ore for gatherer
        GameObject nearestObjective = null;
        var nearestDistance = float.MaxValue;
        var currentDistance = float.MinValue;
        objectivesList.ForEach(objective => {
            currentDistance = Vector3.Distance(agent.transform.position, objective.transform.position);
            if (currentDistance < nearestDistance) {
                nearestObjective = objective;
                nearestDistance = currentDistance;
            }
        });
        return nearestObjective;
    }

    public static GameObject FindNearestGameObjectInTupleList(GameObject agent, List<ResourceTuple> objectivesList) {
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
        } else {
            return null;
        }
        
    }

    public static void MarkObjectiveAsUnGathered(GameObject gameObject, List<ResourceTuple> objectivesList) {
        foreach (var objective in objectivesList) {
            if(ReferenceEquals(gameObject, objective.gameObject)) {
                objective.isBeingGathered = false;
                return;
            }
        }
    }

    public static void LocateMarkerOverGameObject(GameObject go, GameObject marker, RectTransform canvas) {
        // Offset position above object bbox (in world space)
        float offsetPosY = go.transform.position.y + 3f;

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
    
    public static bool DetectObjective(List<GameObject> objectiveList, Transform agentPosition,
        float detectionDistance, ref FighterStatesEnum agentState, ref GameObject objectiveObject) {
        //If there is an enemy in range, go against him
        foreach (var objective in objectiveList) {
            if (Vector3.Distance(objective.transform.position, agentPosition.position) <= detectionDistance) {
                //Set state as Chasing and store the objective
                agentState = FighterStatesEnum.Chasing;
                objectiveObject = objective;
                return true;
            }
        }

        return false;
    }
    
    public static string[] ReadFile(string fileName)
    {
        // Path to the file
        string filePath = Path.Combine(Application.dataPath, "Resources", fileName + ".txt");

        // Check if the file exists
        return File.Exists(filePath) ?
            // Read all text from the file
            File.ReadAllLines(filePath) : new []{"File not found", "File not found"};
    }
}
