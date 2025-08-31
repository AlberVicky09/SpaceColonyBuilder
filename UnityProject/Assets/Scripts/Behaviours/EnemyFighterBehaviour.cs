public class EnemyFighterBehaviour : FighterBehaviour {
    
    private void Start() {
        waypoints = EnemyBaseController.Instance.waypoints;
        propType = PropsEnum.BasicEnemy;
        oppositeType = PropsEnum.BasicFighter;
        oppositeBase = GameControllerScript.Instance.propDictionary[PropsEnum.MainBuilding][0];
        isActivated = true;
    }
}