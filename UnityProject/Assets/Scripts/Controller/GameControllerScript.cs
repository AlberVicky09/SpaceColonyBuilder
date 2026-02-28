using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameControllerScript : MonoBehaviour {

    #region Variables

    public static GameControllerScript Instance { get; private set; }
    public UIUpdateController uiUpdateController; 
    public InteractableButtonManager interactableButtonManager;
    public MissionController missionController;
    public CameraMove cameraMove;
    public BulletPoolController bulletPoolController;
    public EnemyGenerationController enemyGenerationController;
    public TutorialControllerLegacy tutorialControllerLegacy;
    public TutorialControllerImage tutorialControllerImage;
    public ControlsController controlsController;
    public EnemyBaseController enemyBaseController;
    public PauseMenuController pauseMenuController;
    public SummaryPanelController summaryPanelController;
    
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
    public Sprite missingGathererSprite, missingFoodGeneratorSprite, missingStorageSprite, missingFighterSprite;
    public Sprite waterSprite, foodSprite, ironSprite, goldSprite, platinumSprite;
    public Sprite missingWaterSprite, missingFoodSprite, missingIronSprite, missingGoldSprite, missingPlatinumSprite;

    public GameObject mainBuilding, startingGatherer;
    public Dictionary<PropsEnum, List<GameObject>> propDictionary;
    public Dictionary<PropsEnum, Sprite> propSpriteDictionary;
    public Dictionary<PropsEnum, Sprite> missingPropSpriteDictionary;
    public Dictionary<ResourceEnum, Sprite> resourceSpriteDictionary;
    public Dictionary<ResourceEnum, Sprite> missingResourceSpriteDictionary;
    public Dictionary<FighterStatesEnum, Sprite> fighterActionsDictionary;
    public Dictionary<FighterStatesEnum, Sprite> enemyFighterActionsDictionary;
    
    public Dictionary<ResourceEnum, List<ResourceTuple>> oreListDictionary;
    public Dictionary<ResourceEnum, Image> oreListImage;
    public Sprite oreImage;

    public Sprite generatePropsSprite,
        healBaseSprite,
        selectResourceSprite,
        returningToBaseSprite,
        patrolBaseSprite,
        enemyPatrolBaseSprite,
        chasingEnemySprite,
        enemyChasingShipSprite,
        chasingBaseSprite,
        enemyChasingBaseSprite,
        attackSprite,
        enemyAttackSprite,
        startActionSprite,
        stopActionSprite;
    public int numberOfOres;

    public GameObject actionCanvas;
    public GameObject[] actionButtons;
    public TMP_Text actionText;
    public Image uiRepresentation;
    public Dictionary<ResourceEnum, TMP_Text> uiResourcesTextMap;
    public List<TMP_Text> uiMaxResourcesList;
    public Dictionary<ResourceEnum, TMP_Text> uiResourcesChangeTextMap;

    public TMP_Text enemyCountDownText;
    public Image enemyCountDownBg;

    public GameObject missionBigCanvas, missionSmallCanvas;
    public RectTransform alertCanvas;
    public TMP_Text alertText;
    private float prevAlertTimeSpeed;
    public Sprite blueLabelSprite, redLabelSprite, greenLabelSprite;
    public bool isGamePaused, wasGamePaused, isPauseMenuActive, isInMissions, isInAMenu, isInAlert, isInSummary, isGameFinished;
    
    public List<Vector3> waypoints;

    public int currentMissionNumber, isTutorialActivated;

    public Dictionary<ResourceEnum, int> resourcesDictionary;
    public int resourcesLimit = Constants.INITIAL_RESOURCES_LIMIT;

    public RectTransform toolTipGO, toolTipResourceGO;
    public Canvas toolTipCanvas;
    public TMP_Text toolTipText;
    public List<TMP_Text> toolTipResourceText;

    public bool placing;
    #endregion

    void Awake() {
        Instance = this;
        
        //Different behaviour depending on the level you are on
        currentMissionNumber = PlayerPrefs.GetInt("mission", 0);
        isTutorialActivated = PlayerPrefs.GetInt("tutorialActivated", 0);
        
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
        uiResourcesChangeTextMap = new Dictionary<ResourceEnum, TMP_Text> {
            { ResourceEnum.Water, waterChangeGO.GetComponent<TMP_Text>() },
            { ResourceEnum.Food, foodChangeGO.GetComponent<TMP_Text>() },
            { ResourceEnum.Iron, ironChangeGO.GetComponent<TMP_Text>() },
            { ResourceEnum.Gold, goldChangeGO.GetComponent<TMP_Text>() },
            { ResourceEnum.Platinum, platinumChangeGO.GetComponent<TMP_Text>() }
        };
        foreach (var resourceText in uiResourcesChangeTextMap.Values) {
            resourceText.alpha = 0f;
        }

        propSpriteDictionary = new Dictionary<PropsEnum, Sprite>() {
            { PropsEnum.Gatherer, gathererSprite },
            { PropsEnum.Fighter, fighterSprite },
            { PropsEnum.FoodGenerator, foodGeneratorSprite },
            { PropsEnum.Storage, storageSprite }
        };

        missingPropSpriteDictionary = new Dictionary<PropsEnum, Sprite>() {
            { PropsEnum.Gatherer, missingGathererSprite },
            { PropsEnum.Fighter, missingFighterSprite },
            { PropsEnum.FoodGenerator, missingFoodGeneratorSprite },
            { PropsEnum.Storage, missingStorageSprite }
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
        fighterActionsDictionary = new Dictionary<FighterStatesEnum, Sprite>() {
            { FighterStatesEnum.Scouting, patrolBaseSprite },
            { FighterStatesEnum.Chasing, chasingEnemySprite },
            { FighterStatesEnum.ChasingLowPriority, chasingBaseSprite },
            { FighterStatesEnum.Attacking, attackSprite },
            { FighterStatesEnum.AttackingLowPriority, attackSprite },
        };
        enemyFighterActionsDictionary = new Dictionary<FighterStatesEnum, Sprite>() {
            { FighterStatesEnum.Scouting, enemyPatrolBaseSprite },
            { FighterStatesEnum.Chasing, enemyChasingShipSprite },
            { FighterStatesEnum.ChasingLowPriority, enemyChasingBaseSprite },
            { FighterStatesEnum.Attacking, enemyAttackSprite },
            { FighterStatesEnum.AttackingLowPriority, enemyAttackSprite },
        };
            
        //Initialize resources dictionaries
        oreListDictionary = new Dictionary<ResourceEnum, List<ResourceTuple>>();
        resourcesDictionary = new Dictionary<ResourceEnum, int>();
        uiUpdateController.resourcesInitialPositions = new Dictionary<ResourceEnum, Vector3>();
        foreach (ResourceEnum resource in Enum.GetValues(typeof(ResourceEnum))) {
            oreListDictionary.Add(resource, new List<ResourceTuple>());
            resourcesDictionary.Add(resource, Constants.INITIAL_RESOURCES_QUANTITY_MAP[resource]);
            uiUpdateController.resourcesInitialPositions.Add(resource, uiResourcesChangeTextMap[resource].transform.position);
        }
        //Set resources and max on UI
        foreach (var maxResourceText in uiMaxResourcesList) { maxResourceText.text = resourcesLimit.ToString(); }
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
        AudioManager.Instance.SetMusic(MusicTrackNamesEnum.MainBG);

        switch (currentMissionNumber) {
            case 0:
                enemyCountDownText.text = "No enemies in sight";
                enemyCountDownBg.sprite = greenLabelSprite;
                break;
            case 1:
                StartCoroutine(GenerateEnemyShipsCoroutine());
                break;
            case 2:
                enemyBaseController.ActivateEnemyBase();
                enemyCountDownBg.gameObject.SetActive(false);
                break;
        }

        if (isTutorialActivated == 1) { tutorialControllerImage.DisplayTutorialForMission(); }
        
        PauseGame();
    }

    public OreFindingcases CalculateOreForGatherer(GameObject oreGatherer) {
        
        var gathererBehaviour = oreGatherer.GetComponent<GathererBehaviour>();
        
        //Check if resources are at maximum
        if (resourcesDictionary[gathererBehaviour.resourceGatheringType] == resourcesLimit) {
            gathererBehaviour.DisplayAction(missingResourceSpriteDictionary[gathererBehaviour.resourceGatheringType]);
            return OreFindingcases.ResourceAtMax;
        }

        GameObject nearestOre = null;
        //Check if previous ore is still available
        if (gathererBehaviour.previousGatheredOre != null) {
            nearestOre = Utils.FindSpecificGameObjectInList(gathererBehaviour.previousGatheredOre,
                oreListDictionary[gathererBehaviour.resourceGatheringType]);
            Debug.Log("Ore still available");
        }
        
        if(nearestOre == null) {
            //Finds nearest ore of specified type
            nearestOre = Utils.FindNearestGameObjectInList(oreGatherer,
                oreListDictionary[gathererBehaviour.resourceGatheringType]);
        }
        
        if (nearestOre != null) {
            gathererBehaviour.objectiveItem = nearestOre;
            gathererBehaviour.UpdateDestination();
            gathererBehaviour.DisplayAction(resourceSpriteDictionary[gathererBehaviour.resourceGatheringType]);
            return OreFindingcases.AvailableOre;
        }
        
        gathererBehaviour.DisplayAction(missingResourceSpriteDictionary[gathererBehaviour.resourceGatheringType]);
        gathererBehaviour.ReturnToBase(true);
        return OreFindingcases.NoAvailableOres;
    }

    public void ActivateAlertCanvas(string alertDisplayText) {
        //Display alert canvas with specified text and stop time
        alertCanvas.gameObject.SetActive(true);
        alertText.text = alertDisplayText;
        isInAlert = true;
        //Force canvas width update
        LayoutRebuilder.ForceRebuildLayoutImmediate(alertCanvas);
        PauseGame();
    }

    public void CloseAlertCanvas() {
        alertCanvas.gameObject.SetActive(false);
        isInAlert = false;
    }

    public void CloseMissionCanvas() {
        missionBigCanvas.SetActive(false);
        missionSmallCanvas.SetActive(true);
        missionController.missionUIList.ToList().ForEach(item =>
            LayoutRebuilder.ForceRebuildLayoutImmediate(item.GetComponent<RectTransform>()));
        StartCoroutine(DatePanelController.Instance.StartDayCicle());
        if (currentMissionNumber == 2) {
            StartCoroutine(cameraMove.StartTravellingToEnemyBase());
        } else {
            isInMissions = false;
            PlayNormalVelocity();
        }
    }

    public void PlayNormalVelocity() {
        Clickable.selectedClickable = null;
        PlayVelocity(SpeedLevels.NORMAL);
    }

    public void PlayFastVelocity() { PlayVelocity(SpeedLevels.FAST); }

    public void RevertToPreviousVelocity() {
        if (DatePanelController.Instance.prevSpeed.Equals(SpeedLevels.STOPPED)) {
            PauseGame();
        } else {
            PlayVelocity(DatePanelController.Instance.prevSpeed);
        }
    }
    
    public void PlayVelocity(SpeedLevels velocity) {
        //Sets game velocity, to be able to play normal, fast or slow speed
        isGamePaused = false;
        Time.timeScale = Constants.SPEED_LEVEL_EQUIVALENCE[velocity];
        DatePanelController.Instance.SwapButtons(velocity);
    }

    public void PauseGame() {
        wasGamePaused = isGamePaused;
        isGamePaused = true;
        DatePanelController.Instance.SwapButtons(SpeedLevels.STOPPED);
        Time.timeScale = Constants.SPEED_LEVEL_EQUIVALENCE[SpeedLevels.STOPPED];
    }
    
    public void TogglePauseMenu() { isPauseMenuActive = !isPauseMenuActive;}

    public void SwapUIInteraction() {
        canvasGroup.interactable = !canvasGroup.interactable;
        canvasGroup.blocksRaycasts = !canvasGroup.blocksRaycasts;
    }
    
    public void CloseActionPanel() {
        Clickable.selectedClickable = null;
        actionCanvas.SetActive(false);
        CameraMove.Instance.UnFocusCameraInGO();
    }

    private IEnumerator GenerateEnemyShipsCoroutine() {
        //var remainingTime = Random.Range(Constants.MIN_ENEMY_SPAWNING_TIME, Constants.MAX_ENEMY_SPAWNING_TIME);
        var remainingTime = 1f;
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
            enemyGenerationController.GenerateNewEnemy();
            remainingTime = Random.Range(Constants.MIN_ENEMY_SPAWNING_TIME, Constants.MAX_ENEMY_SPAWNING_TIME);
        }
    }

    //isGamePaused, wasGamePaused, isPauseMenuActive, isInMissions, isInAMenu, isInAlert, isInSummary, isGameFinished;
    public bool IsThereSomethingOnTheScreen() {
        return EventSystem.current.IsPointerOverGameObject()
               || isPauseMenuActive
               || isInMissions
               || isInAMenu
               || isInAlert
               || isInSummary
               || isGameFinished
               || placing;
    }
}