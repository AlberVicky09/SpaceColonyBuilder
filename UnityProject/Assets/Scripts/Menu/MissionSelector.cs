using UnityEngine;

public class MissionSelector : MonoBehaviour {
   
    public MissionSelectionManager missionSelectionManager;
    public int missionNumber;
    
    private void OnMouseEnter() {
        AudioManager.Instance.PlaySfx(SfxTrackNamesEnum.OnHoverMenu);
    }

    private void OnMouseDown() {
        if (missionSelectionManager.isAlradyInMission) {
            if (missionSelectionManager.missionsAvailable[missionNumber]) {
                missionSelectionManager.MoveToMission(missionNumber);
                AudioManager.Instance.PlaySfx(SfxTrackNamesEnum.EngineNoise);
            } else {
                AudioManager.Instance.PlaySfx(SfxTrackNamesEnum.OnClickInvalid);
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        missionSelectionManager.DisplayCurrentMission(missionNumber);
    }
}
