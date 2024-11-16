using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FighterBehaviour : MonoBehaviour {
    
    public NavMeshAgent agent;
    public GameObject objectiveGO;
    private const float WAYPOINTS_RADIUS = 12f;
    private const float MAXIMUM_DETECTION_DISTANCE = 13f;
    private const float MAXIMUM_SHOOTING_DISTANCE = 5f;
    
    public int numberOfWaypoints = 8;
    public FighterStatesEnum currentState = FighterStatesEnum.Scouting;

    private const float TIME_TO_CHECK_FOR_ENEMIES = 3.5f;
    private float timeSinceLastCheckForEnemies = TIME_TO_CHECK_FOR_ENEMIES;
    private const float TIME_TO_CHECK_FOR_ENEMY_POSITION = 1f;
    private float timeSinceLastCheckForEnemyPosition;
    
    private List<Vector3> waypoints;
    private int currentWaypointIndex;
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, MAXIMUM_DETECTION_DISTANCE);
    }
    
    void Start()
    {
        waypoints = new List<Vector3>();
        var pivotPosition = GameControllerScript.Instance.mainBuilding.transform.position;

        // Calculate waypoints in a circle around the center object
        for (int i = 0; i < numberOfWaypoints; i++)
        {
            var angle = i * Mathf.PI * 2 / numberOfWaypoints;
            var newPos = new Vector3(Mathf.Cos(angle) * WAYPOINTS_RADIUS, pivotPosition.y, Mathf.Sin(angle) * WAYPOINTS_RADIUS);
            waypoints.Add(pivotPosition + newPos);
        }
        
        // Start moving towards the first waypoint
        agent.SetDestination(waypoints[currentWaypointIndex]);
    }

    void Update()
    {
        if (FighterStatesEnum.Scouting.Equals(currentState)) {
            //Check for enemies each TIME_TO_CHECK_FOR_ENEMIES
            timeSinceLastCheckForEnemies += Time.deltaTime;
            if (timeSinceLastCheckForEnemies >= TIME_TO_CHECK_FOR_ENEMIES) {
                timeSinceLastCheckForEnemies = 0f;
                //If enemy is detected, update state, set check for enemy position for next update, and scape from update
                if (Utils.DetectObjective(GameControllerScript.Instance.propDictionary[PropsEnum.BasicEnemy], transform,
                        MAXIMUM_DETECTION_DISTANCE, ref currentState, ref objectiveGO)) {
                    timeSinceLastCheckForEnemyPosition = TIME_TO_CHECK_FOR_ENEMY_POSITION;
                    agent.SetDestination(objectiveGO.transform.position);
                    return;
                }
            }
            
            // Check if the agent has reached the current waypoint
            if (!agent.pathPending && agent.remainingDistance < 0.5f) {
                MoveToNextWayPoint();
            }
        } else if (FighterStatesEnum.Chasing.Equals(currentState)) {
            timeSinceLastCheckForEnemyPosition += Time.deltaTime;
            
            //Update enemy position each X seconds
            if (timeSinceLastCheckForEnemyPosition >= TIME_TO_CHECK_FOR_ENEMY_POSITION) {
                agent.destination = objectiveGO.transform.position;
            }
            
            //If enemy is near enough, start attacking it
            if (!agent.pathPending && agent.remainingDistance < MAXIMUM_SHOOTING_DISTANCE) {
                Debug.Log("Enemy detected");
                //Start fighting
                StartCoroutine(StartFighting());
            }
        }
    }

    private IEnumerator StartFighting() {
        //Stop agent
        agent.isStopped = true;
        currentState = FighterStatesEnum.Attacking;
        
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
        currentState = FighterStatesEnum.Scouting;
        timeSinceLastCheckForEnemies = TIME_TO_CHECK_FOR_ENEMIES;
        agent.isStopped = false;
        MoveToNextWayPoint();
    }

    private void MoveToNextWayPoint() {
        // Move to the next waypoint
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
        agent.SetDestination(waypoints[currentWaypointIndex]);
    }
}

