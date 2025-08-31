public class PlayerFighterBehaviour : FighterBehaviour {
    
    private void Start() {
        waypoints = GameControllerScript.Instance.waypoints;
        propType = PropsEnum.BasicFighter;
        oppositeType = PropsEnum.BasicEnemy;
        oppositeBase = GameControllerScript.Instance.propDictionary[PropsEnum.EnemyBase].Count != 0
            ? GameControllerScript.Instance.propDictionary[PropsEnum.EnemyBase][0] : null;
        isActivated = false;
    }
}

