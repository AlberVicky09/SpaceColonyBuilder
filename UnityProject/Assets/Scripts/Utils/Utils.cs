
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public static class Utils {

    public static Transform FindNearestInList(GameObject agent, List<GameObject> objectivesList) {
        //Set nearest ore for gatherer
        Transform nearestObjective = null;
        var nearestDistance = float.MaxValue;
        var currentDistance = float.MinValue;
        objectivesList.ForEach(objective => {
            currentDistance = Vector3.Distance(agent.transform.position, objective.transform.position);
            if (currentDistance < nearestDistance) {
                nearestObjective = objective.transform;
                nearestDistance = currentDistance;
            }
        });
        return nearestObjective;
    }

    public static void LocateMarkerOverGameObject(GameObject go, RectTransform canvas, Transform markerPos) {
        // Offset position above object bbox (in world space)
        float offsetPosY = go.transform.position.y + 1.5f;

        // Final position of marker above GO in world space
        Vector3 offsetPos = new Vector3(go.transform.position.x, offsetPosY, go.transform.position.z);

        // Calculate *screen* position (note, not a canvas/recttransform position)
        Vector2 canvasPos;
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(offsetPos);

        // Convert screen position to Canvas / RectTransform space <- leave camera null if Screen Space Overlay
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, screenPoint, null, out canvasPos);

        // Set
        markerPos.localPosition = canvasPos;
    }
}
