public class ClickableFighter : Clickable {

    public FighterBehaviour fighterBehaviour;
    public PropStats propStats;
    
    protected override void DisplayRepresentation() {
        //Put the enemy color
        GameControllerScript.Instance.uiRepresentation.color = Constants.ENEMY_MASK_COLOR;
        GameControllerScript.Instance.uiRepresentation.sprite = objectImage;
    }
    
    public override void UpdateTexts() {
        if (selectedClickable == this) {
            GameControllerScript.Instance.actionText.text = "Fighter " + Constants.FIGHTER_STATE_DISPLAY_NAME[fighterBehaviour.currentState]
                + "\nHealth: " + propStats.healthPoints + "/" + propStats.MAX_HEALTHPOINTS;
        }
    }
}
