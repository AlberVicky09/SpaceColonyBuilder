using UnityEngine;
using UnityEngine.UI;

public class ClickableShip : Clickable {

    private GathererBehaviour gathererBehaviour;

    public override void StartButtons() {
        base.StartButtons();
        gathererBehaviour = GetComponent<GathererBehaviour>();
        gameControllerScript.uiButtons[0].GetComponent<Button>().onClick.AddListener(DisplayScreen);
        gameControllerScript.uiButtons[1].GetComponent<Button>().onClick.AddListener(Retreat);
    }

    private void DisplayScreen() {
        ResourceEnum currentResource;
        gameControllerScript.uiInteractablePanel.gameObject.SetActive(true);
        for(int i = 0; i < Constants.ORE_RESOURCES.Count; i++) {
            gameControllerScript.uiInteractablePanelButtons[i].GetComponent<Button>().onClick.RemoveAllListeners();
            currentResource = Constants.ORE_RESOURCES[i];
            gameControllerScript.uiInteractablePanelButtons[i].GetComponent<Button>().onClick.AddListener(delegate { SelectResource(currentResource); });
        }
        gameControllerScript.uiButtonCanvas.SetActive(false);
        gameControllerScript.PauseGame();
    }

    private void SelectResource(ResourceEnum resource) {
        gathererBehaviour.resourceGatheringType = resource;
        gameControllerScript.CalculateOreForGatherer(this.gameObject);
        gameControllerScript.uiInteractablePanel.gameObject.SetActive(false);
        gameControllerScript.PlayVelocity(Constants.NORMAL_VELOCITY);
        gathererBehaviour.UpdateDestination();
    }

    private void Retreat() {
        gathererBehaviour.ReturnToBase();
    }
}
