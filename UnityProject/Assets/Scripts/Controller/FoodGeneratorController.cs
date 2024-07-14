using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FoodGeneratorController : MonoBehaviour
{
    private GameControllerScript gameControllerScript;
    public RectTransform canvas;
    public GameObject actionPercentage;
    public GameObject actionIcon;
    private float actionPercentageValue = 0f;
    private Image actionPercentageImage;

    private void Start() {
        gameControllerScript = GameObject.Find("GameController").GetComponent<GameControllerScript>();
        actionPercentageImage = actionPercentage.GetComponent<Image>();
        StartCoroutine(GenerateFood());
    }

    private void Update() {
        if (actionPercentage.activeSelf) {
            Utils.LocateMarkerOverGameObject(gameObject, actionPercentage, canvas);
            actionPercentageValue += Time.deltaTime;
            actionPercentageImage.fillAmount = actionPercentageValue / 5.0f;

        } else {
            Utils.LocateMarkerOverGameObject(gameObject, actionIcon, canvas);
        }
        
    }
    
    private IEnumerator GenerateFood() {
        while (true) {
            yield return new WaitForSeconds(5);
            if (gameControllerScript.resourcesDictionary[ResourceEnum.Water] >= 15) {
                actionPercentage.SetActive(true);
                actionPercentageValue = 0f;
                actionPercentageImage.fillAmount = 0f;
                actionIcon.SetActive(false);

                //Remove water
                gameControllerScript.UIUpdateController.UpdateResource(ResourceEnum.Water, 15, ResourceOperationEnum.Decrease);
                
                //Add food
                gameControllerScript.UIUpdateController.UpdateResource(ResourceEnum.Food, 30, ResourceOperationEnum.Increase);
            } else {
                Debug.Log("Missing water");
                actionPercentage.SetActive(false);
                actionIcon.SetActive(true);
            }
        }
    }

}
