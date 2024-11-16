using UnityEngine;
using UnityEngine.UI;

public class PropStats : MonoBehaviour {

    public int healthPoints = 100;
    public int MAX_HEALTHPOINTS;
    public PropsEnum propType;

    public RectTransform canvas;
    public Slider healthBar;
    public float healthBarOffSet;
    
    void Update() { Utils.LocateMarkerOverGameObject(gameObject, healthBar.gameObject, healthBarOffSet, canvas); }
    
    public void ReduceHealthPoints(int damage) {
        
        healthPoints -= damage;
        UpdateHealthBar();
        //Check if this prop has enough healthPoints
        if (healthPoints <= 0) {
            DestroyProp();
        }
    }

    public bool IncreaseHealthPoints(int curation) {
        //Increase healthPoints until limit
        healthPoints += curation;
        UpdateHealthBar();
        //If its completely cured, return to caller a false to check that no more curation is needed
        if (healthPoints > MAX_HEALTHPOINTS) {
            healthPoints = MAX_HEALTHPOINTS;
            return false;
        }
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
        //If is the main building, end game
        GameControllerScript.Instance.missionController.DisplayEndGameCanvas("You have lost!");
        
        //Destroy this gameobject
        Destroy(this);
    }
}