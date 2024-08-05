public class ClickableOre : Clickable {

    private OreBehaviour oreBehaviour;
    
    public override void UpdateTexts() {
        if (oreBehaviour == null) {
            oreBehaviour = GetComponent<OreBehaviour>();
        }

        if (selectedClickable == this) {
            gameControllerScript.actionText.text = oreBehaviour.resourceType + " ore\nRemaining "
                + (oreBehaviour.MAXGATHEREDTIMES - oreBehaviour.gatheredTimes) + "/" + oreBehaviour.MAXGATHEREDTIMES;
        }
    }
    
    protected override void StartButtons() {}
}