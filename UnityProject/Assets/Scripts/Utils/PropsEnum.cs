using System.Collections.Generic;

public enum PropsEnum
{
   Gatherer,
   EnemyGatherer,
   FoodGenerator,
   EnemyFoodGenerator,
   Fighter,
   EnemyFighter,
   House,
   Storage,
   MainBuilding,
   EnemyBase,
}

public static class BuildableProps {
   public static List<PropsEnum> RetrieveBuildableProps() {
      return new List<PropsEnum>{PropsEnum.Gatherer, PropsEnum.Fighter, PropsEnum.FoodGenerator, PropsEnum.Storage};
   }
}