using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyGenerationController : MonoBehaviour {
    
    public GameObject enemyProp;
    public Transform[] enemyGeneratorPosition;

    public void GenerateNewEnemy(int numberOfEnemies = 1) {
        //Generate the numberOfEnemies in that position
        for (int i = 0; i < numberOfEnemies; i++) {
            //Get next position to generate enemies
            var currentGenerator = enemyGeneratorPosition[Random.Range(0, enemyGeneratorPosition.Length-1)];
            var enemy = Instantiate(enemyProp, currentGenerator);
            
            //Add enemy to enemies prop list
            GameControllerScript.Instance.propDictionary[PropsEnum.BasicEnemy].Add(enemy);
            
            //Set enemy objective to base
            enemy.GetComponent<EnemyBehaviour>().agent.SetDestination(GameControllerScript.Instance.mainBuilding.transform.position);
        }
    }
}