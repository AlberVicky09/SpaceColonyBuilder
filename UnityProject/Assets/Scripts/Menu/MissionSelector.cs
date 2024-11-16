using UnityEngine;

public class MissionSelector : MonoBehaviour {
   
    public int missionNumber;
    
    private void OnMouseDown() {
        if (MissionSelectionManager.Instance.missionsAvailable[missionNumber]) {
            MissionSelectionManager.Instance.MoveToMission(missionNumber);
        } else {
            //TODO Do sound if mission cant be selected
        }
    }

    private void OnTriggerEnter(Collider other) {
        MissionSelectionManager.Instance.DisplayCurrentMission(missionNumber);
    }
}
