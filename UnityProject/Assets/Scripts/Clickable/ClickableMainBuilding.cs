using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClickableMainBuilding : Clickable {

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
    
    public override void UpdateTexts() {
        GameControllerScript.Instance.actionText.text = "Main building";
    }
    
    protected override void StartButtons() {
        base.StartButtons();
        GameControllerScript.Instance.actionButtons[0].GetComponent<Button>().onClick.AddListener(DisplayScreen);
        GameControllerScript.Instance.actionButtons[1].GetComponent<Button>().onClick.AddListener(RepairBase);
        GameControllerScript.Instance.actionButtons[0].GetComponent<OnHoverBehaviour>().hoveringDisplayText = "Build";
        GameControllerScript.Instance.actionButtons[1].GetComponent<OnHoverBehaviour>().hoveringDisplayText = "Repair base";
    }
    
    private void DisplayScreen() {
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

    private void RepairBase() {
        //TODO Repair base method (cost depending on current damage)
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
            
            //Reset gatherer
            SetObjectTransparency(false);
            if(propRigibody != null) { propRigibody.detectCollisions = true; }
            if(propCollider != null) { propCollider.isTrigger = false; }
            
            ResetPlacingVariables(true);
            
        } else {
            //TODO Quitar el prop porque no puedes pagarlo (checkear)
            Debug.Log("Not enough resources");
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
            GameControllerScript.Instance.resourcesDictionary[propCost.Key] -= propCost.Value;
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
        GameControllerScript.Instance.PlayVelocity(1f);
        GameControllerScript.Instance.SwapUIInteraction();
    }
}
