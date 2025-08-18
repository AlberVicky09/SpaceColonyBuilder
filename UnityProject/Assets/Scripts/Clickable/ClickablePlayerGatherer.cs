using TMPro;
using UnityEngine.UI;

public class ClickablePlayerGatherer : ClickableGatherer {
    
    protected override void StartButtons() {
        base.StartButtons();
        GameControllerScript.Instance.actionButtons[0].GetComponent<Button>().onClick.AddListener(DisplayScreen);
        GameControllerScript.Instance.actionButtons[1].GetComponent<Button>().onClick.AddListener(Retreat);
        GameControllerScript.Instance.actionButtons[0].GetComponent<OnHoverBehaviour>().hoveringDisplayText = "Select objective";
        GameControllerScript.Instance.actionButtons[1].GetComponent<OnHoverBehaviour>().hoveringDisplayText = "Retreat";
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
        
        //Force button width update
        GameControllerScript.Instance.interactableButtonManager.ForceUpdateOfButtons();
        
        GameControllerScript.Instance.actionCanvas.SetActive(false);
        GameControllerScript.Instance.PauseGame();
    }

    private void SelectResource(ResourceEnum resource) {
        gathererBehaviour.resourceGatheringType = resource;
        gathererBehaviour.DisplayAction(GameControllerScript.Instance.oreListImage[resource].sprite);
        GameControllerScript.Instance.CalculateOreForGatherer(gameObject);
        GameControllerScript.Instance.interactableButtonManager.gameObject.SetActive(false);
        GameControllerScript.Instance.PlayVelocity(Constants.TIME_SCALE_NORMAL);
    }

    private void Retreat() {
        gathererBehaviour.ReturnToBase(false);
    }
}
