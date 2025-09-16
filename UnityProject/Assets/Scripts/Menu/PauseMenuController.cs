using UnityEngine;

public class PauseMenuController : MonoBehaviour {

    public GameObject pauseCanvas, settingsCanvas, tutorialCanvas;

    public void PauseGame() {
        if (GameControllerScript.Instance.isInAMenu) {
            ReturnToPause();
        } else {
            if (GameControllerScript.Instance.isPauseMenuActive && !GameControllerScript.Instance.wasGamePaused) {
                GameControllerScript.Instance.PlayNormalVelocity();
            } else {
                GameControllerScript.Instance.PauseGame();
            }
            GameControllerScript.Instance.TogglePauseMenu();
            pauseCanvas.SetActive(!pauseCanvas.activeSelf);
        }
    }

    public void GoToSettings() {
        settingsCanvas.SetActive(true);
        GameControllerScript.Instance.isInAMenu = true;
    }

    public void GoToTutorial() {
        tutorialCanvas.SetActive(true);
        GameControllerScript.Instance.isInAMenu = true;
    }

    public void ReturnToPause() {
        settingsCanvas.SetActive(false);
        tutorialCanvas.SetActive(false);
        GameControllerScript.Instance.isInAMenu = false;
    }
}
