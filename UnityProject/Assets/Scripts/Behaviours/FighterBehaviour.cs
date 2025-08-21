using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class FighterBehaviour : MonoBehaviour {
    
    public NavMeshAgent agent;
    public GameObject objectiveGO;
    private const float MAXIMUM_DETECTION_DISTANCE = 13f;
    private const float MAXIMUM_SHOOTING_DISTANCE = 5f;
    private const float MAXIMUM_BUILDING_ATTACKING_DISTANCE = 8f;
    
    public FighterStatesEnum currentState = FighterStatesEnum.Scouting;
    private FighterStatesEnum prevState;
    public bool hasBeenPlaced = false;
    
    private const float TIME_TO_CHECK_FOR_ENEMIES = 3.5f;
    private float timeSinceLastCheckForEnemies = TIME_TO_CHECK_FOR_ENEMIES;
    private const float TIME_TO_CHECK_FOR_ENEMY_POSITION = 1f;
    private float timeSinceLastCheckForEnemyPosition;
    
    public int currentWaypointIndex;
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, MAXIMUM_DETECTION_DISTANCE);
    }

    void Update()
    {
        if (hasBeenPlaced && !FighterStatesEnum.Attacking.Equals(currentState)) {
            switch (currentState) {
                //When the fighter is scouting, it can find enemies
                case FighterStatesEnum.Scouting:
                    //Check for enemies nearby
                    if (!CheckForEnemiesNearby(MAXIMUM_DETECTION_DISTANCE, false)) {
                        // Check if the agent has reached the current waypoint, and if so, move to the next one
                        if (!agent.pathPending && agent.remainingDistance < 0.5f) {
                            Debug.Log("Moving to next waypoint");
                            Utils.MoveToNextWayPoint(ref currentWaypointIndex, GameControllerScript.Instance.waypoints,
                                agent);
                        }
                    }

                    break;

                //When the fighter is chasing enemies, wont stop until being close enough
                case FighterStatesEnum.Chasing:
                    timeSinceLastCheckForEnemyPosition += Time.deltaTime;

                    //Update enemy position each X seconds (in case it has moved)
                    if (timeSinceLastCheckForEnemyPosition >= TIME_TO_CHECK_FOR_ENEMY_POSITION) {
                        UpdateFighterDestination(objectiveGO.transform.position);
                    }

                    //If enemy is near enough, start attacking it
                    if (!agent.pathPending && agent.remainingDistance < MAXIMUM_SHOOTING_DISTANCE) {
                        Debug.Log("Enemy in range");
                        prevState = currentState;
                        currentState = FighterStatesEnum.Attacking;
                        //Start fighting
                        StartCoroutine(StartFighting());
                    }

                    break;

                //When the fighter is chasing the base, check if the base is near enough or if there are enemies nearby
                case FighterStatesEnum.ChasingLowPriority:

                    //If base is near enough, start attacking it
                    if (!agent.pathPending && agent.remainingDistance < MAXIMUM_SHOOTING_DISTANCE) {
                        Debug.Log("Base in range");
                        prevState = currentState;
                        currentState = FighterStatesEnum.AttackingLowPriority;
                        //Start fighting
                        StartCoroutine(StartFighting());
                    }

                    break;

                case FighterStatesEnum.AttackingLowPriority:
                    //If its atacking the base, check if there is a enemy nearby
                    if (CheckForEnemiesNearby(MAXIMUM_DETECTION_DISTANCE, true)) {
                        Debug.Log("Stop attacking the base, start chasing enemy");
                        StopCoroutine(StartFighting());
                        prevState = currentState;
                        currentState = FighterStatesEnum.Chasing;
                    }

                    break;
            }
        }
    }

    private IEnumerator StartFighting() {
        //Stop agent
        agent.isStopped = true;

        //Start shooting to objective, finish when objective is death
        while (GameControllerScript.Instance.propDictionary[PropsEnum.BasicEnemy].Contains(objectiveGO)) {
            //Spawn bullet in front
            var bullet = GameControllerScript.Instance.bulletPoolController.GetBullet();
            bullet.transform.position = transform.position + Vector3.forward * 0.5f;

            //Set bullet direction
            bullet.SetActive(true);
            bullet.GetComponent<Rigidbody>().velocity =
                (objectiveGO.transform.position - transform.position) * 0.5f;
            //Set bullet shooter
            var bulletBehaviour = bullet.GetComponent<BulletBehaviour>();
            bulletBehaviour.SetShooter(PropsEnum.BasicEnemy, gameObject);

            //Reload
            yield return new WaitForSeconds(2.5f);
        }

        //Restart the agent, and instantly find enemies
        currentState = prevState;
        switch (currentState) {
            //TODO missing states?
            case FighterStatesEnum.Scouting:
                timeSinceLastCheckForEnemies = TIME_TO_CHECK_FOR_ENEMIES;
                agent.isStopped = false;
                Utils.MoveToNextWayPoint(ref currentWaypointIndex, GameControllerScript.Instance.waypoints, agent);
                break;
                
            case FighterStatesEnum.Chasing:
                break;
            
            case FighterStatesEnum.ChasingLowPriority:
                break;
                
            case FighterStatesEnum.AttackingLowPriority:
                break;
        }
    }

    private bool CheckForEnemiesNearby(float checkDistance, bool isCheckingForBase) {
        //Check for enemies each TIME_TO_CHECK_FOR_ENEMIES
        timeSinceLastCheckForEnemies += Time.deltaTime;
        if (timeSinceLastCheckForEnemies >= TIME_TO_CHECK_FOR_ENEMIES) {
            timeSinceLastCheckForEnemies = 0f;
            //If enemy is detected, start chasing it
            if (Utils.DetectObjective(GameControllerScript.Instance.propDictionary[PropsEnum.BasicEnemy],
                    transform, checkDistance, ref objectiveGO)) {
                prevState = currentState;
                currentState = isCheckingForBase ? FighterStatesEnum.ChasingLowPriority : FighterStatesEnum.Chasing;
                
                timeSinceLastCheckForEnemyPosition = TIME_TO_CHECK_FOR_ENEMY_POSITION;
                UpdateFighterDestination(objectiveGO.transform.position);
                return true;
            }
        }

        return false;
    }

    public void StartScouting() {
        //Find nearest waypoint
        var nearestScoutingPointPosition =
            Utils.FindNearestGameObjectPositionInList(gameObject, GameControllerScript.Instance.waypoints);
        //Set nearest waypoint as objective
        UpdateFighterDestination(
            GameControllerScript.Instance.waypoints[nearestScoutingPointPosition]);
        //Set nearest waypoint as current
        currentWaypointIndex = nearestScoutingPointPosition;
        //Update state
        currentState = FighterStatesEnum.Scouting;
    }

    public void StartChasing() {
        //Mark the enemy base as objectiveS
        UpdateFighterDestination(EnemyBaseController.Instance.mainEnemyBase.transform.position);
        //Update state
        currentState = FighterStatesEnum.AttackingLowPriority;
    }
    
    public void UpdateFighterDestination(Vector3 destination) {
        agent.SetDestination(destination);
    }
}

