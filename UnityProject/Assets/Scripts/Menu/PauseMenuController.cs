using UnityEngine;

public class PauseMenuController : MonoBehaviour {

    public GameObject pauseCanvas, settingsCanvas, tutorialCanvas, quitGameCanvas;
    private bool isTutorialFirstTime = true;
    
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
        pauseCanvas.SetActive(false);
        GameControllerScript.Instance.isInAMenu = true;
    }

    public void GoToTutorial() {
        tutorialCanvas.SetActive(true);
        pauseCanvas.SetActive(false);
        GameControllerScript.Instance.isInAMenu = true;
    }

    public void GoToQuitGame() {
        quitGameCanvas.SetActive(true);
        pauseCanvas.SetActive(false);
        GameControllerScript.Instance.isInAMenu = true;
    }

    public void ExitTutorialScreen() {
        if (isTutorialFirstTime && GameControllerScript.Instance.isTutorialActivated == 1) {
            tutorialCanvas.SetActive(false);
            isTutorialFirstTime = false;
        } else {
            ReturnToPause();
        }
    }

    public void ReturnToPause() {
        settingsCanvas.SetActive(false);
        tutorialCanvas.SetActive(false);
        quitGameCanvas.SetActive(false);
        pauseCanvas.SetActive(true);
        GameControllerScript.Instance.isInAMenu = false;
    }
}
