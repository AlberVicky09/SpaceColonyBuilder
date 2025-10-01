using UnityEngine;

public class FloorClicking : MonoBehaviour {
    
    //If the floor is clicked, its because nothing else was clicked
    public void OnMouseDown() {
        if (GameControllerScript.Instance.IsThereSomethingOnTheScreen()) { return; }
        GameControllerScript.Instance.cameraMove.UnFocusCameraInGO();
        GameControllerScript.Instance.actionCanvas.SetActive(false);
        Clickable.selectedClickable = null;
    }
}