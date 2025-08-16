public class ClickableEnemyBuilding : Clickable {

    private PropStats propStats;
    
    private void Awake() {
        buttonNumber = 0;
    }
    
    public override void UpdateTexts() {
        if (propStats == null) {
            propStats = GetComponent<PropStats>();
        }

        if (selectedClickable == this) {
            GameControllerScript.Instance.actionText.text = "Enemy base health: "
                + propStats.healthPoints + "/" + propStats.MAX_HEALTHPOINTS;
        }
    }
    
    protected override void StartButtons() {}
}