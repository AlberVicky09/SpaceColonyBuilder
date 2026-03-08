using System.Collections;
using UnityEngine;

public class FoodGeneratorBehaviour : ActionUIController_v2 {

    public Coroutine generatorCoroutine;
    public bool isGeneratorPaused = false;
    private bool hasWaterBeenAlreadyTaken = false;

    public void Start() {
        totalProgressTime = Constants.FOOD_GENERATOR_DURATION;
    }
    
    public IEnumerator GenerateFood() {
        while (true) {
            //Check if has been paused, to stop it AFTER the loop is done
            if (isGeneratorPaused) {
                DisplayAction(GameControllerScript.Instance.stopActionSprite);
            }
            //If is paused, dont generate
            yield return new WaitWhile(() => isGeneratorPaused);
            
            if (ActivateConditions()) {
                //Take water only once, even if we pause it
                if (!hasWaterBeenAlreadyTaken) {
                    //Remove water
                    GameControllerScript.Instance.uiUpdateController.UpdateResource(ResourceEnum.Water, 15,
                        ResourceOperationEnum.Decrease);
                    hasWaterBeenAlreadyTaken = true;
                }
                
                //Swap action canvas
                DisplayProgress();
                yield return new WaitForSeconds(5);
                
                //Add food
                GameControllerScript.Instance.uiUpdateController.UpdateResource(ResourceEnum.Food, 30,
                    ResourceOperationEnum.Increase);
                
                //Ensure next time water is taken again
                hasWaterBeenAlreadyTaken = false;
            } else {
                if (GameControllerScript.Instance.resourcesDictionary[ResourceEnum.Water] < 15) {
                    DisplayAction(GameControllerScript.Instance.missingResourceSpriteDictionary[ResourceEnum.Water]);
                } else {
                    DisplayAction(GameControllerScript.Instance.missingResourceSpriteDictionary[ResourceEnum.Food]);
                }
                
                yield return new WaitUntil(() => ActivateConditions());
            }
        }
    }

    //Only activate if enough water and 
    private bool ActivateConditions() {
        return GameControllerScript.Instance.resourcesDictionary[ResourceEnum.Water] >= 15
               && GameControllerScript.Instance.resourcesDictionary[ResourceEnum.Food] < GameControllerScript.Instance.resourcesLimit;
    }
}
