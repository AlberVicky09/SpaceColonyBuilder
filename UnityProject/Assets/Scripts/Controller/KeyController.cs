using UnityEngine;

public class KeyController : MonoBehaviour {

    public PauseMenuController pauseMenuController;
    public MeshCollider worldLimitMeshCollider;
    public GameObject cameraGO;
    
    void Update() {
        //Camera controlls
        if (!GameControllerScript.Instance.isGamePaused && !GameControllerScript.Instance.isPauseMenuActive) {
            //Place camera in gameObject
            if (CameraMove.Instance.isGameObjectCentered) {
                CameraMove.Instance.cameraPivot.transform.position = 
                    new Vector3(CameraMove.Instance.gameObjectCentered.transform.position.x + Constants.CAMERA_OFFSET_X,
                                Constants.CAMERA_OFFSET_Y,
                                CameraMove.Instance.gameObjectCentered.transform.position.z);
            } else {
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
                    CameraMove.Instance.ZoomCamera(Input.mouseScrollDelta.y < 0);
                }
            }
        }
        
        //Enter/Exit pause menu
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape)) { pauseMenuController.PauseGame(); }
    }
}
