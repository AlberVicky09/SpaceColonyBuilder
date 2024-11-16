using UnityEditor;
using UnityEngine;

public class CameraMove : MonoBehaviour {

    public Camera cameraGO;
    [SerializeField] GameObject cameraPivot;

    private bool isGameObjectCentered;
    private GameObject gameObjectCentered;
    private float gameObjectCenteredRefreshTime;

    private float leftCameraMovementThreshold, rightCameraMovementThreshold, rightCameraMovementRange;
    private float upCameraMovementThreshold, upCameraMovementRange, downCameraMovementThreshold;

    void Start() {
        leftCameraMovementThreshold = Screen.width * 0.15f;
        rightCameraMovementThreshold = Screen.width * 0.85f;
        rightCameraMovementRange = (Screen.width - rightCameraMovementThreshold);
        downCameraMovementThreshold = Screen.height * 0.15f;
        upCameraMovementThreshold = Screen.height * 0.85f;
        upCameraMovementRange = (Screen.height - upCameraMovementThreshold);
    }
    
    void LateUpdate(){

        if (isGameObjectCentered && gameObjectCenteredRefreshTime <= Constants.GAMEOBJECT_CENTERED_MAX_REFRESH_TIME) {
            gameObjectCenteredRefreshTime += Time.deltaTime;
        }

        if (!GameControllerScript.Instance.isGamePaused && !GameControllerScript.Instance.isPauseMenuActive) {
            //Place camera in gameObject
            if (isGameObjectCentered) {
                cameraPivot.transform.position = new Vector3(gameObjectCentered.transform.position.x + Constants.CAMERA_OFFSET_X, Constants.CAMERA_OFFSET_Y, gameObjectCentered.transform.position.z);
            } else {
                //Move with wasd
                if (Input.GetKey(KeyCode.W)) {
                    MoveCameraHorizontal(-0.3f);
                } else if (Input.GetKey(KeyCode.S)) {
                    MoveCameraHorizontal(0.3f);
                }

                if (Input.GetKey(KeyCode.A)) {
                    MoveCameraVertical(0.3f);
                } else if (Input.GetKey(KeyCode.D)) {
                    MoveCameraVertical(-0.3f);
                }
                
                //Move by clicking and dragging
                //if (Input.GetMouseButton(0) && (!isGameObjectCentered ||
                //                                gameObjectCenteredRefreshTime >
                //                                Constants.GAMEOBJECT_CENTERED_MAX_REFRESH_TIME)) {
                //    MoveCameraVertical(Input.GetAxis("Mouse X"));
                //    MoveCameraHorizontal(Input.GetAxis("Mouse Y"));
                //}
                
                //Move by mousePosition
                if(Input.mousePosition.x >= rightCameraMovementThreshold && Input.mousePosition.x <= Screen.width) {
                    MoveCameraVertical(-(Input.mousePosition.x - rightCameraMovementThreshold) / rightCameraMovementRange * 0.4f);
                }else if (Input.mousePosition.x <= leftCameraMovementThreshold && Input.mousePosition.x >= 0) {
                    MoveCameraVertical(1 - Input.mousePosition.x * 0.5f / leftCameraMovementThreshold);
                }
                
                if(Input.mousePosition.y >= upCameraMovementThreshold && Input.mousePosition.y <= Screen.height) {
                    MoveCameraHorizontal(-(Input.mousePosition.y - upCameraMovementThreshold) / upCameraMovementRange * 0.4f);
                }else if (Input.mousePosition.y <= downCameraMovementThreshold && Input.mousePosition.y >= 0) {
                    MoveCameraHorizontal(1 - Input.mousePosition.y * 0.7f / downCameraMovementThreshold);
                }

                //Reset camera by right clicking or by clicking F
                if (Input.GetMouseButton(1) || Input.GetKey(KeyCode.F)) {
                    ResetCamera();
                    UnFocusCameraInGO();
                }

                //Zoom camera with Q and E
                if (Input.GetKey(KeyCode.Q)) {
                    ZoomCamera(false);
                } else if (Input.GetKey(KeyCode.E)) {
                    ZoomCamera(true);
                }

                //Zoom camera with mouseScroll
                if (Input.mouseScrollDelta.y != 0) {
                    ZoomCamera(Input.mouseScrollDelta.y > 0);
                }
            }
        }        
    }

    public void ResetCamera() {
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

    private void MoveCameraVertical(float verticalMovement) {
        cameraPivot.transform.position += Vector3.left * verticalMovement * Constants.CAMERA_SMOOTHER_VALUE * 0.5f;
        cameraPivot.transform.position += Vector3.forward * verticalMovement * Constants.CAMERA_SMOOTHER_VALUE * 0.5f;
        UnFocusCameraInGO();
    }

    private void MoveCameraHorizontal(float horizontalMovement) {
        cameraPivot.transform.position -= Vector3.right * horizontalMovement * Constants.CAMERA_SMOOTHER_VALUE * 0.5f;
        cameraPivot.transform.position -= Vector3.forward * horizontalMovement * Constants.CAMERA_SMOOTHER_VALUE * 0.5f;
        UnFocusCameraInGO();
    }

    private void ZoomCamera(bool zoomIn) {
        cameraGO.orthographicSize += (zoomIn ? 1 : -1) * Constants.ZOOM_CHANGE * Time.deltaTime * Constants.CAMERA_SMOOTHER_VALUE;
        cameraGO.orthographicSize = Mathf.Clamp(cameraGO.orthographicSize, Constants.MIN_ZOOM_SIZE, Constants.MAX_ZOOM_SIZE);
    }
}


[CustomEditor (typeof(CameraMove))]
public class CameraMoveEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        CameraMove cameraMove = (CameraMove)target;
        if (GUILayout.Button("Reset move")) {
            cameraMove.ResetCamera();
        }
    }
}
