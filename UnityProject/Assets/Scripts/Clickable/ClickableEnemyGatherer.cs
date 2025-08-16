public class ClickableEnemyGatherer : ClickableGatherer {
    
    protected override void StartButtons() {
        if (gathererBehaviour == null) {
            gathererBehaviour = GetComponent<GathererBehaviour>();
        }
    }
}