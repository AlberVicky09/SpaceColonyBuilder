using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour {
    
    public NavMeshAgent agent;
    public GameObject objectiveGO;
    private FighterStatesEnum currentState = FighterStatesEnum.Chasing;

    private const float TIME_TO_CHECK_FOR_ENEMIES = 1f;
    private float timeSinceLastCheckForEnemies = TIME_TO_CHECK_FOR_ENEMIES;
    private const float MAXIMUM_ATTACKING_DISTANCE = 8f;
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, MAXIMUM_ATTACKING_DISTANCE);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (FighterStatesEnum.Chasing.Equals(currentState)) {
            //Check for enemies each TIME_TO_CHECK_FOR_ENEMIES
            timeSinceLastCheckForEnemies += Time.deltaTime;
            if (timeSinceLastCheckForEnemies >= TIME_TO_CHECK_FOR_ENEMIES) {
                timeSinceLastCheckForEnemies = 0f;
                //If enemy is near enough to shoot, stop and start shooting
                if (Utils.DetectObjective(GameControllerScript.Instance.propDictionary[PropsEnum.BasicFighter], transform,
                        MAXIMUM_ATTACKING_DISTANCE, ref currentState, ref objectiveGO)) {
                    Debug.Log("Fighter detected");
                    StartCoroutine(StartFighting());
                }
            }
        }
    }

    private IEnumerator StartFighting() {
        //Stop agent
        agent.isStopped = true;
        currentState = FighterStatesEnum.Attacking;
        
        //Start shooting to objective (when to finish?)
        while (true) {
            //Spawn bullet in front
            var bullet = GameControllerScript.Instance.bulletPoolController.GetBullet();
            bullet.transform.position = transform.position + Vector3.forward * 0.5f;
            
            //Set bullet direction
            bullet.SetActive(true);
            bullet.GetComponent<Rigidbody>().velocity =
                (objectiveGO.transform.position - transform.position) * 0.5f;
            
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
