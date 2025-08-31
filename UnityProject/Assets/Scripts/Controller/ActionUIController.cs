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

    public void Update() {
        if (currentAction.activeSelf) {
            Utils.LocateMarkerOverGameObject(gameObject, currentAction, 5f, canvas);
        } else {
            actionProgressImage.fillAmount = progressTime / totalProgressTime;
            progressTime += Time.deltaTime;
            Utils.LocateMarkerOverGameObject(gameObject, actionProgress, 5f, canvas);
        }
    }
    
    public void DisplayAction(Sprite displayImage) {
        actionProgress.gameObject.SetActive(false);
        currentAction.gameObject.SetActive(true);
        currentActionImage.sprite = displayImage;
    }

    public void DisplayProgress() {
        actionProgress.gameObject.SetActive(true);
        currentAction.gameObject.SetActive(false);
        progressTime = 0f;
    }
}
