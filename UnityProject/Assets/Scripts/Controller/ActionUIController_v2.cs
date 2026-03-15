using UnityEngine;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class ActionUIController_v2 : MonoBehaviour {

    public RectTransform actionProgress, currentAction, currentActionAux, canvasRect;
    public Canvas actionCanvas;
    public Transform currentGameObject;
    public Image actionProgressImage, currentActionImage, currentActionAuxImage;

    public float iconWorldOffset = 2f;
    public float auxIconWorldOffset = 2f;
    
    protected float progressTime, totalProgressTime;
    
    private Vector3 screenPoint;
    private Vector2 localPoint, auxLocalPoint;

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
            if (currentActionAux != null) {
                var worldPosAux = currentGameObject.position + Vector3.up + Vector3.right * auxIconWorldOffset;
                var screenPointAux = CameraMove.Instance.cameraGO.WorldToScreenPoint(worldPosAux);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvasRect,
                    screenPointAux,
                    actionCanvas.worldCamera,
                    out auxLocalPoint);
                currentActionAux.localPosition = auxLocalPoint;
            }
        }
    }
    
    public void DisplayAction(Sprite displayImage) {
        actionProgress.gameObject.SetActive(false);
        currentAction.gameObject.SetActive(true);
        currentActionImage.sprite = displayImage;
        if (currentActionAux != null) {
            currentActionAuxImage.sprite = displayImage;
            currentActionAux.gameObject.SetActive(false);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(canvasRect);
    }

    public void DisplayProgress() {
        actionProgress.gameObject.SetActive(true);
        currentAction.gameObject.SetActive(false);
        if (currentActionAux != null) {
            currentActionAux.gameObject.SetActive(true);
        }
        progressTime = 0f;
    }
}
