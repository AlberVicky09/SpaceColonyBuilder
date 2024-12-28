using UnityEngine;

public class MissionSelector : MonoBehaviour {
   
    public int missionNumber;

    private void OnMouseEnter() {
        AudioManager.Instance.PlaySfx("OnHoverMenu");
    }

    private void OnMouseDown() {
        if (MissionSelectionManager.Instance.missionsAvailable[missionNumber]) {
            MissionSelectionManager.Instance.MoveToMission(missionNumber);
            AudioManager.Instance.PlaySfx("OnClickMenu");
        } else {
            AudioManager.Instance.PlaySfx("OnClickInvalid");
        }
    }

    private void OnTriggerEnter(Collider other) {
        MissionSelectionManager.Instance.DisplayCurrentMission(missionNumber);
    }
}
