using UnityEditor;
using UnityEngine;

public class CameraMove : MonoBehaviour {

    public static CameraMove Instance;
    public Camera cameraGO, miniMapCamera;
    public GameObject cameraPivot;

    public bool isGameObjectCentered;
    public GameObject gameObjectCentered;
    public float gameObjectCenteredRefreshTime;

    public float leftCameraMovementThreshold, rightCameraMovementThreshold, rightCameraMovementRange;
    public float upCameraMovementThreshold, upCameraMovementRange, downCameraMovementThreshold;

    void Awake() { Instance = this; }

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
        
        //Clamp camera position inside circle (avoid going to far)
        var pos = cameraPivot.transform.position;
        // Ignore Y axis (clamp only in XZ plane)
        var flatPos = new Vector2(pos.x, pos.z);
        var flatPosAroundEnemyBase = new Vector2(pos.x - Constants.ENEMY_CENTER.x, pos.z - Constants.ENEMY_CENTER.y);
        
        // If inside of the main circle, or if exists the enemy circle and inside of it too, exit
        if (flatPos.magnitude <= Constants.VIEW_DISTANCE_RANGE ||
                (EnemyBaseController.Instance.mainEnemyBase != null 
                && flatPosAroundEnemyBase.magnitude < Constants.VIEW_DISTANCE_RANGE)) {
            return;
        }
        //Else, find if we need to clamp it to one or another of the circles
        if (flatPos.magnitude < flatPosAroundEnemyBase.magnitude) {
            flatPos = flatPos.normalized * Constants.VIEW_DISTANCE_RANGE;
            pos.x = flatPos.x;
            pos.z = flatPos.y;
        } else {
            var clamped = flatPosAroundEnemyBase.normalized * Constants.VIEW_DISTANCE_RANGE;
            pos.x = clamped.x + Constants.ENEMY_CENTER.x;
            pos.z = clamped.y + Constants.ENEMY_CENTER.y;
        }
        cameraPivot.transform.position = pos;
        
        //MOVE CAMERA MOVEMENT TO WASD FOR USEFULNESS
        //Move by clicking and dragging
        //if (Input.GetMouseButton(0) && (!isGameObjectCentered ||
        //                                gameObjectCenteredRefreshTime >
        //                                Constants.GAMEOBJECT_CENTERED_MAX_REFRESH_TIME)) {
        //    MoveCameraVertical(Input.GetAxis("Mouse X"));
        //    MoveCameraHorizontal(Input.GetAxis("Mouse Y"));
        //}
                
        //Move by mousePosition
        //if(Input.mousePosition.x >= rightCameraMovementThreshold && Input.mousePosition.x <= Screen.width) {
        //    MoveCameraVertical(-(Input.mousePosition.x - rightCameraMovementThreshold) / rightCameraMovementRange * 0.4f);
        //}else if (Input.mousePosition.x <= leftCameraMovementThreshold && Input.mousePosition.x >= 0) {
        //    MoveCameraVertical(1 - Input.mousePosition.x * 0.5f / leftCameraMovementThreshold);
        //}
                
        //if(Input.mousePosition.y >= upCameraMovementThreshold && Input.mousePosition.y <= Screen.height) {
        //    MoveCameraHorizontal(-(Input.mousePosition.y - upCameraMovementThreshold) / upCameraMovementRange * 0.4f);
        //}else if (Input.mousePosition.y <= downCameraMovementThreshold && Input.mousePosition.y >= 0) {
        //    MoveCameraHorizontal(1 - Input.mousePosition.y * 0.7f / downCameraMovementThreshold);
        //}
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

    public void MoveCameraVertical(float verticalMovement) {
        cameraPivot.transform.position += Vector3.left * verticalMovement * Constants.CAMERA_SMOOTHER_VALUE * 0.5f;
        cameraPivot.transform.position += Vector3.forward * verticalMovement * Constants.CAMERA_SMOOTHER_VALUE * 0.5f;
        UnFocusCameraInGO();
    }

    public void MoveCameraHorizontal(float horizontalMovement) {
        cameraPivot.transform.position -= Vector3.right * horizontalMovement * Constants.CAMERA_SMOOTHER_VALUE * 0.5f;
        cameraPivot.transform.position -= Vector3.forward * horizontalMovement * Constants.CAMERA_SMOOTHER_VALUE * 0.5f;
        UnFocusCameraInGO();
    }

    public void ZoomCamera(bool zoomIn) {
        cameraGO.orthographicSize += (zoomIn ? -1 : 1) * Constants.ZOOM_CHANGE * Time.deltaTime * Constants.CAMERA_SMOOTHER_VALUE;
        cameraGO.orthographicSize = Mathf.Clamp(cameraGO.orthographicSize, Constants.MIN_ZOOM_SIZE, Constants.MAX_ZOOM_SIZE);

        miniMapCamera.orthographicSize += (zoomIn ? -1 : 1) * Constants.ZOOM_CHANGE * Time.deltaTime * Constants.CAMERA_SMOOTHER_VALUE;
        miniMapCamera.orthographicSize = Mathf.Clamp(miniMapCamera.orthographicSize, Constants.MIN_MINIMAP_ZOOM_SIZE, Constants.MAX_MINIMAP_ZOOM_SIZE);
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
