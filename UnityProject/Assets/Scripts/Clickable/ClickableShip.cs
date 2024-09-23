using TMPro;
using UnityEngine.UI;

public class ClickableShip : Clickable {

    private GathererBehaviour gathererBehaviour;

    public override void UpdateTexts() {
        if (selectedClickable == this) {
            GameControllerScript.Instance.actionText.text = "Gatherer \n" + gathererBehaviour.gathererLoad + "/" +
                                                   gathererBehaviour.maxGathererLoad;
        }
    }
    
    protected override void StartButtons() {
        base.StartButtons();
        if (gathererBehaviour == null) {
            gathererBehaviour = GetComponent<GathererBehaviour>();
        }
        GameControllerScript.Instance.actionButtons[0].GetComponent<Button>().onClick.AddListener(DisplayScreen);
        GameControllerScript.Instance.actionButtons[1].GetComponent<Button>().onClick.AddListener(Retreat);
    }

    private void DisplayScreen() {
        //Activate canvas and place buttons
        GameControllerScript.Instance.interactableButtonManager.PlaceButtonsInCircle(Constants.ORE_RESOURCES.Count);
        
        //Setup buttons behaviour
        for(int i = 0; i < Constants.ORE_RESOURCES.Count; i++) {
            GameControllerScript.Instance.interactableButtonManager.interactableButtonList[i].GetComponent<Button>().onClick.RemoveAllListeners();
            var currentResource = Constants.ORE_RESOURCES[i];
            GameControllerScript.Instance.interactableButtonManager.interactableButtonList[i].GetComponent<Button>().onClick.AddListener(() => { SelectResource(currentResource); });
            GameControllerScript.Instance.interactableButtonManager.interactableButtonList[i].GetComponentInChildren<TMP_Text>().text = currentResource.ToString();
        }
        
        GameControllerScript.Instance.actionCanvas.SetActive(false);
        GameControllerScript.Instance.PauseGame();
    }

    private void SelectResource(ResourceEnum resource) {
        gathererBehaviour.resourceGatheringType = resource;
        gathererBehaviour.DisplayAction(GameControllerScript.Instance.oreListImage[resource]);
        GameControllerScript.Instance.CalculateOreForGatherer(gameObject);
        GameControllerScript.Instance.interactableButtonManager.gameObject.SetActive(false);
        GameControllerScript.Instance.PlayVelocity(Constants.TIME_SCALE_NORMAL);
    }

    private void Retreat() {
        gathererBehaviour.ReturnToBase(false);
    }
}
