using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class FighterBehaviour : ActionUIController{
    
    public NavMeshAgent agent;
    public PropsEnum propType;

    //OppositeType is BasicEnemy for player, and BasicFighter for enemy
    protected PropsEnum oppositeType;
    //OpositeBase is enemyBase for player, and mainBase for enemy
    protected GameObject oppositeBase;
    protected PropsEnum oppositeBaseType;
    //Set waypoints for this type, around its base
    protected List<Vector3> waypoints;
    
    protected PropsEnum? currentObjectiveType;
    
    public GameObject objectiveGO;
    public PropsEnum objectiveType;

    public FighterStatesEnum currentState = FighterStatesEnum.Scouting;
    protected FighterStatesEnum prevState;
    public ClickableFighter clickableFighter;
    public int currentWaypointIndex;

    public bool isActivated;

    protected const float TIME_TO_CHECK_FOR_ENEMIES = 1f;
    protected float timeSinceLastCheckForEnemies = TIME_TO_CHECK_FOR_ENEMIES;
    protected const float TIME_TO_CHECK_FOR_ENEMY_POSITION = 1f;
    protected float timeSinceLastCheckForEnemyPosition = TIME_TO_CHECK_FOR_ENEMY_POSITION;

    protected const float MAXIMUM_DETECTION_DISTANCE = 13f;
    protected const float MAXIMUM_FIGHTER_ATTACKING_DISTANCE = 6f;
    protected const float MAXIMUM_BUILDING_ATTACKING_DISTANCE = 7f;
    
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, MAXIMUM_BUILDING_ATTACKING_DISTANCE);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, MAXIMUM_FIGHTER_ATTACKING_DISTANCE);
    }

    private void Start() { DisplayAction(GameControllerScript.Instance.patrolBaseSprite); }

    void Update() {
        if (isActivated) {
            base.Update();
            
            switch (currentState) {
                //When scouting, check for enemies and if not, go on with next waypoint
                case FighterStatesEnum.Scouting:
                    //Check for enemies nearby (if none, keep scouting)
                    if (CheckForEnemiesInSight()) { return; }
                    
                    // Check if the agent has reached the current waypoint, and if so, move to the next one
                    if (!agent.pathPending && agent.remainingDistance < 0.5f) {
                        Utils.MoveToNextWayPoint(
                            ref currentWaypointIndex,
                            waypoints,
                            agent);
                    }
                    break;
                
                //If chasing low priority (base), check for enemies just in case (its more priority)
                case FighterStatesEnum.ChasingLowPriority:
                    //If there are enemies on sight, change objective and go towards them
                    if (CheckForEnemiesInSight()) { return; }

                    //If base is near enough, start attacking it
                    if (!agent.pathPending && agent.remainingDistance < MAXIMUM_BUILDING_ATTACKING_DISTANCE) {
                        UpdateState(FighterStatesEnum.AttackingLowPriority);
                        //Start fighting
                        StartCoroutine(StartFighting());
                    }
                    break;
                
                //If fighting low priority (base), check for enemies just in case (its more priority)
                case FighterStatesEnum.AttackingLowPriority:
                    //If there are enemies on sight, change objective and go towards them
                    CheckForEnemiesInSight();
                    break;
                
                //When the fighter is chasing enemies, won´t stop until being close enough
                case FighterStatesEnum.Chasing:
                    //If other ship destroys current enemy
                    if (!GameControllerScript.Instance.propDictionary[objectiveType].Contains(objectiveGO)) {
                        Debug.Log("Enemy of type " + objectiveType + "has dissapeared");
                        RestartAgent();
                        return;
                    }

                    //Update enemy position each X seconds (in case it has moved)
                    timeSinceLastCheckForEnemyPosition += Time.deltaTime;
                    if (timeSinceLastCheckForEnemyPosition >= TIME_TO_CHECK_FOR_ENEMY_POSITION) {
                        UpdateFighterDestination(objectiveGO.transform.position);
                    }

                    //If enemy is near enough, start attacking it
                    if (!agent.pathPending && agent.remainingDistance < MAXIMUM_FIGHTER_ATTACKING_DISTANCE) {
                        UpdateState(FighterStatesEnum.Attacking);
                        //Start fighting
                        StartCoroutine(StartFighting());
                    }
                    break;
                
                //If its attacking, do nothing
                case FighterStatesEnum.Attacking: break;
            }
        }
    }
    
    private bool CheckForEnemiesInSight() {
        //Check for enemies each TIME_TO_CHECK_FOR_ENEMIES
        timeSinceLastCheckForEnemies += Time.deltaTime;
        if (timeSinceLastCheckForEnemies >= TIME_TO_CHECK_FOR_ENEMIES) {
            timeSinceLastCheckForEnemies = 0f;
            //If enemy is detected, start chasing it
            if (Utils.DetectObjective(GameControllerScript.Instance.propDictionary[oppositeType],
                    transform, MAXIMUM_DETECTION_DISTANCE, ref objectiveGO)) {
                //Set state
                prevState = currentState;
                UpdateState(FighterStatesEnum.Chasing);
                currentObjectiveType = oppositeType;
                
                //Force timer to check for enemies just in case is near enough to shoot
                timeSinceLastCheckForEnemyPosition = TIME_TO_CHECK_FOR_ENEMY_POSITION;
                UpdateFighterDestination(objectiveGO.transform.position);
                return true;
            }
        }

        return false;
    }
    
    protected virtual IEnumerator StartFighting() {
        //Display fighting sprite
        DisplayAction(GameControllerScript.Instance.attackSprite);
        
        //Stop agent
        agent.isStopped = true;
        
        //Rotate agent towards objective
        transform.LookAt(objectiveGO.transform);
        
        //Start shooting to objective until dead
        while (GameControllerScript.Instance.propDictionary[currentObjectiveType.Value].Contains(objectiveGO)) {
            //Variables to avoid waiting unnecesarily
            float timer = 0f;
            
            //Spawn bullet in front
            var bullet = GameControllerScript.Instance.bulletPoolController.GetBullet();
            bullet.transform.position = transform.position + Constants.BULLET_OFFSET;
            
            //Set bullet direction
            bullet.SetActive(true);
            bullet.GetComponent<Rigidbody>().velocity =
                (objectiveGO.transform.position - transform.position) * 0.5f;
            //Set bullet shooter
            bullet.GetComponent<BulletBehaviour>().SetShooter(propType, gameObject);
            //Activate bullet sound
            bullet.GetComponent<SfxSource>().PlaySfx();
            
            // Wait until "reload" happened OR the enemy is already destroyed
            while (timer < Constants.SHOOTING_RELOAD_TIME) {
                if (!GameControllerScript.Instance.propDictionary[currentObjectiveType.Value].Contains(objectiveGO)) {
                    Debug.Log("Enemy destroyed before reload finished");
                    break;
                }

                timer += Time.deltaTime;
                yield return null; // Wait for next frame
            }

            // Check again if the enemy is gone, and exit the shooting loop if so
            if (!GameControllerScript.Instance.propDictionary[currentObjectiveType.Value].Contains(objectiveGO)) {
                break;
            }
        }
        
        Debug.Log("No more pium pium");
        RestartAgent();
    }

    protected virtual void UpdateState(FighterStatesEnum newState) {
        currentState = newState;
        DisplayAction(GameControllerScript.Instance.fighterActionsDictionary[newState]);
        clickableFighter.UpdateTexts();
    }
    
    public void StartScouting() {
        //Display correct image
        DisplayAction(GameControllerScript.Instance.patrolBaseSprite);
        //Find nearest waypoint
        var nearestScoutingPointPosition =
            Utils.FindNearestGameObjectPositionInList(gameObject, waypoints);
        //Set nearest waypoint as objective
        UpdateFighterDestination(waypoints[nearestScoutingPointPosition]);
        //Set nearest waypoint index as current
        currentWaypointIndex = nearestScoutingPointPosition;
        currentObjectiveType = null;
        
        //Update state
        UpdateState(FighterStatesEnum.Scouting);
    }
    
    public void StartChasingBase() {
        //TODO Add chasing sprite
        DisplayAction(GameControllerScript.Instance.healBaseSprite);
        //Mark the enemy base as objective
        UpdateFighterDestination(oppositeBase.transform.position);
        objectiveGO = oppositeBase;
        currentObjectiveType = oppositeBaseType;
        //Update state
        UpdateState(FighterStatesEnum.ChasingLowPriority);
    }
    
    protected void UpdateFighterDestination(Vector3 destination) {
        agent.SetDestination(destination);
    }

    protected void RestartAgent() {
        //Ensure its not stopped, and force checking for enemies
        agent.isStopped = false;
        timeSinceLastCheckForEnemies = TIME_TO_CHECK_FOR_ENEMIES;
        objectiveGO = null;
        
        //Restart the agent, depending on what it was doing before
        switch (prevState) {
            //If it was chasing or attacking the base, get back to chasing it
            case FighterStatesEnum.ChasingLowPriority:
            case FighterStatesEnum.AttackingLowPriority:
                Debug.Log("Chasing base again");
                StartChasingBase();
                break;
            
            //In any other case, go back to scouting
            default:
                Debug.Log("Scouting again");
                StartScouting();
                break;
        }
    }
}