using UnityEngine;

public class PauseMenuController : MonoBehaviour {

    public GameObject pauseCanvas;

    public void PauseGame() {
        if (GameControllerScript.Instance.isPauseMenuActive) {
            GameControllerScript.Instance.PlayVelocity(1f);
        } else {
            GameControllerScript.Instance.PauseGame();
        }
        GameControllerScript.Instance.TogglePauseMenu();
        pauseCanvas.SetActive(!pauseCanvas.activeSelf);
    }
}
