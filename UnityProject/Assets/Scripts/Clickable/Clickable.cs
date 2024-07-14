using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Clickable : MonoBehaviour, IDeselectHandler {

    protected GameControllerScript gameControllerScript;
    protected MissionController missionController;
    [SerializeField] Sprite objectImage;
    [SerializeField] Sprite[] buttonImages;
    [SerializeField] protected int buttonNumber;

    private CameraMove cameraMove;

    private float doubleClickDelay;
    private bool secondClick = false;

    private static GameObject activeButtonsObject;

    public void Start() {
        cameraMove = FindObjectOfType<CameraMove>();
        gameControllerScript = GameObject.Find("GameController").GetComponent<GameControllerScript>();
        missionController = GameObject.Find("MissionPanel").GetComponent<MissionController>();
    }
    
    public void OnClick() {
        if (!(EventSystem.current.IsPointerOverGameObject() || gameControllerScript.isGamePaused)) {
            gameControllerScript.actionCanvas.SetActive(true);
            gameControllerScript.uiRepresentation.sprite = objectImage;
            if (!ReferenceEquals(this.gameObject, activeButtonsObject)) {
                StartButtons();
            }
            UpdateTexts();
            DisplayButtons();
            CheckDoubleClick();
        }
    }
    
    public abstract void UpdateTexts();
    
    private void DisplayButtons() {
        //Disable all buttons
        foreach (var button in gameControllerScript.actionButtons) {
            button.SetActive(false);
        }
        var i = 0;
        while(i < buttonNumber && gameControllerScript.actionButtons.Length >= i) {
            gameControllerScript.actionButtons[i].SetActive(true);
            gameControllerScript.actionButtons[i].GetComponent<Image>().sprite = buttonImages[i];
            i++;
        }
    }

    private void CheckDoubleClick() {
        if (secondClick) {
            if (Time.time - doubleClickDelay < Constants.MAX_DOUBLE_CLICK_DELAY) {
                cameraMove.FocusCameraInGO(this.gameObject);
                secondClick = false;
                EventSystem.current.SetSelectedGameObject(this.gameObject);
            } else {
                doubleClickDelay = Time.time;
            }
        } else {
            secondClick = true;
            doubleClickDelay = Time.time;
        }
    }

    protected virtual void StartButtons() {
        foreach (var button in gameControllerScript.actionButtons) {
            button.GetComponent<Button>().onClick.RemoveAllListeners();
        }
        activeButtonsObject = this.gameObject;
    }

    public void OnDeselect(BaseEventData eventData) {
        cameraMove.UnFocusCameraInGO();
    }
}
