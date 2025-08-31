using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameControllerScript : MonoBehaviour {

    #region Variables

    public static GameControllerScript Instance;
    public UIUpdateController uiUpdateController; 
    public InteractableButtonManager interactableButtonManager;
    public MissionController missionController;
    public CameraMove cameraMove;
    public BulletPoolController bulletPoolController;
    public EnemyGenerationController enemyGenerationController;
    public TutorialControllerLegacy tutorialControllerLegacy;
    public TutorialControllerNew tutorialControllerNew;
    public EnemyBaseController enemyBaseController;
    
    public CanvasGroup canvasGroup;

    public GameObject orePrefab;
    public GameObject gathererPrefab;
    public GameObject foodGeneratorPrefab;
    public GameObject housePrefab;
    public GameObject storagePrefab;
    public GameObject fighterPrefab;
    public GameObject floorPrefab;
    public GameObject enemyBasePrefab;
    public GameObject enemyFighterPrefab;
    public GameObject enemyGathererPrefab;

    public Sprite oreSprite, gathererSprite, foodGeneratorSprite, storageSprite, fighterSprite;
    public Sprite waterSprite, foodSprite, ironSprite, goldSprite, platinumSprite;
    public Sprite missingWaterSprite, missingFoodSprite, missingIronSprite, missingGoldSprite, missingPlatinumSprite;

    public GameObject mainBuilding, startingGatherer;
    public Dictionary<PropsEnum, List<GameObject>> propDictionary;
    public Dictionary<PropsEnum, Sprite> propSpriteDictionary;
    public Dictionary<ResourceEnum, Sprite> resourceSpriteDictionary;
    public Dictionary<ResourceEnum, Sprite> missingResourceSpriteDictionary;
    
    public Dictionary<ResourceEnum, List<ResourceTuple>> oreListDictionary;
    public Dictionary<ResourceEnum, Image> oreListImage;
    public Sprite oreImage;

    public Sprite generatePropsSprite,
        healBaseSprite,
        selectResourceSprite,
        returningToBaseSprite,
        patrolBaseSprite,
        attackSprite,
        startActionSprite,
        stopActionSprite;
    public int numberOfOres;

    public GameObject actionCanvas;
    public GameObject[] actionButtons;
    public TMP_Text actionText;
    public Image uiRepresentation;
    public Dictionary<ResourceEnum, TMP_Text> uiResourcesTextMap;
    public List<TMP_Text> uiMaxResourcesList;
    public Dictionary<ResourceEnum, (TMP_Text text, CanvasRenderer canvas)> uiResourcesChangeTextMap;

    public TMP_Text enemyCountDownText;
    public Image enemyCountDownBg;

    public GameObject alertCanvas;
    public TMP_Text alertCanvasText;
    private float prevAlertTimeSpeed;
    public Sprite blueLabelSprite, redLabelSprite, greenLabelSprite;
    public bool isGamePaused, wasGamePaused, isPauseMenuActive, isInAMenu;
    
    public List<Vector3> waypoints;
    
    public int currentMissionNumber;

    public Dictionary<ResourceEnum, int> resourcesDictionary;
    public int resourcesLimit = Constants.INITIAL_RESOURCES_LIMIT;

    public GameObject toolTipCanvas, toolTipResourceCanvas;
    public RectTransform toolTipCanvasRect, toolTipResourceCanvasRect;
    public TMP_Text toolTipText;
    public List<TMP_Text> toolTipResourceText;

    public bool placing;
    #endregion

    void Awake() {
        Instance = this;
        
        //Initialize ui text resource counters
        uiResourcesTextMap = new Dictionary<ResourceEnum, TMP_Text> {
            { ResourceEnum.Water, GameObject.Find("WaterCounter").GetComponent<TMP_Text>() },
            { ResourceEnum.Food, GameObject.Find("FoodCounter").GetComponent<TMP_Text>() },
            { ResourceEnum.Iron, GameObject.Find("IronCounter").GetComponent<TMP_Text>() },
            { ResourceEnum.Gold, GameObject.Find("GoldCounter").GetComponent<TMP_Text>() },
            { ResourceEnum.Platinum, GameObject.Find("PlatinumCounter").GetComponent<TMP_Text>() }
        };

        uiMaxResourcesList = new List<TMP_Text> {
            GameObject.Find("MaxWaterCounter").GetComponent<TMP_Text>(),
            GameObject.Find("MaxFoodCounter").GetComponent<TMP_Text>(),
            GameObject.Find("MaxIronCounter").GetComponent<TMP_Text>(),
            GameObject.Find("MaxGoldCounter").GetComponent<TMP_Text>(),
            GameObject.Find("MaxPlatinumCounter").GetComponent<TMP_Text>()
        };

        var waterChangeGO = GameObject.Find("WaterChangeCounter");
        var foodChangeGO = GameObject.Find("FoodChangeCounter");
        var ironChangeGO = GameObject.Find("IronChangeCounter");
        var goldChangeGO = GameObject.Find("GoldChangeCounter");
        var platinumChangeGO = GameObject.Find("PlatinumChangeCounter");
        uiResourcesChangeTextMap = new Dictionary<ResourceEnum, ValueTuple<TMP_Text, CanvasRenderer>> {
            { ResourceEnum.Water, new ValueTuple<TMP_Text, CanvasRenderer>(waterChangeGO.GetComponent<TMP_Text>(), waterChangeGO.GetComponent<CanvasRenderer>()) },
            { ResourceEnum.Food, new ValueTuple<TMP_Text, CanvasRenderer>(foodChangeGO.GetComponent<TMP_Text>(), foodChangeGO.GetComponent<CanvasRenderer>()) },
            { ResourceEnum.Iron, new ValueTuple<TMP_Text, CanvasRenderer>(ironChangeGO.GetComponent<TMP_Text>(), ironChangeGO.GetComponent<CanvasRenderer>()) },
            { ResourceEnum.Gold, new ValueTuple<TMP_Text, CanvasRenderer>(goldChangeGO.GetComponent<TMP_Text>(), goldChangeGO.GetComponent<CanvasRenderer>()) },
            { ResourceEnum.Platinum, new ValueTuple<TMP_Text, CanvasRenderer>(platinumChangeGO.GetComponent<TMP_Text>(), platinumChangeGO.GetComponent<CanvasRenderer>()) }
        };

        propSpriteDictionary = new Dictionary<PropsEnum, Sprite>() {
            { PropsEnum.Gatherer, gathererSprite },
            { PropsEnum.BasicFighter, fighterSprite },
            { PropsEnum.FoodGenerator, foodGeneratorSprite },
            { PropsEnum.Storage, storageSprite }
        };
        resourceSpriteDictionary = new Dictionary<ResourceEnum, Sprite>() {
            { ResourceEnum.Water, waterSprite },
            { ResourceEnum.Food, foodSprite },
            { ResourceEnum.Iron, ironSprite },
            { ResourceEnum.Gold, goldSprite },
            { ResourceEnum.Platinum, platinumSprite }
        };
        missingResourceSpriteDictionary = new Dictionary<ResourceEnum, Sprite>() {
            { ResourceEnum.Water, missingWaterSprite },
            { ResourceEnum.Food, missingFoodSprite },
            { ResourceEnum.Iron, missingIronSprite },
            { ResourceEnum.Gold, missingGoldSprite },
            { ResourceEnum.Platinum, missingPlatinumSprite }
        };
            
        //Initialize resources dictionaries
        oreListDictionary = new Dictionary<ResourceEnum, List<ResourceTuple>>();
        resourcesDictionary = new Dictionary<ResourceEnum, int>();
        uiUpdateController.resourcesInitialPositions = new Dictionary<ResourceEnum, Vector3>();
        foreach (ResourceEnum resource in Enum.GetValues(typeof(ResourceEnum))) {
            oreListDictionary.Add(resource, new List<ResourceTuple>());
            resourcesDictionary.Add(resource, Constants.INITIAL_RESOURCES_QUANTITY);
            uiUpdateController.resourcesInitialPositions.Add(resource, uiResourcesChangeTextMap[resource].text.transform.position);
            //Hide resources loss text
            uiResourcesChangeTextMap[resource].canvas.SetAlpha(0f);
        }
        uiUpdateController.SetResourcesText();

        //Initialize ore images 
        oreListImage = new Dictionary<ResourceEnum, Image>();
        foreach (ResourceEnum resource in Enum.GetValues(typeof(ResourceEnum))) {
            if (resource != ResourceEnum.Food) {
                var oreImageGO = new GameObject("GeneratedImage", typeof(Image));
                var oreImageComponent = oreImageGO.GetComponent<Image>();
                oreImageComponent.sprite = oreImage;
                oreImageComponent.color = Constants.ORE_COLOR_MAP[resource];
                oreListImage.Add(resource, oreImageComponent);
            }
        }
        
        //Initialize prop dictionary
        propDictionary = new Dictionary<PropsEnum, List<GameObject>>();
        foreach (PropsEnum propType in Enum.GetValues(typeof(PropsEnum))) {
            propDictionary.Add(propType, new List<GameObject>());
        }
        //Manually add props to list
        propDictionary[PropsEnum.MainBuilding].Add(mainBuilding);
        propDictionary[PropsEnum.Gatherer].Add(startingGatherer);
        
        //Manually initialize startingGatherer
        startingGatherer.GetComponent<Placeable>().OnPropPlaced();
        
        //Calculate waypoints for patrolling
        waypoints = Utils.CalculateWaypointsForBuilding(transform.position, Constants.numberOfWaypoints, Constants.WAYPOINTS_RADIUS);
        
        Utils.GenerateRandomOres(mainBuilding.transform.position);
    }

    void Start() {
        //Different behaviour depending on the level you are on
        currentMissionNumber = PlayerPrefs.GetInt("mission", 0);
        //TODO Just for debugging
        //currentMissionNumber = 2;
        
        switch (currentMissionNumber) {
            case 0:
                enemyCountDownText.text = "No enemies in sight";
                enemyCountDownBg.sprite = greenLabelSprite;
                //TODO Display tutorial screen
                break;
            case 1:
                StartCoroutine(GenerateEnemyShipsCoroutine());
                break;
            case 2:
                enemyBaseController.ActivateEnemyBase();
                enemyCountDownBg.gameObject.SetActive(false);
                break;
        }
        
        PauseGame();
    }

    public void CalculateOreForGatherer(GameObject oreGatherer) {
        //Finds nearest ore of specified type
        var gathererBehaviour = oreGatherer.GetComponent<GathererBehaviour>();
        var nearestOre = Utils.FindNearestGameObjectInList(oreGatherer, oreListDictionary[gathererBehaviour.resourceGatheringType]);

        if (nearestOre is not null) {
            gathererBehaviour.objectiveItem = nearestOre;
            gathererBehaviour.UpdateDestination();
            gathererBehaviour.DisplayAction(resourceSpriteDictionary[gathererBehaviour.resourceGatheringType]);
        } else {
            gathererBehaviour.DisplayAction(missingResourceSpriteDictionary[gathererBehaviour.resourceGatheringType]);
        }
    }

    public void ActivateAlertCanvas(string alertDisplayText) {
        //Display alert canvas with specified text and stop time
        alertCanvas.SetActive(true);
        alertCanvasText.text = alertDisplayText;
        prevAlertTimeSpeed = Time.timeScale;
        PauseGame();
    }

    public void CloseAlertCanvas() {
        //Close alert panel and restart time
        alertCanvas.SetActive(false);
        if (prevAlertTimeSpeed != 0) { 
            PlayVelocity(Constants.TIME_SCALE_STOPPED);
        }
    }

    public void PlayVelocity(float velocity) {
        //Sets game velocity, to be able to play normal, fast or slow speed
        isGamePaused = false;
        Time.timeScale = velocity;
    }

    public void PauseGame() {
        wasGamePaused = isGamePaused;
        isGamePaused = true;
        Time.timeScale = Constants.TIME_SCALE_STOPPED;
    }
    
    public void TogglePauseMenu() { isPauseMenuActive = !isPauseMenuActive;}

    public void SwapUIInteraction() {
        canvasGroup.interactable = !canvasGroup.interactable;
        canvasGroup.blocksRaycasts = !canvasGroup.blocksRaycasts;
    }

    private IEnumerator GenerateEnemyShipsCoroutine() {
        var remainingTime = Random.Range(Constants.MIN_ENEMY_SPAWNING_TIME, Constants.MAX_ENEMY_SPAWNING_TIME);
        while (true) {
            //Spawn enemies in remainingTime seconds
            while (remainingTime > 0) {
                remainingTime--;
                yield return new WaitForSeconds(1f);
                
                var remainingMinutes = Mathf.Floor(remainingTime / 60f);
                var remainingMinutesString = remainingMinutes < 10f ? "0" + remainingMinutes : remainingMinutes.ToString(CultureInfo.CurrentCulture);
                var remainingSeconds = Mathf.Floor(remainingTime - remainingMinutes * 60);
                String remainingSecondsString = remainingSeconds < 10f ? "0" + remainingSeconds : remainingSeconds.ToString(CultureInfo.CurrentCulture);
                enemyCountDownText.text = $"Enemy in {remainingMinutesString}:{remainingSecondsString}";
            }
            
            //Generate between 2-4 enemy ships
            enemyGenerationController.GenerateNewEnemy(Random.Range(2, 5));
            remainingTime = Random.Range(Constants.MIN_ENEMY_SPAWNING_TIME, Constants.MAX_ENEMY_SPAWNING_TIME);
        }
    }
}
