using System.Collections.Generic;
using Random = System.Random;

public enum ResourceEnum
{
    Water = 0,
    Food = 1,
    Iron = 2,
    Gold = 3,
    Platinum = 4
}

public static class OreResources {
    public static List<ResourceEnum> RetrieveOreResources() {
        return new List<ResourceEnum>() {ResourceEnum.Water, ResourceEnum.Iron, ResourceEnum.Gold, ResourceEnum.Platinum };
    }

    public static ResourceEnum GetRandomOreResource() {
        var rand = new Random();
        var resourceList = RetrieveOreResources();
        return resourceList[rand.Next(resourceList.Count)];
    }
}
