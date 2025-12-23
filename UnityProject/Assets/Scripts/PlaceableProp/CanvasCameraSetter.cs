using UnityEngine;

public class CanvasCameraSetter : MonoBehaviour {
    public Canvas propCanvas;

    void Start() {
        propCanvas.worldCamera = GameControllerScript.Instance.cameraMove.cameraGO;
        propCanvas.planeDistance = 1f;
        propCanvas.sortingOrder = 1;
    }
}
