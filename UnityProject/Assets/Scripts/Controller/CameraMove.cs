using System;
using UnityEditor;
using UnityEngine;

public class CameraMove : MonoBehaviour {

    [SerializeField] Camera cameraGO;
    [SerializeField] GameObject cameraPivot;
    [SerializeField] GameControllerScript gameControllerScript;

    private bool isGameObjectCentered = false;
    private GameObject gameObjectCentered;
    private float gameObjectCenteredRefreshTime = 0f;

    void LateUpdate(){

        if (isGameObjectCentered && gameObjectCenteredRefreshTime <= Constants.GAMEOBJECT_CENTERED_MAX_REFRESH_TIME) {
            gameObjectCenteredRefreshTime += Time.deltaTime;
        }

        if (!gameControllerScript.isGamePaused) {
            if (Input.GetMouseButton(0) && (!isGameObjectCentered || gameObjectCenteredRefreshTime > Constants.GAMEOBJECT_CENTERED_MAX_REFRESH_TIME)) {
                cameraPivot.transform.position += Vector3.left * Input.GetAxis("Mouse X") * Constants.CAMERA_SMOOTHER_VALUE * 0.5f;
                cameraPivot.transform.position += Vector3.forward * Input.GetAxis("Mouse X") * Constants.CAMERA_SMOOTHER_VALUE * 0.5f;

                cameraPivot.transform.position -= Vector3.right * Input.GetAxis("Mouse Y") * Constants.CAMERA_SMOOTHER_VALUE * 0.5f;
                cameraPivot.transform.position -= Vector3.forward * Input.GetAxis("Mouse Y") * Constants.CAMERA_SMOOTHER_VALUE * 0.5f;

                UnFocusCameraInGO();
            }

            if (Input.GetMouseButton(1)) {
                resetCamera();
                UnFocusCameraInGO();
            }

            if (Input.mouseScrollDelta.y != 0) {
                if (Input.mouseScrollDelta.y > 0) {
                    cameraGO.orthographicSize -= Constants.ZOOM_CHANGE * Time.deltaTime * Constants.CAMERA_SMOOTHER_VALUE;
                } else if (Input.mouseScrollDelta.y < 0) {
                    cameraGO.orthographicSize += Constants.ZOOM_CHANGE * Time.deltaTime * Constants.CAMERA_SMOOTHER_VALUE;
                }
                cameraGO.orthographicSize = Mathf.Clamp(cameraGO.orthographicSize, Constants.MIN_ZOOM_SIZE, Constants.MAX_ZOOM_SIZE);
            }


            if (isGameObjectCentered) {
                cameraPivot.transform.position = new Vector3(gameObjectCentered.transform.position.x + Constants.CAMERA_OFFSET_X, Constants.CAMERA_OFFSET_Y, gameObjectCentered.transform.position.z);
            }
        }        
    }

    public void resetCamera() {
        cameraPivot.transform.position = Constants.RESET_CAMERA_POSITION;
    }

    public void FocusCameraInGO(GameObject go) {
        isGameObjectCentered = true;
        gameObjectCentered = go;
    }

    public void UnFocusCameraInGO() {
        gameObjectCenteredRefreshTime = 0f;
        isGameObjectCentered = false;
    }
}


[CustomEditor (typeof(CameraMove))]
public class CameraMoveEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        CameraMove cameraMove = (CameraMove)target;
        if (GUILayout.Button("Reset move")) {
            cameraMove.resetCamera();
        }
    }
}
