using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class ClickableMainBuilding : Clickable {

    private bool placeable;
    private float placingDelay;
    private Ray placingRay;
    private RaycastHit hitPoint;
    
    private GameObject instantiatedGatherer;
    private Rigidbody gathererRigibody;
    private BoxCollider gathererCollider;
    private Renderer[] renderers;
    
    public LayerMask placementMask, obstructionMask;
    
    public override void UpdateTexts() {
        gameControllerScript.actionText.text = "Main building";
    }
    
    protected override void StartButtons() {
        base.StartButtons();
        gameControllerScript.actionButtons[0].GetComponent<Button>().onClick.AddListener(GenerateGatherer);
    }

    private void GenerateGatherer() {
        if (gameControllerScript.resourcesDictionary[ResourceEnum.Iron] > Constants.INITIAL_GATHERER_PRICE[0] 
            && gameControllerScript.resourcesDictionary[ResourceEnum.Gold] > Constants.INITIAL_GATHERER_PRICE[1]
            && gameControllerScript.resourcesDictionary[ResourceEnum.Platinum] > Constants.INITIAL_GATHERER_PRICE[2]) {
            gameControllerScript.placing = true;
        } else {
            gameControllerScript.ActivateAlertCanvas("Not enough resources");
            Debug.Log("Not enough resources");
        }
        
    }

    private void Update() {
        if (gameControllerScript.placing) {
            placingDelay += Time.deltaTime;
            //Locate object with mouse
            placingRay = cameraMove.cameraGO.ScreenPointToRay(Input.mousePosition);
            
            if (instantiatedGatherer == null) {
                //Instantiate gatherer
                Physics.Raycast(placingRay, out hitPoint, Mathf.Infinity, placementMask);
                instantiatedGatherer = Instantiate(gameControllerScript.defaultGathererPrefab, hitPoint.point, Quaternion.identity);
                
                //Disable collisions
                gathererRigibody = instantiatedGatherer.GetComponent<Rigidbody>();
                gathererRigibody.detectCollisions = false;
                gathererCollider = instantiatedGatherer.GetComponent<BoxCollider>();
                gathererCollider.isTrigger = true;
                
                //Disable UI collisions
                gameControllerScript.SwapUIInteraction();
                
                //Store gatherer material info
                renderers = instantiatedGatherer.GetComponentsInChildren<Renderer>();
            }
            
            // Check if the ray hits any object in the obstruction layer first
            if (Physics.Raycast(placingRay, out hitPoint, Mathf.Infinity, obstructionMask)) {
                if (placeable) {
                    // If the ray hits an obstructing object, prevent placing
                    placeable = false;
                    SetObjectTransparencyr(false);
                }
            } else {
                if (!placeable) {
                    placeable = true;
                    SetObjectTransparencyr(true);
                }
                Physics.Raycast(placingRay, out hitPoint, Mathf.Infinity, placementMask);
            }

            instantiatedGatherer.transform.position = new Vector3(hitPoint.point.x, 0.0f, hitPoint.point.z);

            if (placeable && placingDelay > Constants.PLACING_REQUIRED_DELAY && Input.GetMouseButtonDown(0)) {
                placingDelay = 0f;
                PlaceGatherer();
            }
        }
    }

    void SetObjectTransparencyr(bool isTransparent) {
        for (int i = 0; i < renderers.Length; i++) {
            var material = renderers[i].material;
            var color = material.color;
            color.a = isTransparent ? 0.25f : 1.0f;
            material.color = color;
        }
    }
    
    private void PlaceGatherer() {
        gameControllerScript.oreGatherersList.Add(instantiatedGatherer);

        gameControllerScript.resourcesDictionary[ResourceEnum.Iron] -= Constants.INITIAL_GATHERER_PRICE[0];
        gameControllerScript.resourcesDictionary[ResourceEnum.Gold] -= Constants.INITIAL_GATHERER_PRICE[1];
        gameControllerScript.resourcesDictionary[ResourceEnum.Platinum] -= Constants.INITIAL_GATHERER_PRICE[2];

        //Check if mission is completed
        missionController.CheckPropMission(PropsEnum.Gatherer, gameControllerScript.oreGatherersList.Count);
        
        //Reset gatherer
        SetObjectTransparencyr(false);
        gathererRigibody.detectCollisions = true;
        
        gameControllerScript.SwapUIInteraction();
        
        gameControllerScript.placing = placeable = gathererCollider.isTrigger = false;
        instantiatedGatherer = null;
        renderers = null;
    }
}
