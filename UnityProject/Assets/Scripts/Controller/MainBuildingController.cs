using UnityEngine;

public class MainBuildingController : MonoBehaviour
{
    private GameControllerScript gameControllerScript;

    private void Start() {
        gameControllerScript = GameObject.Find("GameController").GetComponent<GameControllerScript>();
    }

    public void GenerateGatherer() {
        var instantiatedGatherer = Instantiate(gameControllerScript.defaultGathererPrefab, new Vector3(gameObject.transform.position.x - 5f, gameObject.transform.position.y, gameObject.transform.position.z - 3f), Quaternion.identity);
        gameControllerScript.oreGatherersList.Add(instantiatedGatherer);
    }
}
