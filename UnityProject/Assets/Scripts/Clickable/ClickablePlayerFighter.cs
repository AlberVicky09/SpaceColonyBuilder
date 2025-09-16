using UnityEngine;
using UnityEngine.UI;

public class ClickablePlayerFighter : ClickableFighter {
    
    protected override void DisplayButtons() {
        base.DisplayButtons();
        
        //If there is no enemy base, disable buttons
        if (GameControllerScript.Instance.propDictionary[PropsEnum.EnemyBase].Count == 0) {
            GameControllerScript.Instance.actionButtons[0].SetActive(false);
        } else {
            var isActive = !(FighterStatesEnum.Attacking.Equals(fighterBehaviour.currentState) ||
                             FighterStatesEnum.AttackingLowPriority.Equals(fighterBehaviour.currentState));
            SetUpToggleButton(isActive);
        }
    }
    
    protected override void DisplayRepresentation() {
        //Put the original color
        GameControllerScript.Instance.uiRepresentation.color = Color.white;
        GameControllerScript.Instance.uiRepresentation.sprite = objectImage;
    }
    
    protected override void StartButtons() {
        if (fighterBehaviour == null) {
            fighterBehaviour = GetComponent<PlayerFighterBehaviour>();
        }
        
        //If there is no enemy base, dont put buttons to work
        if (GameControllerScript.Instance.propDictionary[PropsEnum.EnemyBase].Count != 0) {
            base.StartButtons();
            GameControllerScript.Instance.actionButtons[0].GetComponent<Button>().onClick.AddListener(ToggleState);
        }
    }

    public void SetUpToggleButton(bool active) {
        if (selectedClickable == this) {
            GameControllerScript.Instance.actionButtons[0].GetComponent<Button>().interactable = active;
            
            if (FighterStatesEnum.Scouting.Equals(fighterBehaviour.currentState)) {
                GameControllerScript.Instance.actionButtons[0].GetComponent<OnHoverBehaviour>().hoveringDisplayText =
                    "Attack enemy base";
                GameControllerScript.Instance.actionButtons[0].GetComponent<Image>().sprite =
                    active ? buttonImages[0] : buttonImages[2];
            } else {
                GameControllerScript.Instance.actionButtons[0].GetComponent<OnHoverBehaviour>().hoveringDisplayText =
                    "Scout player base";
                GameControllerScript.Instance.actionButtons[0].GetComponent<Image>().sprite =
                    active ? buttonImages[1] : buttonImages[3];
            }
        }
    }
    
    private void ToggleState() {
        if (FighterStatesEnum.Scouting.Equals(fighterBehaviour.currentState)) {
            //Start chasing enemy base
            fighterBehaviour.StartChasingBase();
        } else {
            //Start scouting
            fighterBehaviour.StartScouting();
        }
        //Update screen and force tooltip update
        SetUpToggleButton(true);
        GameControllerScript.Instance.actionButtons[0].GetComponent<OnHoverBehaviour>().RefreshText();
    }
}
