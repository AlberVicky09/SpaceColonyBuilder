public class EnemyFighterBehaviour : FighterBehaviour {
    
    private void Awake() {
        waypoints = EnemyBaseController.Instance.waypoints;
        propType = PropsEnum.EnemyFighter;
        oppositeType = PropsEnum.Fighter;
        oppositeBase = GameControllerScript.Instance.propDictionary[PropsEnum.MainBuilding][0];
        oppositeBaseType = PropsEnum.MainBuilding;
        isActivated = true;
    }
}