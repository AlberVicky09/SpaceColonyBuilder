using System.Collections.Generic;

public enum PropsEnum
{
   Gatherer,
   ImprovedGatherer,
   FoodGenerator,
   ImprovedFoodGenerator,
   BasicFighter,
   ImprovedFighter,
   BasicEnemy,
   ImprovedEnemy,
   House,
   ImprovedHouse,
   Storage,
   ImprovedStorage,
   MainBuilding,
   ImprovedMainBuilding
}

public static class BuildableProps {
   public static List<PropsEnum> RetrieveBuildableProps() {
      return new List<PropsEnum>{PropsEnum.Gatherer, PropsEnum.MainBuilding, PropsEnum.FoodGenerator, PropsEnum.BasicFighter};
   }
}
