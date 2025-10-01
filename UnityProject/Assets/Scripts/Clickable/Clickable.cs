using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Clickable : MonoBehaviour {
    
    public static Clickable selectedClickable;
    
    [SerializeField] protected Sprite objectImage;
    [SerializeField] protected Sprite[] buttonImages;
    [SerializeField] protected int buttonNumber;
    [SerializeField] protected SfxSource sfxSource;

    private float doubleClickDelay;
    private bool secondClick = false;

    public void OnMouseDown() {
        if (GameControllerScript.Instance.IsThereSomethingOnTheScreen()) { return; }
        
        if (selectedClickable == this) {
            CheckDoubleClick();
        } else {
            selectedClickable = this;
            GameControllerScript.Instance.actionCanvas.SetActive(true);
            DisplayRepresentation();
            StartButtons();
            UpdateTexts();
            DisplayButtons();
            sfxSource.PlaySfx();
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

    protected virtual void DisplayRepresentation() {
        //Reset color just in case it has been modified
        GameControllerScript.Instance.uiRepresentation.color = Color.white;
        GameControllerScript.Instance.uiRepresentation.sprite = objectImage;
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
}
