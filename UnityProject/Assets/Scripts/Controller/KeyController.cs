using UnityEngine;

public class KeyController : MonoBehaviour {
    
    public GameObject cameraGO;

    private float horizontalMove = 0f;
    private float verticalMove = 0f;
    
    void Update() {
        //TODO Remove
        if (Input.GetKeyDown(KeyCode.PageUp)) {
            GameControllerScript.Instance.uiUpdateController.UpdateRandomResources_TESTONLY(ResourceOperationEnum.Decrease);
        }else if (Input.GetKeyDown(KeyCode.PageDown)) {
            GameControllerScript.Instance.uiUpdateController.UpdateRandomResources_TESTONLY(ResourceOperationEnum.Increase);
        }
        
        if (!(GameControllerScript.Instance.isInMissions || GameControllerScript.Instance.isGameFinished)) {
            //Camera controlls
            if (!GameControllerScript.Instance.IsThereSomethingOnTheScreen()) {
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
                    verticalMove = 0.3f;
                    //CameraMove.Instance.MoveCameraHorizontal(-0.3f);
                } else if (Input.GetKey(KeyCode.S)) {
                    verticalMove = -0.3f;
                    //CameraMove.Instance.MoveCameraHorizontal(0.3f);
                }
                
                if (Input.GetKey(KeyCode.A)) {
                    horizontalMove = -0.3f;
                    //CameraMove.Instance.MoveCameraVertical(0.3f);
                } else if (Input.GetKey(KeyCode.D)) {
                    horizontalMove = 0.3f;
                    //CameraMove.Instance.MoveCameraVertical(-0.3f);
                }

                if (horizontalMove != 0 || verticalMove != 0) {
                    CameraMove.Instance.MoveCamera(horizontalMove, verticalMove);
                    horizontalMove = verticalMove = 0f;
                }
                
                //Zoom camera with Q and E
                if (Input.GetKey(KeyCode.Q)) {
                    CameraMove.Instance.ZoomCamera(false);
                } else if (Input.GetKey(KeyCode.E)) {
                    CameraMove.Instance.ZoomCamera(true);
                }

                //Reset camera by right clicking or by clicking F
                if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.F)) {
                    CameraMove.Instance.ResetCamera();
                    CameraMove.Instance.UnFocusCameraInGO();
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
            if (!(GameControllerScript.Instance.isInAlert || GameControllerScript.Instance.isInAMenu || GameControllerScript.Instance.isPauseMenuActive)) {
                //Enter/Exit summary menu
                if (Input.GetKeyDown(KeyCode.T) && GameControllerScript.Instance.currentMissionNumber != 0) { GameControllerScript.Instance.summaryPanelController.ToggleSummaryMenu(); }

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
