using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClickableMainBuilding : Clickable {

    public Canvas healingCanvas;
    public Slider healingSlider;
    public RectTransform healingSliderRectTransform;
    public RectTransform healingTextRectTransform;
    public Image healingSliderFill;
    public TMP_Text healingText;
    public RectTransform healingSliderBeginning;
    public Button healingAcceptButton;
    public PropStats mainBuildingStats;
    private int healingAmount;
    
    private bool placeable;
    private float placingDelay;
    private Ray placingRay;
    private RaycastHit hitPoint;

    private PropsEnum currentProp;
    private GameObject instantiatedProp;
    private Rigidbody propRigibody;
    private BoxCollider propCollider;
    public Renderer[] propRenderers;
    private Color[] propOriginalColor;
    private Color invalidColor;
    
    public LayerMask placementMask, obstructionMask;

    private void Start() {
        healingSlider.onValueChanged.AddListener(s => SetUpSlider(s));
        invalidColor = Color.red;
        invalidColor.a = 0.3f;
    }

    public override void UpdateTexts() {
        if (selectedClickable == this) {
            GameControllerScript.Instance.actionText.text = "Main building " + "\nHealth: " + mainBuildingStats.healthPoints + "/" + mainBuildingStats.MAX_HEALTHPOINTS;
        }
    }

    protected override void DisplayButtons() {
        base.DisplayButtons();
        
        //If the base is fully healed, disable the button
        if (mainBuildingStats.healthPoints == mainBuildingStats.MAX_HEALTHPOINTS) {
            GameControllerScript.Instance.actionButtons[1].SetActive(false);
        }
    }

    protected override void StartButtons() {
        base.StartButtons();
        GameControllerScript.Instance.actionButtons[0].GetComponent<Button>().onClick.AddListener(DisplayBuildableScreen);
        GameControllerScript.Instance.actionButtons[0].GetComponent<OnHoverBehaviour>().hoveringDisplayText = "Build";
        if (mainBuildingStats.healthPoints < mainBuildingStats.MAX_HEALTHPOINTS) {
            GameControllerScript.Instance.actionButtons[1].GetComponent<Button>().onClick.AddListener(DisplayRepairingScreen);
            GameControllerScript.Instance.actionButtons[1].GetComponent<OnHoverBehaviour>().hoveringDisplayText = "Repair base";
        }
    }
    
    private void DisplayBuildableScreen() {
        //Activate canvas and place buttons
        GameControllerScript.Instance.interactableButtonManager.PlaceButtonsInCircle(BuildableProps.RetrieveBuildableProps().Count);
        
        //Setup buttons behaviour
        for(int i = 0; i < BuildableProps.RetrieveBuildableProps().Count; i++) {
            GameControllerScript.Instance.interactableButtonManager.interactableButtonList[i].GetComponent<Button>().onClick.RemoveAllListeners();
            var currentProp = BuildableProps.RetrieveBuildableProps()[i];
            //Setup button
            GameControllerScript.Instance.interactableButtonManager.interactableButtonList[i].GetComponent<Button>().onClick.AddListener(() => { GenerateProp(currentProp); });
            GameControllerScript.Instance.interactableButtonManager.interactableButtonImageList[i].sprite = GameControllerScript.Instance.propSpriteDictionary[currentProp];
            GameControllerScript.Instance.interactableButtonManager.interactableButtonImageList[i]
                .GetComponent<RectTransform>().localScale = Constants.INTERACTABLE_BUTTON_PROP_SCALE;
            
            //Setup hover behaviour
            var onHoverBehaviour = GameControllerScript.Instance.interactableButtonManager.interactableButtonList[i].GetComponent<OnHoverBehaviour>();
            onHoverBehaviour.hoveringDisplayText = Constants.PROPS_SUMMARY_NAME[currentProp];
            onHoverBehaviour.usesResourceTooltip = true;
            onHoverBehaviour.RefreshText();
        }
        
        //Force button width update
        GameControllerScript.Instance.interactableButtonManager.ForceUpdateOfButtons();
        
        GameControllerScript.Instance.actionCanvas.SetActive(false);
        GameControllerScript.Instance.PauseGame();
    }

    private void DisplayRepairingScreen() {
        healingCanvas.gameObject.SetActive(true);

        //Show slider at full fill
        healingSlider.value = 1;
        SetUpSlider(healingSlider.value);
        
        GameControllerScript.Instance.actionCanvas.SetActive(false);
        GameControllerScript.Instance.PauseGame();
    }

    private void GenerateProp(PropsEnum prop) {
        if (Utils.CheckEnoughResources(GameControllerScript.Instance.resourcesDictionary, Constants.PROP_CREATION_PRICES[prop])) {
            GameControllerScript.Instance.placing = true;
            GameControllerScript.Instance.interactableButtonManager.gameObject.SetActive(false);
            currentProp = prop;
            
        } else {
            GameControllerScript.Instance.ActivateAlertCanvas("Not enough resources");
            Debug.Log("Not enough resources");
        }
    }

    private void Update() {
        if (GameControllerScript.Instance.placing) {
            //Stop placing if right click is pressed or scape button is pressed
            if (Input.GetMouseButton(1)) {
                ResetPlacingVariables(false);
                GameControllerScript.Instance.PlayVelocity(Constants.TIME_SCALE_NORMAL);
            } else if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape)) {
                GameControllerScript.Instance.wasGamePaused = false;
                ResetPlacingVariables(false);
            }else {
                placingDelay += Time.unscaledDeltaTime;
                //Locate object with mouse
                placingRay = GameControllerScript.Instance.cameraMove.cameraGO.ScreenPointToRay(Input.mousePosition);
                
                if (instantiatedProp == null) {
                    Physics.Raycast(placingRay, out hitPoint, Mathf.Infinity, placementMask);
                    //Instantiate depending of currentProp
                    instantiatedProp = Instantiate(
                        currentProp switch {
                            PropsEnum.Gatherer => GameControllerScript.Instance.gathererPrefab,
                            PropsEnum.FoodGenerator => GameControllerScript.Instance.foodGeneratorPrefab,
                            PropsEnum.House => GameControllerScript.Instance.housePrefab,
                            PropsEnum.Storage => GameControllerScript.Instance.storagePrefab,
                            PropsEnum.Fighter => GameControllerScript.Instance.fighterPrefab
                        },
                        hitPoint.point,
                        Quaternion.identity);
                    
                    //Put a name on it if its a fighter
                    if (PropsEnum.Fighter.Equals(currentProp)) {
                        instantiatedProp.name = "PlayerFighter";
                    }
                    
                    //Disable collisions if needed
                    propRigibody = instantiatedProp.GetComponent<Rigidbody>();
                    if (propRigibody != null) { propRigibody.detectCollisions = false; }
                    propCollider = instantiatedProp.GetComponent<BoxCollider>();
                    if (propCollider != null) { propCollider.isTrigger = true; }
                    
                    //Disable UI collisions
                    GameControllerScript.Instance.SwapUIInteraction();
                    
                    //Store gatherer material info
                    propRenderers = instantiatedProp.GetComponentsInChildren<Renderer>();
                    propOriginalColor = propRenderers
                        .SelectMany(r => r.materials)
                        .Select(m => m.color)
                        .ToArray();
                }
                
                // Check if the ray hits any object in the obstruction layer first
                if (Physics.Raycast(placingRay, out hitPoint, Mathf.Infinity, obstructionMask)) {
                    if (placeable) {
                        // If the ray hits an obstructing object, prevent placing
                        placeable = false;
                        SetObjectTransparency(false);
                    }
                } else {
                    if (!placeable) {
                        placeable = true;
                        SetObjectTransparency(true);
                    }
                    Physics.Raycast(placingRay, out hitPoint, Mathf.Infinity, placementMask);
                }

                instantiatedProp.transform.position = new Vector3(hitPoint.point.x, 0.0f, hitPoint.point.z);

                if (placeable && placingDelay > Constants.PLACING_REQUIRED_DELAY && Input.GetMouseButtonDown(0)) {
                    placingDelay = 0f;
                    PlaceProp();
                }
            }
        }
    }

    private void SetUpSlider(float sliderValue) {
        //Update position of healingQuantity to follow slider
        healingTextRectTransform.anchoredPosition = new Vector2(
            healingSliderBeginning.anchoredPosition.x + sliderValue * healingSliderRectTransform.rect.width,
            healingTextRectTransform.anchoredPosition.y);
            
        //Update value of the healingQuantity
        healingAmount = (int)Math.Ceiling(sliderValue * (mainBuildingStats.MAX_HEALTHPOINTS - mainBuildingStats.healthPoints));
        healingText.text = healingAmount + " iron";
        
        //If healing is more than can pay, put it in red and dissable accept button
        var isValidHealing = healingAmount < GameControllerScript.Instance.resourcesDictionary[ResourceEnum.Iron];
        healingAcceptButton.interactable = isValidHealing;
        healingSliderFill.color = isValidHealing
            ? Constants.GREEN_COLOR
            : Constants.RED_COLOR;
    }

    public void DoHeal() {
        //Remove cost of current resources
        GameControllerScript.Instance.uiUpdateController.UpdateResource(ResourceEnum.Iron, healingAmount, ResourceOperationEnum.Decrease);
        
        //Heal mainBuilding
        mainBuildingStats.IncreaseHealthPoints(healingAmount);
        
        GameControllerScript.Instance.PlayVelocity(Constants.TIME_SCALE_NORMAL);
    }
    
    private void SetObjectTransparency(bool isPositionValid) {
        var counter = 0;
        for (int i = 0; i < propRenderers.Length; i++) {
            for (int j = 0; j < propRenderers[i].materials.Length; j++) {
                propRenderers[i].materials[j].color =
                    isPositionValid ? propOriginalColor[counter] : invalidColor;
                counter++;
            }
        }
    }
    
    private void PlaceProp() {
        if (Utils.CheckEnoughResources(GameControllerScript.Instance.resourcesDictionary , Constants.PROP_CREATION_PRICES[currentProp])) {
            //Add prop to list
            GameControllerScript.Instance.propDictionary[currentProp].Add(instantiatedProp);
            
            //Reduce prop resources
            ReducePriceResources(Constants.PROP_CREATION_PRICES[currentProp]);

            //Call placing function (instead of start, in case of delayed operation needed, avoid activating until placed
            instantiatedProp.GetComponent<Placeable>().OnPropPlaced();
                
            //Check if mission is completed
            GameControllerScript.Instance.missionController.CheckPropMission(currentProp, GameControllerScript.Instance.propDictionary[currentProp].Count);

            //Reset gatherer
            SetObjectTransparency(true);
            if(propRigibody != null) { propRigibody.detectCollisions = true; }
            if(propCollider != null) { propCollider.isTrigger = false; }
            
            ResetPlacingVariables(true);
            GameControllerScript.Instance.PlayVelocity(Constants.TIME_SCALE_NORMAL);
        } else {
            GameControllerScript.Instance.ActivateAlertCanvas("Not enough resources");
            Debug.Log("Couldnt place it");
            ResetPlacingVariables(false);
        }
    }

    private void ReducePriceResources(Dictionary<ResourceEnum, int> propCosts) {
        foreach (var propCost in propCosts) {
            GameControllerScript.Instance.uiUpdateController.UpdateResource(propCost.Key, propCost.Value, ResourceOperationEnum.Decrease);
        }
    }

    private void ResetPlacingVariables(bool hasBeenPlaced) {
        //Reset placing variables
        GameControllerScript.Instance.placing = placeable = false;
        if(!hasBeenPlaced) Destroy(instantiatedProp);
        instantiatedProp = null;
        propRenderers = null;
        propOriginalColor = null;
        propCollider = null;
        propRigibody = null;
        GameControllerScript.Instance.SwapUIInteraction();
    }
}
