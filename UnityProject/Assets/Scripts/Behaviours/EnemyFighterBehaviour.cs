public class EnemyFighterBehaviour : FighterBehaviour {
    
    private void Start() {
        waypoints = EnemyBaseController.Instance.waypoints;
        propType = PropsEnum.EnemyFighter;
        oppositeType = PropsEnum.Fighter;
        oppositeBase = GameControllerScript.Instance.propDictionary[PropsEnum.MainBuilding][0];
        isActivated = true;
    }
}