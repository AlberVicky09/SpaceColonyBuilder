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
        gameControllerScript.uiInteractablePanel.gameObject.SetActive(true);
        for(int i = 0; i < Constants.ORE_RESOURCES.Count; i++) {
            gameControllerScript.uiInteractablePanelButtons[i].GetComponent<Button>().onClick.RemoveAllListeners();
            var currentResource = Constants.ORE_RESOURCES[i];
            gameControllerScript.uiInteractablePanelButtons[i].GetComponent<Button>().onClick.AddListener(() => { SelectResource(currentResource); });
        }
        gameControllerScript.uiButtonCanvas.SetActive(false);
        gameControllerScript.PauseGame();
    }

    private void SelectResource(ResourceEnum resource) {
        gathererBehaviour.resourceGatheringType = resource;
        gathererBehaviour.DisplayAction(gameControllerScript.oreListImage[resource]);
        gameControllerScript.CalculateOreForGatherer(gameObject);
        gameControllerScript.uiInteractablePanel.gameObject.SetActive(false);
        gameControllerScript.PlayVelocity(Constants.TIME_SCALE_NORMAL);
    }

    private void Retreat() {
        gathererBehaviour.ReturnToBase();
    }
}
