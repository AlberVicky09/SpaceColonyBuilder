using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodGeneratorController : MonoBehaviour
{
    private GameControllerScript gameControllerScript;

    private void Start() {
        gameControllerScript = GameObject.Find("GameController").GetComponent<GameControllerScript>();
    }

    private IEnumerator GenerateFood() {
        // @TODO : Something
        yield return new WaitForSeconds(1);
    }

}
