using System.Collections.Generic;
using UnityEngine;

public static class Constants {
    
    public static Color WATER_COLOR = new (0, 0.42f, 1);
    public static Color IRON_COLOR = new (0.63f, 0.70f, 0.67f);
    public static Color GOLD_COLOR = new(1, 0.843f, 0);
    public static Color PLATINUM_COLOR = new(0.5f, 0.41f, 0.34f);
    public static Color MISSING_RESOURCE_COLOR = new(1f, 0.5f, 0);
    public static Color ENEMY_MASK_COLOR = new(1f, 0.3647798f, 0.3647798f);

    public static string LOSE_GAME_TEXT = "You have lost!";
    public static string WIN_GAME_TEXT = "Mission Completed!";

    public static string FILE_NOT_FOUND = "404: File not found";
    
    public static Dictionary<ResourceEnum, Color> ORE_COLOR_MAP = new Dictionary<ResourceEnum, Color> {
            { ResourceEnum.Water, WATER_COLOR },
            { ResourceEnum.Iron, IRON_COLOR },
            { ResourceEnum.Gold, GOLD_COLOR },
            { ResourceEnum.Platinum, PLATINUM_COLOR }
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

    public static Dictionary<FighterStatesEnum, string> FIGHTER_STATE_DISPLAY_NAME =
        new Dictionary<FighterStatesEnum, string>() {
            {FighterStatesEnum.Scouting, "Scouting base"},
            {FighterStatesEnum.Chasing, "Searching enemies"},
            {FighterStatesEnum.ChasingLowPriority, "Tracking enemy base"},
            {FighterStatesEnum.Attacking, "Attacking!"},
            {FighterStatesEnum.AttackingLowPriority, "Attacking base!"}
        };

    //TODO Check if its good
    public static readonly Dictionary<PropsEnum, Dictionary<ResourceEnum, int>> PROPS_MANTAINING_COST =
        new Dictionary<PropsEnum, Dictionary<ResourceEnum, int>> {
            {
                PropsEnum.MainBuilding,
                new Dictionary<ResourceEnum, int>()
            },{ //TODO DECIDE RESOURCES
                PropsEnum.Gatherer,
                new Dictionary<ResourceEnum, int>()
                    { { ResourceEnum.Iron, 5 }, { ResourceEnum.Gold, 10 }, { ResourceEnum.Platinum, 5 } }
            },
            {
                PropsEnum.FoodGenerator,
                new Dictionary<ResourceEnum, int>()
                    { { ResourceEnum.Iron, 0 }, { ResourceEnum.Gold, 0 }, { ResourceEnum.Platinum, 10 } }
            },
            {
                PropsEnum.Fighter,
                new Dictionary<ResourceEnum, int>()
                    { { ResourceEnum.Iron, 10 }, { ResourceEnum.Gold, 10 }, { ResourceEnum.Platinum, 0 } }
            },
            {
                PropsEnum.Storage,
                new Dictionary<ResourceEnum, int>()
                    { { ResourceEnum.Iron, 0 }, { ResourceEnum.Gold, 0 }, { ResourceEnum.Platinum, 0 } }
            }
        };

    public static readonly Dictionary<PropsEnum, Dictionary<ResourceEnum, int>> PROP_CREATION_PRICES =
        new Dictionary<PropsEnum, Dictionary<ResourceEnum, int>> {
            { //TODO DECIDE RESOURCES
                PropsEnum.Gatherer,
                new Dictionary<ResourceEnum, int>()
                    { { ResourceEnum.Iron, 25 }, { ResourceEnum.Gold, 50 }, { ResourceEnum.Platinum, 25 } }
            },
            {
                PropsEnum.FoodGenerator,
                new Dictionary<ResourceEnum, int>()
                    { { ResourceEnum.Iron, 25 }, { ResourceEnum.Gold, 50 }, { ResourceEnum.Platinum, 25 } }
            },
            {
                PropsEnum.Fighter,
                new Dictionary<ResourceEnum, int>()
                    { { ResourceEnum.Iron, 25 }, { ResourceEnum.Gold, 50 }, { ResourceEnum.Platinum, 25 } }
            },
            {
                PropsEnum.Storage,
                new Dictionary<ResourceEnum, int>()
                    { { ResourceEnum.Iron, 25 }, { ResourceEnum.Gold, 50 }, { ResourceEnum.Platinum, 25 } }
            },
            { 
                PropsEnum.EnemyGatherer,
                new Dictionary<ResourceEnum, int>()
                    { { ResourceEnum.Iron, 25 }, { ResourceEnum.Gold, 50 }, { ResourceEnum.Platinum, 25 } }
            },
            { 
                PropsEnum.EnemyFighter,
                new Dictionary<ResourceEnum, int>()
                    { { ResourceEnum.Iron, 25 }, { ResourceEnum.Gold, 50 }, { ResourceEnum.Platinum, 25 } }
            },
        };

    public static readonly Dictionary<PropsEnum, List<ResourceEnum>> ENEMY_RESOURCE_PREFFERENCE =
        new Dictionary<PropsEnum, List<ResourceEnum>> {
            {
                //TODO Put the resources that the producing doesnt use
                PropsEnum.EnemyGatherer,
                new List<ResourceEnum>(new [] { ResourceEnum.Gold, ResourceEnum.Iron, ResourceEnum.Platinum})
            },{ 
                PropsEnum.EnemyFighter,
                new List<ResourceEnum>(new [] { ResourceEnum.Gold, ResourceEnum.Iron, ResourceEnum.Platinum})
            },
        };

    public static readonly Dictionary<PropsEnum, string> PROPS_SUMMARY_NAME =
        new Dictionary<PropsEnum, string>() {
            { PropsEnum.Gatherer, "Gathering ship" }, { PropsEnum.Fighter, "Fighting ship" },
            { PropsEnum.FoodGenerator, "Food generator" }, { PropsEnum.Storage, "Resource Storage" }
        };
    
    public const float ORE_GENERATION_DISTANCE_RANGE = 30f;
    public const float VIEW_DISTANCE_RANGE = 38f;
    public const int INITIAL_ORE_NUMBER = 15;
    public const float ORE_FLOOR_OFFSET = 0.4f;
    public static Vector3 BULLET_OFFSET = new Vector3(0f, 0.25f, 0f);

    public const float FOOD_GENERATOR_DURATION = 5.0f;
    
    public const float SHOOTING_RELOAD_TIME = 3.5f;

    public const float MAX_DOUBLE_CLICK_DELAY = 1f;
    
    public const float GAMEOBJECT_CENTERED_MAX_REFRESH_TIME = 1f;

    public const float CAMERA_OFFSET_X = -9.3f;
    public const float CAMERA_OFFSET_Y = 7.25f;

    public const float ZOOM_CHANGE = 12;
    public const float CAMERA_SMOOTHER_VALUE = 1.3f;
    public const float MIN_ZOOM_SIZE = 4f;
    public const float MAX_ZOOM_SIZE = 13f;
    public const float MIN_MINIMAP_ZOOM_SIZE = 13f;
    public const float MAX_MINIMAP_ZOOM_SIZE = 19f;

    public const float MIN_X_WORLD_POSITION = -100f;
    public const float MAX_X_WORLD_POSITION = 100f;
    public const float MIN_Y_WORLD_POSITION = -100f;
    public const float MAX_Y_WORLD_POSITION = 100f;

    public const float GATHERER_ACTION_OFFSET = 1.5f;
    
    public static Vector3 RESET_CAMERA_POSITION = new Vector3(-9.3f, 7.25f, 0f);
    public static float BASE_RETREAT_OFFSET = 6.5f;

    public static float MENU_ITEM_WIGGLE_DISPLACEMENT = 5f;
    public static float MENU_ITEM_WIGGLE_SPEED = 4f;

    public static readonly Dictionary<SpeedLevels, float> SPEED_LEVEL_EQUIVALENCE =
        new Dictionary<SpeedLevels, float> {
            { SpeedLevels.NORMAL, 1f},
            { SpeedLevels.SLOW, 0.5f},
            { SpeedLevels.FAST, 1.5f},
            { SpeedLevels.STOPPED, 0f},
        };

    public static int INITIAL_RESOURCES_QUANTITY = 500; //TODO Fix this number
    public static int INITIAL_RESOURCES_LIMIT = 100;
    public static int ENEMY_INITIAL_RESOURCES_LIMIT = 250;
    public static int RESOURCES_LIMIT_INCREASE = 50;
    
    public static int GATHERER_GATHERING_QUANTITY = 15;
    public static int DEFAULT_GATHERER_MAX_LOAD = 75;
    public static int UPGRADED_GATHERER_LOAD = 175;

    public static float RESOURCE_CHANGE_MOVEMENT_TIME = 1.5f;
    public static float RESOURCE_CHANGE_MOVEMENT_LERP_TIME = 0.05f;
    public static Vector3 RESOURCE_CHANGE_DISPLACE = new Vector3(0, 30f, 0);
    public static Color GREEN_COLOR = new Color(0.4947595f, 1, 0.3176471f);
    public static Color RED_COLOR = new Color(1, 0.3539989f, 0.31761f);

    public static float PLACING_REQUIRED_DELAY = 0.3f;

    public static float MAX_BULLET_TRAVEL_DISTANCE = 12f;

    public static float MIN_ENEMY_SPAWNING_TIME = 120f;
    public static float MAX_ENEMY_SPAWNING_TIME = 240f;

    public static int DEFAULT_MISSING_RESOURCE_VALUE = 0;

    public static List<int> RESOLUTIONS_VALID_HEIGHTS = new List<int>() {600, 720, 900, 1080};
    public static List<int> RESOLUTIONS_VALID_WIDTHS = new List<int>() {800, 1280, 1400, 1920};
    
    public const float WAYPOINTS_RADIUS = 18f;
    public const int numberOfWaypoints = 8;
    
    public static Vector3 ENEMY_CENTER = new (75f, 0, 0);

    public static Vector3 INITIAL_SUMMARY_ITEM_POSITION = new(-45, -340, 1);
    public static float SUMMARY_OFFSET = -120f;

    public static Vector3 INTERACTABLE_BUTTON_PROP_SCALE = new(2.5f, 2.5f, 1);
    public static Vector3 INTERACTABLE_BUTTON_RESOURCE_SCALE = new(1f, 1f, 1f);

    public static int TUTORIAL_MISSION_0_MAX = 8;
    public static int TUTORIAL_MISSION_1_MAX = 10;
    public static string VIDEO_TUTORIAL_CLIP_NAME = "TutorialClip";
    public static List<string> TUTORIAL_TEXTS = new List<string>() {
        //MISSION 1
        "Welcome to Space Colony Builder!.\nYour objective in this game is to complete the missions within 5 (in-game) years and without loosing all health in your base.",
        "This is your base. It has a health bar with 100 health points, if you loose all of them, your mission will fail! Protect it and spread around to win.",
        "On the top left of your screen, its the list of your resources, being in order water, food, iron, gold and platinum. The base limit is " + INITIAL_RESOURCES_LIMIT + ", but wou will be able to increase it once we are better established.",
        "On the top right is the calendar panel. On it, you can check the current in-game date and find the buttons to control time. You can pause, play or put it in fast forward, as desired.",
        "Under it, you can check at any moment the missions you need to complete to win the level. Once they are completed, they will turn green.",
        "If you select the base, you can generate other ships to help you complete your missions, which will cost specific resources. Besides that, you can heal the base (in case you loose some health).",
        "This is a gatherer. It can retrieve resources from the ores spread along the galaxy. You can select the desired resource, and it will automatically search for the nearest of that type.",
        "This is a food generator. With it, you can transform water into food (which cannot be obtained from any ore). It can be started and paused at will.",
        //MISSION 2
        "This, are the fighters. This ships will patrol around the base and will defend it from the pirates, but beware, because they can be destroyed if they suffer too much",
        "Along with them, you can now create storages, which will increase the maximum of each resource that you can store. You will need some to be able to create fighters!",
        //MISSION 3
        "The nearby base, Phobos III, has been established nearby for some years now. Lately, they have shown some aggressive behaviour, and now its our moment to counter-attack. You must destroy their base before they defeat us!",
        "Now, your fighter ships will have an option to attack enemy base. Select it, and they will go towards it. While its not fighting, you can make it go back to patrolling mode."
    };
}
