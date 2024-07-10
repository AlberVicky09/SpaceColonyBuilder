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

    public static GameObject activeButtonsObject = null;

    private void Start() {
        cameraMove = FindObjectOfType<CameraMove>();
        gameControllerScript = GameObject.Find("GameController").GetComponent<GameControllerScript>();
        missionController = GameObject.Find("MissionPanel").GetComponent<MissionController>();
    }
    public void OnClick() {
        if (!(EventSystem.current.IsPointerOverGameObject() || gameControllerScript.isGamePaused)) {
            gameControllerScript.uiButtonCanvas.SetActive(true);
            gameControllerScript.uiRepresentation.sprite = objectImage;
            if (!ReferenceEquals(this.gameObject, activeButtonsObject)) {
                StartButtons();
            }
            DisplayButtons();
            CheckDoubleClick();
        }
    }
    public void DisplayButtons() {
        //Disable all buttons
        foreach (var button in gameControllerScript.uiButtons) {
            button.SetActive(false);
        }
        var i = 0;
        while(i < buttonNumber && gameControllerScript.uiButtons.Length >= i) {
            gameControllerScript.uiButtons[i].SetActive(true);
            gameControllerScript.uiButtons[i].GetComponent<Image>().sprite = buttonImages[i];
            i++;
        }
    }

    public void CheckDoubleClick() {
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

    public virtual void StartButtons() {
        foreach (var button in gameControllerScript.uiButtons) {
            button.GetComponent<Button>().onClick.RemoveAllListeners();
        }
        activeButtonsObject = this.gameObject;
    }

    public void OnDeselect(BaseEventData eventData) {
        cameraMove.UnFocusCameraInGO();
    }
}
