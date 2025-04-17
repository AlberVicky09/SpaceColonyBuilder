using UnityEngine.UI;

public class ClickableFoodGenerator : Clickable {

    private FoodGeneratorController foodGeneratorController;
    
    public override void UpdateTexts() {
        if (foodGeneratorController == null) {
            foodGeneratorController = GetComponent<FoodGeneratorController>();
        }

        if (selectedClickable == this) {
            GameControllerScript.Instance.actionText.text = "Food generator";
        }
    }

    protected override void StartButtons() {
        base.StartButtons();
        if (foodGeneratorController == null) {
            foodGeneratorController = GetComponent<FoodGeneratorController>();
        }
        GameControllerScript.Instance.actionButtons[0].GetComponent<Button>().onClick.AddListener(ToggleGenerator);
        GameControllerScript.Instance.actionButtons[0].GetComponent<OnHoverBehaviour>().hoveringDisplayText = 
            foodGeneratorController.isGeneratorPaused ? "Resume generator" : "Pause generator";
    }

    private void ToggleGenerator() {
        foodGeneratorController.isGeneratorPaused = !foodGeneratorController.isGeneratorPaused;
        foodGeneratorController.ToggleActionCanvas(!foodGeneratorController.isGeneratorPaused);
        GameControllerScript.Instance.actionButtons[0].GetComponent<OnHoverBehaviour>().hoveringDisplayText = 
            foodGeneratorController.isGeneratorPaused ? "Resume generator" : "Pause generator";
        GameControllerScript.Instance.actionButtons[0].GetComponent<OnHoverBehaviour>().RefreshText();
    }
}
