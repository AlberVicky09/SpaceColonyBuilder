using System;
using System.Collections.Generic;

public class PlaceableGatherer : Placeable {
    
    public GathererBehaviour gathererBehaviour;

    //Start variables once its placed
    public override void OnPropPlaced() {
        gathererBehaviour.maxGathererLoad = Constants.DEFAULT_GATHERER_MAX_LOAD;
        gathererBehaviour.loadDictionary = new Dictionary<ResourceEnum, int>();
        foreach (ResourceEnum resource in Enum.GetValues(typeof(ResourceEnum))) {
            gathererBehaviour.loadDictionary.Add(resource, 0);
        }
        gathererBehaviour.DisplayAction(GameControllerScript.Instance.stopActionSprite);
    }
}
