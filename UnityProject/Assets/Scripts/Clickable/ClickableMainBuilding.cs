using UnityEngine;
using UnityEngine.UI;

public class ClickableMainBuilding : Clickable {

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
            
            var instantiatedGatherer = Instantiate(gameControllerScript.defaultGathererPrefab, new Vector3(transform.position.x + 10, 0f, transform.position.z + 10), Quaternion.identity);
            gameControllerScript.oreGatherersList.Add(instantiatedGatherer);

            gameControllerScript.resourcesDictionary[ResourceEnum.Iron] -= Constants.INITIAL_GATHERER_PRICE[0];
            gameControllerScript.resourcesDictionary[ResourceEnum.Gold] -= Constants.INITIAL_GATHERER_PRICE[1];
            gameControllerScript.resourcesDictionary[ResourceEnum.Platinum] -= Constants.INITIAL_GATHERER_PRICE[2];

            missionController.CheckPropMission(PropsEnum.Gatherer, gameControllerScript.oreGatherersList.Count);
        } else {
            gameControllerScript.ActivateAlertCanvas("Not enough resources");
            Debug.Log("Not enough resources");
        }
        
    }
}
