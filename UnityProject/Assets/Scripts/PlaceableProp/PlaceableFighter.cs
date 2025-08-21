public class PlaceableFighter : Placeable {
    
    public FighterBehaviour fighterBehaviour;
    
    //Start scouting once its placed
    public override void OnPropPlaced() {
        // Start moving towards the first waypoint
        fighterBehaviour.currentWaypointIndex = 0;
        fighterBehaviour.UpdateFighterDestination(GameControllerScript.Instance.waypoints[fighterBehaviour.currentWaypointIndex]);
        fighterBehaviour.hasBeenPlaced = true;
    }
}
