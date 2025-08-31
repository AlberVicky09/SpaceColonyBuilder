using UnityEngine.UI;

public class ClickableFoodGenerator : Clickable {

    private FoodGeneratorBehaviour foodGeneratorBehaviour;
    
    public override void UpdateTexts() {
        if (foodGeneratorBehaviour == null) {
            foodGeneratorBehaviour = GetComponent<FoodGeneratorBehaviour>();
        }

        if (selectedClickable == this) {
            GameControllerScript.Instance.actionText.text = "Food generator";
        }
    }

    protected override void StartButtons() {
        base.StartButtons();
        if (foodGeneratorBehaviour == null) {
            foodGeneratorBehaviour = GetComponent<FoodGeneratorBehaviour>();
        }
        GameControllerScript.Instance.actionButtons[0].GetComponent<Button>().onClick.AddListener(ToggleGenerator);
        GameControllerScript.Instance.actionButtons[0].GetComponent<OnHoverBehaviour>().hoveringDisplayText = 
            foodGeneratorBehaviour.isGeneratorPaused ? "Resume generator" : "Pause generator";
    }

    private void ToggleGenerator() {
        foodGeneratorBehaviour.isGeneratorPaused = !foodGeneratorBehaviour.isGeneratorPaused;
        if (foodGeneratorBehaviour.isGeneratorPaused) {
            GameControllerScript.Instance.actionButtons[0].GetComponent<OnHoverBehaviour>().hoveringDisplayText = "Resume generator";
            GameControllerScript.Instance.actionButtons[0].GetComponent<Image>().sprite = GameControllerScript.Instance.startActionSprite;
        } else {
            GameControllerScript.Instance.actionButtons[0].GetComponent<OnHoverBehaviour>().hoveringDisplayText = "Pause generator";
            GameControllerScript.Instance.actionButtons[0].GetComponent<Image>().sprite = GameControllerScript.Instance.stopActionSprite;
        }

        GameControllerScript.Instance.actionButtons[0].GetComponent<OnHoverBehaviour>().RefreshText();
    }

    protected override void DisplayButtons() {
        //Disable all buttons (but first)
        for(int i = 1; i < GameControllerScript.Instance.actionButtons.Length; i++) {
            GameControllerScript.Instance.actionButtons[i].SetActive(false);
        }
        
        //Ensure image is correct with current state
        GameControllerScript.Instance.actionButtons[0].SetActive(true);
        GameControllerScript.Instance.actionButtons[0].GetComponent<Image>().sprite =
            foodGeneratorBehaviour.isGeneratorPaused
                ? GameControllerScript.Instance.startActionSprite
                : GameControllerScript.Instance.stopActionSprite;
    }
}
