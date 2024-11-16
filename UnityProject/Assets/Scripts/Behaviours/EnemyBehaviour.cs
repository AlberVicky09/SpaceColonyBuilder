using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour {
    
    public NavMeshAgent agent;
    public GameObject objectiveGO;
    private PropsEnum objectiveType;
    private FighterStatesEnum currentState = FighterStatesEnum.Chasing;

    private const float TIME_TO_CHECK_FOR_ENEMIES = 1f;
    private float timeSinceLastCheckForEnemies = TIME_TO_CHECK_FOR_ENEMIES;
    private const float MAXIMUM_FIGHTER_ATTACKING_DISTANCE = 5f;
    private const float MAXIMUM_BUILDING_ATTACKING_DISTANCE = 8f;
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, MAXIMUM_BUILDING_ATTACKING_DISTANCE);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, MAXIMUM_FIGHTER_ATTACKING_DISTANCE);
    }
    
    // Update is called once per frame
    void Update()
    {
        //Check for enemies each TIME_TO_CHECK_FOR_ENEMIES
        timeSinceLastCheckForEnemies += Time.deltaTime;
        if (timeSinceLastCheckForEnemies >= TIME_TO_CHECK_FOR_ENEMIES) {
            timeSinceLastCheckForEnemies = 0f;

            //If enemy is near enough to shoot (and not already shooting an enemy), stop and start shooting
            if (!FighterStatesEnum.Attacking.Equals(currentState) && Utils.DetectObjective(GameControllerScript.Instance.propDictionary[PropsEnum.BasicFighter], transform,
                    MAXIMUM_FIGHTER_ATTACKING_DISTANCE, ref currentState, ref objectiveGO)) {
                currentState = FighterStatesEnum.Attacking;
                objectiveType = PropsEnum.BasicFighter;
                //Stop any prev shooting
                StopCoroutine(StartFighting());
                StartCoroutine(StartFighting());
            //If we havent detected any enemy, check building
            }else if (FighterStatesEnum.Chasing.Equals(currentState) && Utils.DetectObjective(GameControllerScript.Instance.propDictionary[PropsEnum.MainBuilding],
                   transform, MAXIMUM_BUILDING_ATTACKING_DISTANCE, ref currentState, ref objectiveGO)) {
                objectiveType = PropsEnum.MainBuilding;
                currentState = FighterStatesEnum.AttackingLowPriority;
                StartCoroutine(StartFighting());
            }
        }
    }

    private IEnumerator StartFighting() {
        //Stop agent
        agent.isStopped = true;
        
        //Rotate agent towards objective
        transform.LookAt(objectiveGO.transform);
        
        //Start shooting to objective until dead
        while (GameControllerScript.Instance.propDictionary[objectiveType].Contains(objectiveGO)) {
            //Spawn bullet in front
            var bullet = GameControllerScript.Instance.bulletPoolController.GetBullet();
            bullet.transform.position = transform.position;
            
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
        currentState = FighterStatesEnum.Chasing;
        timeSinceLastCheckForEnemies = TIME_TO_CHECK_FOR_ENEMIES;
        agent.isStopped = false;
        agent.SetDestination(GameControllerScript.Instance.mainBuilding.transform.position);
    }
}
