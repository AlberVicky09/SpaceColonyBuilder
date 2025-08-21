using UnityEngine.UI;

public class PlaceableFoodGenerator : Placeable {

    public FoodGeneratorBehaviour foodGeneratorBehaviour;
    
    //Start variables once its placed and start generating coroutine
    public override void OnPropPlaced() {
        foodGeneratorBehaviour.actionPercentageImage = foodGeneratorBehaviour.actionPercentage.GetComponent<Image>();
        foodGeneratorBehaviour.generatorCoroutine = StartCoroutine(foodGeneratorBehaviour.GenerateFood());
    }
}
