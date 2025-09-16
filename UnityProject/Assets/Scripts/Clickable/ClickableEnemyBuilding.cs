public class ClickableEnemyBuilding : Clickable {

    public PropStats propStats;
    
    private void Awake() {
        buttonNumber = 0;
    }
    
    public override void UpdateTexts() {

        if (selectedClickable == this) {
            GameControllerScript.Instance.actionText.text = "Enemy base health: "
                + propStats.healthPoints + "/" + propStats.MAX_HEALTHPOINTS;
        }
    }
    
    protected override void DisplayRepresentation() {
        //Put the enemy color
        GameControllerScript.Instance.uiRepresentation.color = Constants.ENEMY_MASK_COLOR;
        GameControllerScript.Instance.uiRepresentation.sprite = objectImage;
    }
    
    protected override void StartButtons() {}
}