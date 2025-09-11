public class ClickableFighter : Clickable {

    public FighterBehaviour fighterBehaviour;
    public PropStats propStats;
    
    public override void UpdateTexts() {
        if (selectedClickable == this) {
            GameControllerScript.Instance.actionText.text = "Fighter " + Constants.FIGHTER_STATE_DISPLAY_NAME[fighterBehaviour.currentState]
                + "\nHealth: " + propStats.healthPoints + "/" + propStats.MAX_HEALTHPOINTS;
        }
    }
}
