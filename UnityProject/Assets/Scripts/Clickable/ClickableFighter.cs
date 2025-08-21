using UnityEngine.UI;

public class ClickableFighter : Clickable {
    
    private FighterBehaviour fighterBehaviour;
    
    public override void UpdateTexts() {
        if (selectedClickable == this) {
            GameControllerScript.Instance.actionText.text = "Fighter";
        }
    }

    protected override void DisplayButtons() {
        base.DisplayButtons();
        
        //If there is no enemy base, disable buttons
        if (EnemyBaseController.Instance.mainEnemyBase == null) {
            GameControllerScript.Instance.actionButtons[0].SetActive(false);
        }
    }
    
    protected override void StartButtons() {
        if (fighterBehaviour == null) {
            fighterBehaviour = GetComponent<FighterBehaviour>();
        }
        
        //If there is no enemy base, disable buttons
        if (EnemyBaseController.Instance.mainEnemyBase != null) {
            base.StartButtons();
            GameControllerScript.Instance.actionButtons[0].GetComponent<Button>().onClick.AddListener(ToggleState);
            SetUpToggleButton();
        }
    }

    public void SetUpToggleButton() {
        if (FighterStatesEnum.Scouting.Equals(fighterBehaviour.currentState)) {
            GameControllerScript.Instance.actionButtons[0].GetComponent<OnHoverBehaviour>().hoveringDisplayText = "Attack enemy base";
            GameControllerScript.Instance.actionButtons[0].GetComponent<Image>().sprite = buttonImages[0];
        } else {
            GameControllerScript.Instance.actionButtons[0].GetComponent<OnHoverBehaviour>().hoveringDisplayText = "Scout player base";
            GameControllerScript.Instance.actionButtons[0].GetComponent<Image>().sprite = buttonImages[1];
        }
    }
    
    private void ToggleState() {
        if (FighterStatesEnum.Scouting.Equals(fighterBehaviour.currentState)) {
            //Start chasing enemy base
            fighterBehaviour.StartChasing();
        } else {
            //Start scouting
            fighterBehaviour.StartScouting();
        }
        //Update screen and force tooltip update
        SetUpToggleButton();
        GameControllerScript.Instance.actionButtons[0].GetComponent<OnHoverBehaviour>().RefreshText();
    }
}
