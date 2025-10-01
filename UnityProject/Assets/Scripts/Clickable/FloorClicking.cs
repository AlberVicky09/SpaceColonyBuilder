using UnityEngine;
using UnityEngine.EventSystems;

public class FloorClicking : MonoBehaviour {
    
    //If the floor is clicked, its because nothing else was clicked
    public void OnMouseDown() {
        if (EventSystem.current.IsPointerOverGameObject()
            || GameControllerScript.Instance.isGamePaused
            || GameControllerScript.Instance.placing) { return; }
        GameControllerScript.Instance.cameraMove.UnFocusCameraInGO();
        GameControllerScript.Instance.actionCanvas.SetActive(false);
        Clickable.selectedClickable = null;
    }
}