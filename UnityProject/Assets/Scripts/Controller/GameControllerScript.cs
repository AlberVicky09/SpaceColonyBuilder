using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GameControllerScript : MonoBehaviour {

    #region Variables
    [SerializeField] GameObject defaultOrePrefab;
    public GameObject defaultGathererPrefab;

    public List<GameObject> mainBuildingList;
    public List<GameObject> oreGatherersList;

    public Dictionary<ResourceEnum, List<GameObject>> oreListDictionary;
    public Dictionary<ResourceEnum, Sprite> oreListImage;
    public Sprite[] oreImages;
    public Sprite missingAction;

    public GameObject uiButtonCanvas;
    public GameObject[] uiButtons;
    public Image uiRepresentation;
    public Dictionary<ResourceEnum, TMP_Text> uiResourcesTextMap;
    public Dictionary<ResourceEnum, TMP_Text> uiResourcesLossTextMap;

    public GameObject alertCanvas;
    public TMP_Text alertCanvasText;
    public bool isGamePaused = false;

    public GameObject uiInteractablePanel;
    public GameObject[] uiInteractablePanelButtons;

    private float previousMaxViewDistance = 0f;

    public Dictionary<ResourceEnum, int> resourcesDictionary;
    #endregion

    void Start() {
        //Initialize resources dictionaries
        oreListDictionary = new Dictionary<ResourceEnum, List<GameObject>>();
        resourcesDictionary = new Dictionary<ResourceEnum, int>();
        foreach (ResourceEnum resource in Enum.GetValues(typeof(ResourceEnum))) {
            oreListDictionary.Add(resource, new List<GameObject>());
            resourcesDictionary.Add(resource, 0);
        }
        //Initialize ui text resource counters
        uiResourcesTextMap = new Dictionary<ResourceEnum, TMP_Text> {
            { ResourceEnum.Water, GameObject.Find("WaterCounter").GetComponent<TMP_Text>() },
            { ResourceEnum.Food, GameObject.Find("FoodCounter").GetComponent<TMP_Text>() },
            { ResourceEnum.Iron, GameObject.Find("IronCounter").GetComponent<TMP_Text>() },
            { ResourceEnum.Gold, GameObject.Find("GoldCounter").GetComponent<TMP_Text>() },
            { ResourceEnum.Platinum, GameObject.Find("PlatinumCounter").GetComponent<TMP_Text>() }
        };
        uiResourcesLossTextMap = new Dictionary<ResourceEnum, TMP_Text> {
            { ResourceEnum.Water, GameObject.Find("WaterLossCounter").GetComponent<TMP_Text>() },
            { ResourceEnum.Food, GameObject.Find("FoodLossCounter").GetComponent<TMP_Text>() },
            { ResourceEnum.Iron, GameObject.Find("IronLossCounter").GetComponent<TMP_Text>() },
            { ResourceEnum.Gold, GameObject.Find("GoldLossCounter").GetComponent<TMP_Text>() },
            { ResourceEnum.Platinum, GameObject.Find("PlatinumLossCounter").GetComponent<TMP_Text>() }
        };
        //Initialize ore images 
        oreListImage = new Dictionary<ResourceEnum, Sprite> {
            { ResourceEnum.Water, oreImages[0] },
            { ResourceEnum.Iron, oreImages[1] },
            { ResourceEnum.Gold, oreImages[2] },
            { ResourceEnum.Platinum, oreImages[3] }
        };

        GenerateRandomOres();
    }

    public void GenerateRandomOres() {
        //Generate ores in random position inside initial circle
        for (int i = 0; i < Constants.INITIAL_ORE_NUMBER; i++) {
            var circlePos = generateNewOrePosition();
            var randomResource = Constants.ORE_RESOURCES[UnityEngine.Random.Range(0, Constants.ORE_RESOURCES.Count)];
            var instantiatedOre = Instantiate(defaultOrePrefab, new Vector3(circlePos.x, Constants.ORE_FLOOR_OFFSET, circlePos.y), Quaternion.identity);

            instantiatedOre.name = randomResource.ToString() + circlePos.ToString();

            instantiatedOre.GetComponent<OreBehaviour>().resourceType = randomResource;
            instantiatedOre.GetComponent<Renderer>().material.color = Constants.ORE_COLOR_MAP[randomResource];
            instantiatedOre.GetComponent<Renderer>().material.SetFloat("_Glossiness", Constants.ORE_SMOOTHNESS_MAP[randomResource]);
            instantiatedOre.GetComponent<Renderer>().material.SetFloat("_Metallic", Constants.ORE_METALLIC_MAP[randomResource]);
            
            oreListDictionary[randomResource].Add(instantiatedOre);
        }
    }

    private Vector2 generateNewOrePosition() {
        //Generates a new valid ore position within range
        var valid = false;
        Vector2 pos = new Vector2();
        while (!valid) {
            pos = UnityEngine.Random.insideUnitCircle * Constants.VIEW_DISTANCE_RANGE + new Vector2(previousMaxViewDistance, previousMaxViewDistance);
            valid = true;
            Vector2 currentOrePos;
            foreach (ResourceEnum resource in Enum.GetValues(typeof(ResourceEnum))) {
                for (int i = 0; i < oreListDictionary[resource].Count; i++) {
                    currentOrePos = new Vector2(oreListDictionary[resource][i].transform.position.x, oreListDictionary[resource][i].transform.position.y);
                    if (Vector2.Distance(pos, currentOrePos) < 15) { valid = false; break; }
                }
                if(!valid) { break; }
            }
        }
        return pos;
    }

    public void CalculateOreForGatherer(GameObject oreGatherer) {
        //Finds nearest ore of specified type
        var gathererBehaviour = oreGatherer.GetComponent<GathererBehaviour>();
        var nearestOre = Utils.FindNearestInList(oreGatherer, oreListDictionary[gathererBehaviour.resourceGatheringType]);

        if (nearestOre is not null) {
            gathererBehaviour.objectiveItem = nearestOre.transform;
            gathererBehaviour.UpdateDestination();
        } else {
            gathererBehaviour.DisplayAction(missingAction);
        }
    }

    public void ActivateAlertCanvas(string alertDisplayText) {
        //Display alert canvas with specified text and stop time
        alertCanvas.SetActive(true);
        alertCanvasText.text = alertDisplayText;
        PauseGame();
    }

    public void CloseAlertCanvas() {
        //Close alert panel and restart time
        alertCanvas.SetActive(false);
        PlayVelocity(Constants.TIME_SCALE_NORMAL);
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
}
