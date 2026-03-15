using UnityEngine;

public class CompletedMissionsController : MonoBehaviour {

    public static CompletedMissionsController Instance;
    public MissionAvailabilityDTO missionAvailability;
    public bool missionsRecovered;
    
    public void Awake() {
        if(Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        
        //Retrieve completed missions from file
        try {
            missionAvailability = JsonUtility.FromJson<MissionAvailabilityDTO>(Utils.ReadFile("missionsAvailable"));
            missionsRecovered = true;
        } catch {
            missionAvailability = new MissionAvailabilityDTO(new []{true, false, false, false});
            missionsRecovered = false;
        }
    }

    
}
