using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public static List<ResourceEnum> ORE_RESOURCES = new List<ResourceEnum>() {ResourceEnum.Water, ResourceEnum.Iron, ResourceEnum.Gold, ResourceEnum.Platinum };
    public static Dictionary<ResourceEnum, Color> ORE_COLOR_MAP = new Dictionary<ResourceEnum, Color> {
            { ResourceEnum.Water, new Color(0,0.42f,1) },
            { ResourceEnum.Iron, new Color(0.63f, 0.70f, 0.67f) },
            { ResourceEnum.Gold, new Color(1, 0.843f, 0) },
            { ResourceEnum.Platinum, new Color(0.5f, 0.41f, 0.34f) }
    };
    public static Dictionary<ResourceEnum, float> ORE_METALLIC_MAP = new Dictionary<ResourceEnum, float> {
            { ResourceEnum.Water, 0.17f },
            { ResourceEnum.Iron, 0.37f },
            { ResourceEnum.Gold, 0.61f },
            { ResourceEnum.Platinum, 0.58f }
    };
    public static Dictionary<ResourceEnum, float> ORE_SMOOTHNESS_MAP = new Dictionary<ResourceEnum, float> {
            { ResourceEnum.Water, 1f },
            { ResourceEnum.Iron, 0.81f },
            { ResourceEnum.Gold, 0.79f },
            { ResourceEnum.Platinum, 1f }
    };

    //Calendar Map
    public static Dictionary<int, string> CALENDAR_MAP = new Dictionary<int, string> {
        { 1, "Jan" },
        { 2, "Feb" },
        { 3, "Mar" },
        { 4, "Apr" },
        { 5, "May" },
        { 6, "Jun" },
        { 7, "Jul" },
        { 8, "Aug" },
        { 9, "Sep" },
        { 10, "Oct" },
        { 11, "Nov" },
        { 12, "Dec" },
    };

    public static readonly Dictionary<ResourceEnum, int> MAIN_BUILDING_MANTAINING_COST = new Dictionary<ResourceEnum, int>
        { {ResourceEnum.Food, 3 },  {ResourceEnum.Water, 5} };
    public static readonly Dictionary<ResourceEnum, int> ORE_GATHERER_COST = new Dictionary<ResourceEnum, int>
        { {ResourceEnum.Platinum, 5}, {ResourceEnum.Iron, 5} };


    public const float VIEW_DISTANCE_RANGE = 24f;
    public const int INITIAL_ORE_NUMBER = 10;
    public const float ORE_FLOOR_OFFSET = 0.4f;

    public const float MAX_DOUBLE_CLICK_DELAY = 1f;

    public const int MAX_GATHERING_TIMES = 3;//25;

    public const float GAMEOBJECT_CENTERED_MAX_REFRESH_TIME = 1f;

    public const float CAMERA_OFFSET_X = -9.3f;
    public const float CAMERA_OFFSET_Y = 7.25f;

    public const float ZOOM_CHANGE = 12;
    public const float CAMERA_SMOOTHER_VALUE = 1.3f;
    public const float MIN_ZOOM_SIZE = 4f;
    public const float MAX_ZOOM_SIZE = 13f;

    public const float GATHERER_ACTION_OFFSET = 1.5f;

    public static List<int> INITIAL_GATHERER_PRICE = new List<int>() { 25, 50, 25};

    public static Vector3 RESET_CAMERA_POSITION = new Vector3(-9.3f, 7.25f, 0f);
    public static Vector3 BASE_RETREAT_OFFSET = new Vector3(10f, 0f, 10f);

    public static float MENU_ITEM_WIGGLE_DISPLACEMENT = 5f;
    public static float MENU_ITEM_WIGGLE_SPEED = 4f;

    public const float TIME_SCALE_NORMAL = 1f;
    public const float TIME_SCALE_SLOW = 0.5f;
    public const float TIME_SCALE_FAST = 1.5f;
    public static float TIME_SCALE_STOPPED = 0f;

    public static float RESOURCE_LOSS_MOVEMENT_TIME = 1.5f;
    public static float RESOURCE_LOSS_MOVEMENT_LERP_TIME = 0.05f;
    public static Vector3 RESOURCE_LOSS_DISPLACE = new Vector3(0, 30f, 0);
}
