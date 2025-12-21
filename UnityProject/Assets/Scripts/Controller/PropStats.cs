using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PropStats : ActionUIController_v2 {

    public int healthPoints = 100;
    public int MAX_HEALTHPOINTS;
    public PropsEnum propType;

    public RectTransform canvas;
    public Slider healthBar;
    public float healthBarOffSet;

    public Renderer renderer;
    private Color[] originalColor;
    public Clickable clickable;
    
    private void Start() {
        healthBar.maxValue = MAX_HEALTHPOINTS;
        UpdateHealthBar();
        originalColor = renderer.materials.Select(m => m.color).ToArray();
    }

    public void ReduceHealthPoints(int damage) {
        
        healthPoints -= damage;
        UpdateHealthBar();
        StartCoroutine(FlashOnColor(Color.red));
        clickable.UpdateTexts();
        //Check if this prop has enough healthPoints
        if (healthPoints <= 0) {
            DestroyProp();
        }
    }

    public bool IncreaseHealthPoints(int curation) {
        //Increase healthPoints until limit
        healthPoints += curation;
        UpdateHealthBar();
        StartCoroutine(FlashOnColor(Color.green));
        //If its completely cured, return to caller a false to check that no more curation is needed
        if (healthPoints > MAX_HEALTHPOINTS) {
            healthPoints = MAX_HEALTHPOINTS;
            clickable.UpdateTexts();
            return false;
        }
        clickable.UpdateTexts();
        return true;
    }

    private void UpdateHealthBar() {
        healthBar.value = healthPoints;
    }
    
    private void DestroyProp() {
        //Notify the gameController
        GameControllerScript.Instance.propDictionary[propType].Remove(gameObject);
        
        //If is an enemy, check enemy defeat missions
        GameControllerScript.Instance.missionController.CheckEnemiesDefeatedMission(propType, 1);

        if (PropsEnum.MainBuilding.Equals(propType)) {
            //If is the main building, end game
            GameControllerScript.Instance.missionController.DisplayEndGameCanvas(Constants.LOSE_GAME_TEXT);
        }

        //Destroy this gameobject
        Destroy(gameObject);
    }

    private IEnumerator FlashOnColor(Color flashColor) {
        for (int i = 0; i < 10; i++) {
            for (int j = 0; j < renderer.materials.Length; j++) {
                renderer.materials[j].color = i % 2 == 0 ? flashColor : originalColor[j];
            }
            yield return new WaitForSecondsRealtime(0.2f);
        }
    }
}