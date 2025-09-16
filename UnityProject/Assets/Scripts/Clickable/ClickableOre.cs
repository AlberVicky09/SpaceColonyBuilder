public class ClickableOre : Clickable {

    public OreBehaviour oreBehaviour;
    
    public override void UpdateTexts() {

        if (selectedClickable == this) {
            GameControllerScript.Instance.actionText.text = oreBehaviour.resourceType + " ore\nRemaining gatherings: "
                + (oreBehaviour.MAXGATHEREDTIMES - oreBehaviour.gatheredTimes) + "/" + oreBehaviour.MAXGATHEREDTIMES;
        }
    }

    protected override void DisplayRepresentation() {
        //Put the ore color
        GameControllerScript.Instance.uiRepresentation.color = Constants.ORE_COLOR_MAP[oreBehaviour.resourceType];
        GameControllerScript.Instance.uiRepresentation.sprite = objectImage;
    }
    
    protected override void StartButtons() {}
}