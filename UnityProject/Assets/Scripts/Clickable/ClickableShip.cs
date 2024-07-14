using UnityEngine.UI;

public class ClickableShip : Clickable {

    private GathererBehaviour gathererBehaviour;

    public override void UpdateTexts() {
        gameControllerScript.actionText.text = "Gatherer \n" + gathererBehaviour.gathererLoad + "/" + gathererBehaviour.maxGathererLoad;
    }
    
    protected override void StartButtons() {
        base.StartButtons();
        if (gathererBehaviour == null) {
            gathererBehaviour = GetComponent<GathererBehaviour>();
        }
        gameControllerScript.actionButtons[0].GetComponent<Button>().onClick.AddListener(DisplayScreen);
        gameControllerScript.actionButtons[1].GetComponent<Button>().onClick.AddListener(Retreat);
    }

    private void DisplayScreen() {
        gameControllerScript.uiInteractablePanel.gameObject.SetActive(true);
        for(int i = 0; i < Constants.ORE_RESOURCES.Count; i++) {
            gameControllerScript.uiInteractablePanelButtons[i].GetComponent<Button>().onClick.RemoveAllListeners();
            var currentResource = Constants.ORE_RESOURCES[i];
            gameControllerScript.uiInteractablePanelButtons[i].GetComponent<Button>().onClick.AddListener(() => { SelectResource(currentResource); });
        }
        gameControllerScript.actionCanvas.SetActive(false);
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
        gathererBehaviour.ReturnToBase(false);
    }
}
