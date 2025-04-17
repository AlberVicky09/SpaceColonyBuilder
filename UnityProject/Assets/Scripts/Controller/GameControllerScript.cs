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
    
    public CanvasGroup canvasGroup;

    public GameObject orePrefab;
    public GameObject gathererPrefab;
    public GameObject foodGeneratorPrefab;
    public GameObject housePrefab;
    public GameObject storagePrefab;
    public GameObject fighterPrefab;

    public GameObject mainBuilding, startingGatherer;
    public Dictionary<PropsEnum, List<GameObject>> propDictionary;

    public Dictionary<ResourceEnum, List<ResourceTuple>> oreListDictionary;
    public Dictionary<ResourceEnum, Sprite> oreListImage;
    public Sprite[] oreImages;
    public Sprite missingAction, goingToAction;
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
    public bool isGamePaused, isPauseMenuActive;
    
    public bool isTutorialActivated;

    public Dictionary<ResourceEnum, int> resourcesDictionary;
    public int resourcesLimit = Constants.INITIAL_RESOURCES_LIMIT;

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
        oreListImage = new Dictionary<ResourceEnum, Sprite> {
            { ResourceEnum.Water, oreImages[0] },
            { ResourceEnum.Iron, oreImages[1] },
            { ResourceEnum.Gold, oreImages[2] },
            { ResourceEnum.Platinum, oreImages[3] }
        };
        
        //Initialize prop dictionary
        propDictionary = new Dictionary<PropsEnum, List<GameObject>>();
        foreach (PropsEnum propType in Enum.GetValues(typeof(PropsEnum))) {
            propDictionary.Add(propType, new List<GameObject>());
        }
        propDictionary[PropsEnum.MainBuilding].Add(mainBuilding);
        propDictionary[PropsEnum.Gatherer].Add(startingGatherer);
        
        GenerateRandomOres();
    }

    void Start() {
        //If tutorial has been activated,< start it, else start enemy generation
        isTutorialActivated = PlayerPrefs.GetInt("tutorialActivated", 1) == 0;
        if (!isTutorialActivated) {
            StartCoroutine(GenerateEnemyShipsCoroutine());
        } else {
            enemyCountDownText.text = "No enemies in sight";
            enemyCountDownBg.sprite = greenLabelSprite;
            //tutorialController.DisplayNextTutorial();
        }
        
        PauseGame();
    }
    
    public void GenerateRandomOres() {
        //Generate ores in random position inside initial circle
        for (int i = 0; i < Constants.INITIAL_ORE_NUMBER; i++) {
            var circlePos = GenerateNewOrePosition();
            var randomResource = Constants.ORE_RESOURCES[Random.Range(0, Constants.ORE_RESOURCES.Count)];
            var instantiatedOre = Instantiate(orePrefab, new Vector3(circlePos.x, Constants.ORE_FLOOR_OFFSET, circlePos.y), Quaternion.identity);

            instantiatedOre.name = randomResource.ToString() + circlePos.ToString();

            instantiatedOre.GetComponent<OreBehaviour>().SetResourceType(randomResource);
            instantiatedOre.GetComponent<Renderer>().material.color = Constants.ORE_COLOR_MAP[randomResource];
            instantiatedOre.GetComponent<Renderer>().material.SetFloat("_Glossiness", Constants.ORE_SMOOTHNESS_MAP[randomResource]);
            instantiatedOre.GetComponent<Renderer>().material.SetFloat("_Metallic", Constants.ORE_METALLIC_MAP[randomResource]);
            
            oreListDictionary[randomResource].Add(new ResourceTuple(instantiatedOre, false));
            numberOfOres++;
        }
    }

    private Vector2 GenerateNewOrePosition() {
        //Generates a new valid ore position within range
        var valid = false;
        Vector2 pos = new Vector2();
        while (!valid) {
            pos = Random.insideUnitCircle * Constants.VIEW_DISTANCE_RANGE;
            valid = true;
            Vector2 currentOrePos;
            foreach (ResourceEnum resource in Enum.GetValues(typeof(ResourceEnum))) {
                for (int i = 0; i < oreListDictionary[resource].Count; i++) {
                    currentOrePos = new Vector2(oreListDictionary[resource][i].gameObject.transform.position.x, oreListDictionary[resource][i].gameObject.transform.position.y);
                    if (Vector2.Distance(pos, currentOrePos) < 15) { valid = false; break; }
                }
                if(!valid) { break; }
            }
        }
        return pos;
    }

    public void RemoveOre() {
        numberOfOres--;
        if (numberOfOres <= 3) {
            GenerateRandomOres();
        }
    }

    public void CalculateOreForGatherer(GameObject oreGatherer) {
        //Finds nearest ore of specified type
        var gathererBehaviour = oreGatherer.GetComponent<GathererBehaviour>();
        var nearestOre = Utils.FindNearestGameObjectInTupleList(oreGatherer, oreListDictionary[gathererBehaviour.resourceGatheringType]);

        if (nearestOre is not null) {
            gathererBehaviour.objectiveItem = nearestOre;
            gathererBehaviour.UpdateDestination();
        } else {
            gathererBehaviour.DisplayAction(missingAction);
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
