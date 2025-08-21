using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlaceableGatherer : Placeable {
    
    public GathererBehaviour gathererBehaviour;

    //Start variables once its placed
    public override void OnPropPlaced() {
        gathererBehaviour.actionProgressImage = gathererBehaviour.actionProgress.GetComponent<Image>();
        gathererBehaviour.currentActionImage = gathererBehaviour.currentAction.GetComponent<Image>();
        
        gathererBehaviour.maxGathererLoad = Constants.DEFAULT_GATHERER_MAX_LOAD;
        gathererBehaviour.loadDictionary = new Dictionary<ResourceEnum, int>();
        foreach (ResourceEnum resource in Enum.GetValues(typeof(ResourceEnum))) {
            gathererBehaviour.loadDictionary.Add(resource, 0);
        }
    }
}
