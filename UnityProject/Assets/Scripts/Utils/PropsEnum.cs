using System.Collections.Generic;

public enum PropsEnum
{
   Gatherer,
   EnemyGatherer,
   FoodGenerator,
   EnemyFoodGenerator,
   BasicFighter,
   BasicEnemy,
   House,
   Storage,
   MainBuilding,
   EnemyBase,
}

public static class BuildableProps {
   public static List<PropsEnum> RetrieveBuildableProps() {
      return new List<PropsEnum>{PropsEnum.Gatherer, PropsEnum.BasicFighter, PropsEnum.FoodGenerator, PropsEnum.Storage};
   }
}