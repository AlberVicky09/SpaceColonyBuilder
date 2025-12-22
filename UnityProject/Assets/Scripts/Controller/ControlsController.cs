using TMPro;
using UnityEngine;

public class ControlsController : MonoBehaviour {

    public TMP_Text controlsText;
    public GameObject prevTutorialButton, nextTutorialButton;
    private int controlsIndex;
    
    public void GoToNextPage() {
        controlsIndex++;
        UpdateControls();
    }

    public void GoToPrevPage() {
        controlsIndex--;
        UpdateControls();
    }

    public void RestartControls() {
        controlsIndex = 0;
        UpdateControls();
    }

    private void UpdateControls() {
        prevTutorialButton.SetActive(controlsIndex != 0);
        
        controlsText.text = Constants.CONTROLS_TEXTS[controlsIndex];
        
        nextTutorialButton.SetActive(controlsIndex != Constants.CONTROLS_TEXTS.Count - 1);
    }
}
