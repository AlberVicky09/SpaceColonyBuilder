public abstract class ClickableGatherer : Clickable {

    protected GathererBehaviour gathererBehaviour;

    public override void UpdateTexts() {
        if (selectedClickable == this) {
            GameControllerScript.Instance.actionText.text = "Gatherer load\n" + gathererBehaviour.gathererLoad + "/" +
                                                   gathererBehaviour.maxGathererLoad;
        }
    }
    
    protected override void StartButtons() {
        base.StartButtons();
        if (gathererBehaviour == null) {
            gathererBehaviour = GetComponent<GathererBehaviour>();
        }
    }
}
