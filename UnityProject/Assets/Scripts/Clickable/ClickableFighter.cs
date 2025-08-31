using UnityEngine.UI;

public class ClickableFighter : Clickable {
    
    private PlayerFighterBehaviour playerFighterBehaviour;
    
    public override void UpdateTexts() {
        if (selectedClickable == this) {
            GameControllerScript.Instance.actionText.text = "Fighter";
        }
    }

    protected override void DisplayButtons() {
        base.DisplayButtons();
        
        //If there is no enemy base, disable buttons
        if (GameControllerScript.Instance.propDictionary[PropsEnum.EnemyBase].Count == 0) {
            GameControllerScript.Instance.actionButtons[0].SetActive(false);
        }
    }
    
    protected override void StartButtons() {
        if (playerFighterBehaviour == null) {
            playerFighterBehaviour = GetComponent<PlayerFighterBehaviour>();
        }
        
        //If there is no enemy base, disable buttons
        if (GameControllerScript.Instance.propDictionary[PropsEnum.EnemyBase].Count != 0) {
            base.StartButtons();
            GameControllerScript.Instance.actionButtons[0].GetComponent<Button>().onClick.AddListener(ToggleState);
            SetUpToggleButton();
        }
    }

    public void SetUpToggleButton() {
        if (FighterStatesEnum.Scouting.Equals(playerFighterBehaviour.currentState)) {
            GameControllerScript.Instance.actionButtons[0].GetComponent<OnHoverBehaviour>().hoveringDisplayText = "Attack enemy base";
            GameControllerScript.Instance.actionButtons[0].GetComponent<Image>().sprite = buttonImages[0];
        } else {
            GameControllerScript.Instance.actionButtons[0].GetComponent<OnHoverBehaviour>().hoveringDisplayText = "Scout player base";
            GameControllerScript.Instance.actionButtons[0].GetComponent<Image>().sprite = buttonImages[1];
        }
    }
    
    private void ToggleState() {
        if (FighterStatesEnum.Scouting.Equals(playerFighterBehaviour.currentState)) {
            //Start chasing enemy base
            playerFighterBehaviour.StartChasingBase();
        } else {
            //Start scouting
            playerFighterBehaviour.StartScouting();
        }
        //Update screen and force tooltip update
        SetUpToggleButton();
        GameControllerScript.Instance.actionButtons[0].GetComponent<OnHoverBehaviour>().RefreshText();
    }
}
