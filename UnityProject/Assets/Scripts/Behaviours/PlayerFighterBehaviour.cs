public class PlayerFighterBehaviour : FighterBehaviour {
    
    private void Awake() {
        waypoints = GameControllerScript.Instance.waypoints;
        propType = PropsEnum.Fighter;
        oppositeType = PropsEnum.EnemyFighter;
        oppositeBase = GameControllerScript.Instance.propDictionary[PropsEnum.EnemyBase].Count != 0
            ? GameControllerScript.Instance.propDictionary[PropsEnum.EnemyBase][0] : null;
        oppositeBaseType = PropsEnum.EnemyBase;
        isActivated = false;
    }
}

