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
        foodGeneratorBehaviour.ToggleActionCanvas(!foodGeneratorBehaviour.isGeneratorPaused);
        GameControllerScript.Instance.actionButtons[0].GetComponent<OnHoverBehaviour>().hoveringDisplayText = 
            foodGeneratorBehaviour.isGeneratorPaused ? "Resume generator" : "Pause generator";
        GameControllerScript.Instance.actionButtons[0].GetComponent<OnHoverBehaviour>().RefreshText();
    }

    protected override void DisplayButtons() {
        base.DisplayButtons();
        
        //Ensure image is correct with current state
        GameControllerScript.Instance.actionButtons[0].GetComponent<Image>().sprite =
            foodGeneratorBehaviour.isGeneratorPaused ? buttonImages[0] : buttonImages[1];
    }
}
