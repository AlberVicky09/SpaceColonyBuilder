using UnityEngine;
using UnityEngine.UI;

public class ActionUIController : MonoBehaviour {
    public RectTransform canvas;
    public GameObject actionProgress;
    public Image actionProgressImage;
    public GameObject currentAction;
    public Image currentActionImage;
    protected float progressTime;
    protected float totalProgressTime;
    public float scalingFactor = 0.017f;
    public Vector3 upVector = new(0f, 2f, 0f);
    private float canvasDistance, scaleFactor, orthoRatio;
    private Vector3 worldPosition, worldOffsetedPosition, offsettedPosition;

    // public void Update() {
    //     //Only update position if its inside camera
    //     var positionInCamera = CameraMove.Instance.cameraGO.WorldToViewportPoint(transform.position);
    //     if (positionInCamera.z > -0.3f && positionInCamera.x > -0.3f && positionInCamera.x < 1.03f && positionInCamera.y > -0.3f && positionInCamera.y < 1.03f) {
    //         canvas.gameObject.SetActive(true);
    //         if (currentAction.activeSelf) {
    //             Utils.LocateMarkerOverGameObject(gameObject, currentAction, 5f, canvas);
    //         } else {
    //             actionProgressImage.fillAmount = progressTime / totalProgressTime;
    //             progressTime += Time.deltaTime;
    //             Utils.LocateMarkerOverGameObject(gameObject, actionProgress, 5f, canvas);
    //         }
    //     } else {
    //         canvas.gameObject.SetActive(false);
    //     }
    // }
    
    void LateUpdate() {
        orthoRatio = CameraMove.Instance.cameraGO.orthographicSize / Constants.MIN_ZOOM_SIZE;
        worldPosition = transform.position + upVector * orthoRatio; 
        worldOffsetedPosition = CameraMove.Instance.cameraGO.WorldToScreenPoint(worldPosition);
        //offsettedPosition = transform.position + upVector * orthoRatio;
        
        if (currentAction.activeSelf) {
            currentAction.transform.position = offsettedPosition;
            currentAction.transform.forward = CameraMove.Instance.cameraGO.transform.forward; // face the camera
            currentAction.transform.localScale = Vector3.one * (orthoRatio * scalingFactor);
        } else {
            actionProgressImage.fillAmount = progressTime / totalProgressTime;
            progressTime += Time.deltaTime;
            
            actionProgress.transform.position = offsettedPosition;
            actionProgress.transform.forward = CameraMove.Instance.cameraGO.transform.forward;
            actionProgress.transform.localScale = Vector3.one * (orthoRatio * scalingFactor);
        }
    }
    
    public void DisplayAction(Sprite displayImage) {
        actionProgress.gameObject.SetActive(false);
        currentAction.gameObject.SetActive(true);
        currentActionImage.sprite = displayImage;
        LayoutRebuilder.ForceRebuildLayoutImmediate(canvas);
    }

    public void DisplayProgress() {
        actionProgress.gameObject.SetActive(true);
        currentAction.gameObject.SetActive(false);
        progressTime = 0f;
    }
}
