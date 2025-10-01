using UnityEngine;

public class KeyController : MonoBehaviour {
    
    public GameObject cameraGO;
    
    void Update() {
        if (!(GameControllerScript.Instance.isInMissions || GameControllerScript.Instance.isGameFinished)) {
            //Camera controlls
            if (!GameControllerScript.Instance.isGamePaused && !GameControllerScript.Instance.isPauseMenuActive) {
                //Place camera in gameObject
                if (CameraMove.Instance.isGameObjectCentered) {
                    CameraMove.Instance.cameraPivot.transform.position =
                        new Vector3(
                            CameraMove.Instance.gameObjectCentered.transform.position.x + Constants.CAMERA_OFFSET_X,
                            Constants.CAMERA_OFFSET_Y,
                            CameraMove.Instance.gameObjectCentered.transform.position.z);
                }
                
                //Move with wasd
                if (Input.GetKey(KeyCode.W)) {
                    CameraMove.Instance.MoveCameraHorizontal(-0.3f);
                } else if (Input.GetKey(KeyCode.S)) {
                    CameraMove.Instance.MoveCameraHorizontal(0.3f);
                }

                if (Input.GetKey(KeyCode.A)) {
                    CameraMove.Instance.MoveCameraVertical(0.3f);
                } else if (Input.GetKey(KeyCode.D)) {
                    CameraMove.Instance.MoveCameraVertical(-0.3f);
                }

                //Reset camera by right clicking or by clicking F
                if (Input.GetMouseButton(1) || Input.GetKey(KeyCode.F)) {
                    CameraMove.Instance.ResetCamera();
                    CameraMove.Instance.UnFocusCameraInGO();
                }

                //Zoom camera with Q and E
                if (Input.GetKey(KeyCode.Q)) {
                    CameraMove.Instance.ZoomCamera(false);
                } else if (Input.GetKey(KeyCode.E)) {
                    CameraMove.Instance.ZoomCamera(true);
                }

                //Zoom camera with mouseScroll
                if (Input.mouseScrollDelta.y != 0) {
                    CameraMove.Instance.ZoomCamera(Input.mouseScrollDelta.y > 0);
                }
            }

            //Enter/Exit pause menu
            if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape)) {
                GameControllerScript.Instance.pauseMenuController.PauseGame();
            }

            //Ensure summary and time controls can be used
            if (!(GameControllerScript.Instance.isInAlert || GameControllerScript.Instance.isInAMenu || GameControllerScript.Instance.isPauseMenuActive || GameControllerScript.Instance.currentMissionNumber == 0)) {
                //Enter/Exit summary menu
                if (Input.GetKeyDown(KeyCode.T)) { GameControllerScript.Instance.summaryPanelController.ToggleSummaryMenu(); }

                //Adjust play/pause with numbers 1 to 3
                if (!GameControllerScript.Instance.isInSummary) {
                    if (Input.GetKeyDown(KeyCode.Alpha1)) { GameControllerScript.Instance.PauseGame(); }
                    if (Input.GetKeyDown(KeyCode.Alpha2)) { GameControllerScript.Instance.PlayNormalVelocity(); }
                    if (Input.GetKeyDown(KeyCode.Alpha3)) { GameControllerScript.Instance.PlayFastVelocity(); }
                }
            }
        }
    }
}
