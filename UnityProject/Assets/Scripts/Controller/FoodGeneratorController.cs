using System.Collections;
using UnityEngine;

public class FoodGeneratorController : MonoBehaviour
{
    private GameControllerScript gameControllerScript;

    private void Start() {
        gameControllerScript = GameObject.Find("GameController").GetComponent<GameControllerScript>();
        StartCoroutine(GenerateFood());
    }

    private IEnumerator GenerateFood() {
        while (true) {
            yield return new WaitForSeconds(5);
            if (gameControllerScript.resourcesDictionary[ResourceEnum.Water] >= 15) {
                Debug.Log("HAZ COMIDA");
                //Remove water
                gameControllerScript.UIUpdateController.UpdateResource(ResourceEnum.Water, 15, ResourceOperationEnum.Decrease);
                StartCoroutine(gameControllerScript.UIUpdateController.DisplayResourceChange(ResourceEnum.Water, 15, ResourceOperationEnum.Decrease));

                //Add food
                gameControllerScript.UIUpdateController.UpdateResource(ResourceEnum.Food, 30, ResourceOperationEnum.Increase);
                StartCoroutine(gameControllerScript.UIUpdateController.DisplayResourceChange(ResourceEnum.Food, 30, ResourceOperationEnum.Increase));
            } else {
                Debug.Log("Missing water");
                //TODO Display the icon of missing water
            }
        }
    }

}
