using UnityEngine;

public class MainBuildingController : MonoBehaviour
{
    private GameControllerScript gameControllerScript;
    private MissionController missionController;

    private void Start() {
        gameControllerScript = GameObject.Find("GameController").GetComponent<GameControllerScript>();
        missionController = GameObject.Find("MissionPanel").GetComponent<MissionController>();
    }
}
