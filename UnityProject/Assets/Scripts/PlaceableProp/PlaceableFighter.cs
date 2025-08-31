using UnityEngine.Serialization;

public class PlaceableFighter : Placeable {
    
    [FormerlySerializedAs("fighterBehaviour")] public PlayerFighterBehaviour playerFighterBehaviour;
    
    //Start scouting once its placed
    public override void OnPropPlaced() {
        // Start moving towards the first waypoint
        playerFighterBehaviour.currentWaypointIndex = 0;
        playerFighterBehaviour.StartScouting();
        playerFighterBehaviour.isActivated = true;
    }
}
