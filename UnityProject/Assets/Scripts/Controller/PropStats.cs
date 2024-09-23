using UnityEngine;

public class PropStats : MonoBehaviour {

    public int healthPoints = 100;
    public int MAX_HEALTHPOINTS;
    public PropsEnum propType;

    public void ReduceHealthPoints(int damage) {
        healthPoints -= damage;
        //Check if this 
        if (healthPoints <= 0) {
            DestroyProp();
        }
    }

    public bool IncreaseHealthPoints(int curation) {
        //Increase healthPoints until limit
        healthPoints += curation;
        //If its completely cured, return to caller a false to check that no more curation is needed
        if (healthPoints > MAX_HEALTHPOINTS) {
            healthPoints = MAX_HEALTHPOINTS;
            return false;
        }
        return true;
    }

    //TODO Fill this function
    private void DestroyProp() {
        //Destroy this gameobject
        
        //Notify the gameController
    }
}