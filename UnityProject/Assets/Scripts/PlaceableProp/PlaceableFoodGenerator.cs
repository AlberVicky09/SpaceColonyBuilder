public class PlaceableFoodGenerator : Placeable {

    public FoodGeneratorBehaviour foodGeneratorBehaviour;
    
    //Start variables once its placed and start generating coroutine
    public override void OnPropPlaced() {
        foodGeneratorBehaviour.generatorCoroutine = StartCoroutine(foodGeneratorBehaviour.GenerateFood());
    }
}
