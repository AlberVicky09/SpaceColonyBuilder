using UnityEngine;
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
        GameControllerScript.Instance.interactableButtonManager.PlaceButtonsInCircle(OreResources.RetrieveOreResources().Count);
        
        //Setup buttons behaviour
        var oreResources = OreResources.RetrieveOreResources();
        for(int i = 0; i < oreResources.Count; i++) {
            GameControllerScript.Instance.interactableButtonManager.interactableButtonList[i].GetComponent<Button>().onClick.RemoveAllListeners();
            var currentResource = oreResources[i];
            GameControllerScript.Instance.interactableButtonManager.interactableButtonList[i].GetComponent<Button>().onClick.AddListener(() => { SelectResource(currentResource); });
            GameControllerScript.Instance.interactableButtonManager.interactableButtonImageList[i].sprite = GameControllerScript.Instance.resourceSpriteDictionary[currentResource];
            GameControllerScript.Instance.interactableButtonManager.interactableButtonImageList[i]
                .GetComponent<RectTransform>().localScale = Constants.INTERACTABLE_BUTTON_RESOURCE_SCALE;
            
            //Setup hover behaviour
            var onHoverBehaviour = GameControllerScript.Instance.interactableButtonManager.interactableButtonList[i].GetComponent<OnHoverBehaviour>();
            onHoverBehaviour.hoveringDisplayText = currentResource.ToString();
            onHoverBehaviour.usesResourceTooltip = false;
            onHoverBehaviour.RefreshText();
        }
        
        //Force button width update
        GameControllerScript.Instance.interactableButtonManager.ForceUpdateOfButtons();
        
        GameControllerScript.Instance.actionCanvas.SetActive(false);
        GameControllerScript.Instance.PauseGame();
    }

    private void SelectResource(ResourceEnum resource) {
        gathererBehaviour.resourceGatheringType = resource;
        GameControllerScript.Instance.CalculateOreForGatherer(gameObject);
        GameControllerScript.Instance.interactableButtonManager.gameObject.SetActive(false);
        GameControllerScript.Instance.PlayVelocity(Constants.TIME_SCALE_NORMAL);
    }

    private void Retreat() {
        gathererBehaviour.ReturnToBase(true);
    }
}
