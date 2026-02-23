using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyGenerationController : MonoBehaviour {
    
    public Transform[] enemyGeneratorPosition;

    public void GenerateNewEnemy(int numberOfEnemies = 1) {
        //Generate the numberOfEnemies in that position
        for (int i = 0; i < numberOfEnemies; i++) {
            //Get next position to generate enemies
            var currentGenerator = enemyGeneratorPosition[Random.Range(0, enemyGeneratorPosition.Length-1)];
            var enemy = Instantiate(GameControllerScript.Instance.enemyFighterPrefab, currentGenerator.position, currentGenerator.rotation);
            
            //Add enemy to enemies prop list
            GameControllerScript.Instance.propDictionary[PropsEnum.EnemyFighter].Add(enemy);
            
            //Set enemy objective to base
            enemy.GetComponent<EnemyFighterBehaviour>().StartChasingBase();
        }
    }
}