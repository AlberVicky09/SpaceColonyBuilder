public class ClickableEnemyGatherer : ClickableGatherer {
    
    protected override void DisplayRepresentation() {
        //Put the enemy color
        GameControllerScript.Instance.uiRepresentation.color = Constants.ENEMY_MASK_COLOR;
        GameControllerScript.Instance.uiRepresentation.sprite = objectImage;
    }
    
    protected override void StartButtons() {
        if (gathererBehaviour == null) {
            gathererBehaviour = GetComponent<GathererBehaviour>();
        }
    }
}