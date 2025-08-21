public class PlaceableStorage : Placeable {
    
    //Increase storage maximum size once its placed
    public override void OnPropPlaced() {
        GameControllerScript.Instance.resourcesLimit += Constants.RESOURCES_LIMIT_INCREASE;
        foreach (var maxResourceText in GameControllerScript.Instance.uiMaxResourcesList) {
            maxResourceText.text = GameControllerScript.Instance.resourcesLimit.ToString();
        }
    }
}
