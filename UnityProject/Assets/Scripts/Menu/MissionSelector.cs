using System;
using UnityEngine;

public class MissionSelector : MonoBehaviour {
   
    public MissionSelectionManager missionSelectionManager;
    public int missionNumber;
    
    private void OnMouseDown() {
        missionSelectionManager.MoveToMission(missionNumber);        
    }

    private void OnTriggerEnter(Collider other) {
        missionSelectionManager.DisplayCurrentMission(missionNumber);
    }
}
