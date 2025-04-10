using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FoodGeneratorController : MonoBehaviour
{
    public RectTransform canvas;
    public GameObject actionPercentage;
    public GameObject idleAction;
    private float actionPercentageValue;
    private Image actionPercentageImage;
    private Coroutine generatorCoroutine;
    public bool isGeneratorPaused = false;

    private void Start() {
        actionPercentageImage = actionPercentage.GetComponent<Image>();
        generatorCoroutine = StartCoroutine(GenerateFood());
    }

    private void Update() {
        if (actionPercentage.activeSelf && !isGeneratorPaused) {
            Utils.LocateMarkerOverGameObject(gameObject, actionPercentage, 3.5f, canvas);
            actionPercentageValue += Time.deltaTime;
            actionPercentageImage.fillAmount = actionPercentageValue / 5.0f;

        } else {
            Utils.LocateMarkerOverGameObject(gameObject, idleAction, 3.5f, canvas);
        }
        
    }
    
    private IEnumerator GenerateFood() {
        while (true) {
            //If is paused, dont generate
            while (isGeneratorPaused) { yield return null; }
            
            yield return new WaitForSeconds(5);
            if (GameControllerScript.Instance.resourcesDictionary[ResourceEnum.Water] >= 15) {
                actionPercentage.SetActive(true);
                actionPercentageValue = 0f;
                actionPercentageImage.fillAmount = 0f;
                idleAction.SetActive(false);

                //Remove water
                GameControllerScript.Instance.uiUpdateController.UpdateResource(ResourceEnum.Water, 15, ResourceOperationEnum.Decrease);
                
                //Add food
                GameControllerScript.Instance.uiUpdateController.UpdateResource(ResourceEnum.Food, 30, ResourceOperationEnum.Increase);
            } else {
                Debug.Log("Missing water");
                actionPercentage.SetActive(false);
                idleAction.SetActive(true);
            }
        }
    }

}
