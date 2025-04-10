using System;
using System.Collections.Generic;
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
    private Renderer[] propRenderers;
    
    public LayerMask placementMask, obstructionMask;

    private void Start() {
        healingSlider.onValueChanged.AddListener(s => SetUpSlider(s));
    }

    public override void UpdateTexts() {
        GameControllerScript.Instance.actionText.text = "Main building";
    }
    
    protected override void StartButtons() {
        base.StartButtons();
        GameControllerScript.Instance.actionButtons[0].GetComponent<Button>().onClick.AddListener(DisplayBuildableScreen);
        GameControllerScript.Instance.actionButtons[0].GetComponent<OnHoverBehaviour>().hoveringDisplayText = "Build";
        if (mainBuildingStats.healthPoints < mainBuildingStats.MAX_HEALTHPOINTS) {
            GameControllerScript.Instance.actionButtons[1].GetComponent<Button>().onClick.AddListener(DisplayRepairingScreen);
            GameControllerScript.Instance.actionButtons[1].GetComponent<OnHoverBehaviour>().hoveringDisplayText = "Repair base";
        } else {
            GameControllerScript.Instance.actionButtons[1].SetActive(true);
        }
    }
    
    private void DisplayBuildableScreen() {
        //Activate canvas and place buttons
        GameControllerScript.Instance.interactableButtonManager.PlaceButtonsInCircle(Constants.BUILDABLE_LIST.Count);
        
        //Setup buttons behaviour
        for(int i = 0; i < Constants.BUILDABLE_LIST.Count; i++) {
            GameControllerScript.Instance.interactableButtonManager.interactableButtonList[i].GetComponent<Button>().onClick.RemoveAllListeners();
            var currentProp = Constants.BUILDABLE_LIST[i];
            GameControllerScript.Instance.interactableButtonManager.interactableButtonList[i].GetComponent<Button>().onClick.AddListener(() => { GenerateProp(currentProp); });
            GameControllerScript.Instance.interactableButtonManager.interactableButtonList[i].GetComponentInChildren<TMP_Text>().text = currentProp.ToString();
        }
        
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
        if (CheckEnoughResources(Constants.BUILDABLE_PRICES[prop])) {
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
                GameControllerScript.Instance.PlayVelocity(1f);
            } else {
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
                            PropsEnum.BasicFighter => GameControllerScript.Instance.fighterPrefab
                        },
                        hitPoint.point,
                        Quaternion.identity);
                    
                    //Disable collisions if needed
                    propRigibody = instantiatedProp.GetComponent<Rigidbody>();
                    if (propRigibody != null) { propRigibody.detectCollisions = false; }
                    propCollider = instantiatedProp.GetComponent<BoxCollider>();
                    if (propCollider != null) { propCollider.isTrigger = true; }
                    
                    //Disable UI collisions
                    GameControllerScript.Instance.SwapUIInteraction();
                    
                    //Store gatherer material info
                    propRenderers = instantiatedProp.GetComponentsInChildren<Renderer>();
                }
                
                // Check if the ray hits any object in the obstruction layer first
                if (Physics.Raycast(placingRay, out hitPoint, Mathf.Infinity, obstructionMask)) {
                    Debug.Log("Is not placeable");
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
        
        GameControllerScript.Instance.PlayVelocity(1f);
    }
    
    private void SetObjectTransparency(bool isTransparent) {
        for (int i = 0; i < propRenderers.Length; i++) {
            var material = propRenderers[i].material;
            var color = material.color;
            color.a = isTransparent ? 0.25f : 1.0f;
            material.color = color;
        }
    }
    
    private void PlaceProp() {
        if (CheckEnoughResources(Constants.BUILDABLE_PRICES[currentProp])) {
            //Add prop to list
            GameControllerScript.Instance.propDictionary[currentProp].Add(instantiatedProp);
            
            //Reduce prop resources
            ReducePriceResources(Constants.BUILDABLE_PRICES[currentProp]);

            //Check if mission is completed
            GameControllerScript.Instance.missionController.CheckPropMission(currentProp, GameControllerScript.Instance.propDictionary[currentProp].Count);

            if (PropsEnum.Storage.Equals(currentProp)) {
                GameControllerScript.Instance.resourcesLimit += Constants.RESOURCES_LIMIT_INCREASE;
                foreach (var maxResourceText in GameControllerScript.Instance.uiMaxResourcesList) {
                    maxResourceText.text = GameControllerScript.Instance.resourcesLimit.ToString();
                }
            }
            
            //Reset gatherer
            SetObjectTransparency(false);
            if(propRigibody != null) { propRigibody.detectCollisions = true; }
            if(propCollider != null) { propCollider.isTrigger = false; }
            
            ResetPlacingVariables(true);
            GameControllerScript.Instance.PlayVelocity(1f);
        } else {
            GameControllerScript.Instance.ActivateAlertCanvas("Not enough resources");
            ResetPlacingVariables(false);
        }
    }
    
    private bool CheckEnoughResources(Dictionary<ResourceEnum, int> propCosts) {
        foreach (var propCost in propCosts) {
            if (GameControllerScript.Instance.resourcesDictionary[propCost.Key] < propCost.Value) {
                return false;
            }
        }
        return true;
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
        propCollider = null;
        propRigibody = null;
        GameControllerScript.Instance.SwapUIInteraction();
    }
}
