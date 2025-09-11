using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Clickable : MonoBehaviour, IDeselectHandler {
    
    protected static Clickable selectedClickable;
    
    [SerializeField] Sprite objectImage;
    [SerializeField] protected Sprite[] buttonImages;
    [SerializeField] protected int buttonNumber;

    private float doubleClickDelay;
    private bool secondClick = false;
    
    public void OnMouseDown() {
        if (!(EventSystem.current.IsPointerOverGameObject() || GameControllerScript.Instance.isGamePaused || GameControllerScript.Instance.placing)) {
            selectedClickable = this;
            GameControllerScript.Instance.actionCanvas.SetActive(true);
            GameControllerScript.Instance.uiRepresentation.sprite = objectImage;
            StartButtons();
            UpdateTexts();
            DisplayButtons();
            CheckDoubleClick();
        }
    }
    
    public abstract void UpdateTexts();
    
    protected virtual void DisplayButtons() {
        //Disable all buttons
        foreach (var button in GameControllerScript.Instance.actionButtons) {
            button.SetActive(false);
        }
        var i = 0;
        while(i < buttonNumber && GameControllerScript.Instance.actionButtons.Length >= i) {
            GameControllerScript.Instance.actionButtons[i].SetActive(true);
            GameControllerScript.Instance.actionButtons[i].GetComponent<Image>().sprite = buttonImages[i];
            i++;
        }
    }

    protected void CheckDoubleClick() {
        if (secondClick) {
            if (Time.time - doubleClickDelay < Constants.MAX_DOUBLE_CLICK_DELAY) {
                GameControllerScript.Instance.cameraMove.FocusCameraInGO(gameObject);
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
        foreach (var button in GameControllerScript.Instance.actionButtons) {
            var buttonComponent = button.GetComponent<Button>();
            buttonComponent.onClick.RemoveAllListeners();
            buttonComponent.interactable = true;
        }
    }

    public void OnDeselect(BaseEventData eventData) {
        GameControllerScript.Instance.cameraMove.UnFocusCameraInGO();
    }
}
