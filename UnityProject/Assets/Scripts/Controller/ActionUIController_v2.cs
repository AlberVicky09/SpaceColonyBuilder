using UnityEngine;
using UnityEngine.UI;

public class ActionUIController_v2 : MonoBehaviour {

    public RectTransform actionProgress, currentAction, canvasRect;
    public Canvas actionCanvas;
    public Transform currentGameObject;
    public Image actionProgressImage, currentActionImage;

    public float iconWorldOffset = 2f;
    
    protected float progressTime, totalProgressTime;
    private float orthoRatio, heightOffset, widthOffset;
    
    private Vector3 screenPoint;
    private Vector2 localPoint;

    private void LateUpdate() {
        //Calculate zoom ratio (0 when farthest, 1 when closest)
        var worldPos = currentGameObject.position + Vector3.up * iconWorldOffset;
        
        //Projection in screen
        var screenPoint = CameraMove.Instance.cameraGO.WorldToScreenPoint(worldPos);
        
        //Remove if behind camera
        if (screenPoint.z < 0f) {
            actionCanvas.enabled = false;
            return;
        }
        actionCanvas.enabled = true;
        
        //Screen to object canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPoint,
            actionCanvas.worldCamera,
            out localPoint);
        
        //Locate active icon
        if (currentAction.gameObject.activeSelf) {
            currentAction.localPosition = localPoint;
        } else {
            actionProgressImage.fillAmount = progressTime / totalProgressTime;
            progressTime += Time.deltaTime;

            actionProgress.localPosition = localPoint;
        }
    }
    
    public void DisplayAction(Sprite displayImage) {
        actionProgress.gameObject.SetActive(false);
        currentAction.gameObject.SetActive(true);
        currentActionImage.sprite = displayImage;
        LayoutRebuilder.ForceRebuildLayoutImmediate(canvasRect);
    }

    public void DisplayProgress() {
        actionProgress.gameObject.SetActive(true);
        currentAction.gameObject.SetActive(false);
        progressTime = 0f;
    }
}
