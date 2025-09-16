public abstract class ClickableGatherer : Clickable {

    public GathererBehaviour gathererBehaviour;

    public override void UpdateTexts() {
        if (selectedClickable == this) {
            GameControllerScript.Instance.actionText.text = "Gatherer load\n" + gathererBehaviour.gathererLoad + "/" +
                                                   gathererBehaviour.maxGathererLoad;
        }
    }
}
