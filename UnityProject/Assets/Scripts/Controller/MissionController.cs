using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionController : MonoBehaviour
{
    private int currentMission;
    private int completedMissions;
    private MissionListDTO missionListDto;

    public GameObject[] missionUIList, missionUIListDuplicated;
    public Image[] missionUIBg;
    public TMP_Text[] missionListText, missionListTextDuplicated;

    public GameObject endGameCanvas;
    public TMP_Text endGameText, missionsCompletedText, timeSpentText;

    private void Start() {
        currentMission = PlayerPrefs.GetInt("mission", 0);
        //TODO just for debugging
        currentMission = 2;
        missionListDto = JsonUtility.FromJson<MissionListDTO>(Utils.ReadFile("missionObjectives" + currentMission));
        
        for(int i = 0; i < missionUIList.Length; i++) {
            missionUIList[i].SetActive(false);
            missionUIListDuplicated[i].SetActive(false);
        }
        
        //Initialize all missions from file
        for (int i = 0; i < missionListDto.missions.Length; i++) {
            missionUIList[i].SetActive(true);
            missionUIListDuplicated[i].SetActive(true);
            //Display mission objective on screen
            missionListText[i].text = missionListTextDuplicated[i].text = missionListDto.missions[i].missionDescription;
            LayoutRebuilder.ForceRebuildLayoutImmediate(missionUIList[i].GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(missionUIListDuplicated[i].GetComponent<RectTransform>());
        }

        GameControllerScript.Instance.isInMissions = true;
    }

    public void CheckResourceMission(ResourceEnum resourceType, int quantity) {
        for (int i = 0; i < missionListDto.missions.Length; i++) {
            //If mission is not completed and of resource type
            if (!missionListDto.missions[i].completed && missionListDto.missions[i].missionType == MissionTypeEnum.Resource) {
                //Check if is expected resource and its current quantity
                if (resourceType.Equals((ResourceEnum)Enum.Parse(typeof(ResourceEnum), missionListDto.missions[i].objectiveName))
                    && quantity >= missionListDto.missions[i].objectiveQuantity) {
                    CompleteMission(missionListDto.missions[i], i);
                }
            }
        }
        CheckVictoryConditions();
    }

    public void CheckPropMission(PropsEnum propType, int quantity) {
        for (int i = 0; i < missionListDto.missions.Length; i++) {
            //If mission is not completed and of prop type
            if (!missionListDto.missions[i].completed && missionListDto.missions[i].missionType == MissionTypeEnum.Prop) {
                //Check if is expected prop and its current quantity
                if (propType.Equals((PropsEnum)Enum.Parse(typeof(PropsEnum), missionListDto.missions[i].objectiveName))
                    && quantity >= missionListDto.missions[i].objectiveQuantity) {
                    CompleteMission(missionListDto.missions[i], i);
                }
            }
        }
        CheckVictoryConditions();
    }
    
    public void CheckDateMission(int months) {
        for (int i = 0; i < missionListDto.missions.Length; i++) {
            //If mission is not completed and of resource type
            if (!missionListDto.missions[i].completed && missionListDto.missions[i].missionType == MissionTypeEnum.Date) {
                if(DateMissionEnum.After.Equals((DateMissionEnum)Enum.Parse(typeof(DateMissionEnum), missionListDto.missions[i].objectiveName))
                && missionListDto.missions[i].objectiveQuantity <= months) {
                    CompleteMission(missionListDto.missions[i], i);
                } else {
                    //If date is after limit, loose
                    if (missionListDto.missions[i].objectiveQuantity <= months) {
                        DisplayEndGameCanvas(Constants.LOSE_GAME_TEXT);
                        //Else, if all other missions have been completed, win
                    } else if (completedMissions == missionListDto.missionQuantity - 1) {
                        CompleteMission(missionListDto.missions[i], i);
                    }
                }
            }
        }
        CheckVictoryConditions();
    }

    public void CheckEnemiesDefeatedMission(PropsEnum enemyType, int defeatedQuantity) {
        for (int i = 0; i < missionListDto.missions.Length; i++) {
            //If mission is not completed and of resource type
            if (!missionListDto.missions[i].completed && missionListDto.missions[i].missionType == MissionTypeEnum.Enemy) {
                //Check if is expected resource and its current quantity
                if (enemyType.Equals((PropsEnum)Enum.Parse(typeof(PropsEnum), missionListDto.missions[i].objectiveName))
                    && defeatedQuantity >= missionListDto.missions[i].objectiveQuantity) {
                    CompleteMission(missionListDto.missions[i], i);
                }
            }
        }
        CheckVictoryConditions();
    }

    private void CompleteMission(MissionDTO mission, int missionPosition) {
        missionUIBg[missionPosition].sprite = GameControllerScript.Instance.greenLabelSprite;
        mission.completed = true;
        completedMissions++;
    }
    
    private void CheckVictoryConditions() {
        if (completedMissions == missionListDto.missionQuantity) {
           DisplayEndGameCanvas(Constants.WIN_GAME_TEXT);
        }
    }

    public void DisplayEndGameCanvas(string endText) {
        GameControllerScript.Instance.isGameFinished = true;
        Time.timeScale = 0f;
        endGameCanvas.SetActive(true);
        endGameText.text = endText;
        missionsCompletedText.text = $"Missions completed = {completedMissions} / {missionListDto.missionQuantity}";
        var minutesSpent = (int)(Time.timeSinceLevelLoad / 60);
        var secondsSpent = (int)(Time.timeSinceLevelLoad % 60);
        timeSpentText.text = $"Time = {minutesSpent:00}:{secondsSpent:00}";
    }

    public void EndGame() {
        //If all missions have been completed
        if (completedMissions == missionListDto.missionQuantity) {
            //Retrieve mission availability and update current mission to completed
            var missionAvailability = new MissionAvailabilityDTO();
            for (int i = 0; i <= currentMission; i++) {
                missionAvailability.boolArray[i] = true;
            }
            //Store mission availability
            Utils.WriteFile("missionsAvailable", JsonUtility.ToJson(missionAvailability));
        }

        StartCoroutine(AudioManager.Instance.UpdateScene(0.35f, false, true, "MissionSelection"));
    }
}
