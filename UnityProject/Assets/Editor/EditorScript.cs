using UnityEditor;
using UnityEngine;

[CustomEditor (typeof(GameControllerScript))]
public class EditorScript : Editor {
    
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        if (GUILayout.Button("Reset camera move")) {
            GameControllerScript.Instance.cameraMove.ResetCamera();
        }

        if (GUILayout.Button("Move to enemyBase")) {
            GameControllerScript.Instance.cameraMove.StartTravellingToEnemyBase();
        }

        if (GUILayout.Button("Increase random resources")) {
            GameControllerScript.Instance.uiUpdateController.UpdateRandomResources_TESTONLY(ResourceOperationEnum.Increase);
        }
    
        if (GUILayout.Button("Decrease random resources")) {
            GameControllerScript.Instance.uiUpdateController.UpdateRandomResources_TESTONLY(ResourceOperationEnum.Decrease);
        }
    }
}
