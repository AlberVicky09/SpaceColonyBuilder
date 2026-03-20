using UnityEngine;

public class MissionInformationController : MonoBehaviour {

    public static MissionInformationController Instance;
    public MissionAvailabilityDTO missionAvailability;
    public MissionDescriptionListDTO missionDescriptionList;
    public bool missionsRecovered;
    
    public void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        //Retrieve completed missions from file
        var missionsFile = Utils.ReadFile("missionsAvailable");
        if (Constants.FILE_NOT_FOUND.Equals(missionsFile)) {
            missionAvailability = new MissionAvailabilityDTO(new []{true, false, false, false});
            missionsRecovered = false;
        } else {
            missionAvailability = JsonUtility.FromJson<MissionAvailabilityDTO>(missionsFile);
            missionsRecovered = true;
        }

        var missionTextFile = Resources.Load<TextAsset>("missionDescriptions");
        missionDescriptionList = JsonUtility.FromJson<MissionDescriptionListDTO>(missionTextFile.text);
    }

    public void RestartSaveFile() {
        missionAvailability = new MissionAvailabilityDTO(new []{true, false, false, false});
        missionsRecovered = false;
    }

    public void UpdateSavedFile() {
        missionsRecovered = true;
        //Store mission availability
        Utils.WriteFile("missionsAvailable", JsonUtility.ToJson(missionAvailability));
    }
    
    public void CompleteMission(int currentCompletedMission) {
        //Retrieve mission availability and update current mission to completed
        for (int i = 0; i <= currentCompletedMission + 1; i++) {
            missionAvailability.boolArray[i] = true;
        }
        UpdateSavedFile();
    }
}
