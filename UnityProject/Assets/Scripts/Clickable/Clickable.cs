using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Clickable : MonoBehaviour, IDeselectHandler {

    protected static GameControllerScript gameControllerScript;
    protected static MissionController missionController;
    protected static CameraMove cameraMove;
    protected static Clickable selectedClickable;
    
    [SerializeField] Sprite objectImage;
    [SerializeField] Sprite[] buttonImages;
    [SerializeField] protected int buttonNumber;

    private float doubleClickDelay;
    private bool secondClick = false;

    private static GameObject activeButtonsObject;

    public void Start() {
        if(cameraMove == null) {cameraMove = FindObjectOfType<CameraMove>();}
        if(gameControllerScript == null) {gameControllerScript = GameObject.Find("GameController").GetComponent<GameControllerScript>();}
        if(missionController == null) {missionController = GameObject.Find("MissionPanel").GetComponent<MissionController>();}
    }
    
    public void OnClick() {
        if (!(EventSystem.current.IsPointerOverGameObject() || gameControllerScript.isGamePaused || gameControllerScript.placing)) {
            selectedClickable = this;
            gameControllerScript.actionCanvas.SetActive(true);
            gameControllerScript.uiRepresentation.sprite = objectImage;
            if (!ReferenceEquals(gameObject, activeButtonsObject)) {
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
                EventSystem.current.SetSelectedGameObject(gameObject);
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
