using UnityEngine;

public class MissionSelector : MonoBehaviour {
   
    public int missionNumber;
    
    private void OnMouseEnter() {
        AudioManager.Instance.PlaySfx(SfxTrackNamesEnum.OnHoverMenu);
    }

    private void OnMouseDown() {
        if (MissionSelectionManager.Instance.missionsAvailable[missionNumber]) {
            MissionSelectionManager.Instance.MoveToMission(missionNumber);
            AudioManager.Instance.PlaySfx(SfxTrackNamesEnum.EngineNoise);
        } else {
            AudioManager.Instance.PlaySfx(SfxTrackNamesEnum.OnClickInvalid);
        }
    }

    private void OnTriggerEnter(Collider other) {
        MissionSelectionManager.Instance.DisplayCurrentMission(missionNumber);
    }
}
