using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionController : MonoBehaviour
{
    private int currentMission;
    private int completedMissions;
    private MissionListDTO missionListDto;
    
    public GameObject[] missionUIList;
    public TMP_Text[] missionListText;
    public Sprite missionCompletedSprite;

    public GameObject endGameCanvas;
    public TMP_Text endGameText, missionsCompletedText, timeSpentText;

    private void Awake() {
        currentMission = PlayerPrefs.GetInt("mission", 0);
        missionListDto = JsonUtility.FromJson<MissionListDTO>(Utils.ReadFile("missionObjectives"+currentMission));
    }

    private void Start() {
        foreach (var missionObject in missionUIList) {
            missionObject.SetActive(false);
        }
        
        //Initialize all missions from file
        for (int i = 0; i < missionListDto.missions.Length; i++) {
            missionUIList[i].SetActive(true);
            //Display mission objective on screen
            missionListText[i].text = missionListDto.missions[i].missionDescription;
        }
    }

    public void CheckResourceMission(ResourceEnum resourceType, int quantity) {
        foreach (var mission in missionListDto.missions) {
            //If mission is not completed and of resource type
            if (!mission.completed && mission.missionType == MissionTypeEnum.Resource) {
                //Check if is expected resource and its current quantity
                if (resourceType.Equals((ResourceEnum)Enum.Parse(typeof(ResourceEnum), mission.objectiveName))
                    && quantity >= mission.objectiveQuantity) {
                    CompleteMission(mission);
                }
            }
        }
        CheckVictoryConditions();
    }

    public void CheckPropMission(PropsEnum propType, int quantity) {
        foreach (var mission in missionListDto.missions) {
            //If mission is not completed and of prop type
            if (!mission.completed && mission.missionType == MissionTypeEnum.Prop) {
                //Check if is expected prop and its current quantity
                if (propType.Equals((PropsEnum)Enum.Parse(typeof(PropsEnum), mission.objectiveName))
                    && quantity >= mission.objectiveQuantity) {
                    CompleteMission(mission);
                }
            }
        }
        CheckVictoryConditions();
    }
    
    public void CheckDateMission(int months) {
        foreach (var mission in missionListDto.missions) {
            //If mission is not completed and of resource type
            if (!mission.completed && mission.missionType == MissionTypeEnum.Date) {
                if(DateMissionEnum.After.Equals((DateMissionEnum)Enum.Parse(typeof(DateMissionEnum), mission.objectiveName))
                && mission.objectiveQuantity <= months) {
                    CompleteMission(mission);
                } else {
                    //If date is after limit, loose
                    if (mission.objectiveQuantity <= months) {
                        DisplayEndGameCanvas("You loose!");
                        //Else, if all other missions have been completed, win
                    } else if (completedMissions == missionListDto.missionQuantity - 1) {
                        CompleteMission(mission);
                    }
                }
            }
        }
        CheckVictoryConditions();
    }

    public void CheckEnemiesDefeatedMission(PropsEnum enemyType, int defeatedQuantity) {
        foreach (var mission in missionListDto.missions) {
            //If mission is not completed and of resource type
            if (!mission.completed && mission.missionType == MissionTypeEnum.Enemy) {
                //Check if is expected resource and its current quantity
                if (enemyType.Equals((PropsEnum)Enum.Parse(typeof(PropsEnum), mission.objectiveName))
                    && defeatedQuantity >= mission.objectiveQuantity) {
                    CompleteMission(mission);
                }
            }
        }
        CheckVictoryConditions();
    }

    private void CompleteMission(MissionDTO mission) {
        mission.missionUIGameObject.sprite = missionCompletedSprite;
        mission.completed = true;
        completedMissions++;
    }
    
    private void CheckVictoryConditions() {
        if (completedMissions == missionListDto.missionQuantity) {
           DisplayEndGameCanvas("Mission Completed!");
        }
    }

    public void DisplayEndGameCanvas(string endText) {
        Time.timeScale = 0f;
        endGameCanvas.SetActive(true);
        endGameText.text = endText;
        missionsCompletedText.text = $"Missions completed = {completedMissions}/{missionListDto.missionQuantity}";
        var minutesSpent = Math.Truncate(Time.timeSinceLevelLoad / 60);
        var secondsSpent = Math.Round(Time.timeSinceLevelLoad % 60);
        timeSpentText.text = $"Time = {minutesSpent}:{secondsSpent}";
    }

    public void EndGame() {
        //If all missions have been completed
        if (completedMissions == missionListDto.missionQuantity) {
            //Retrieve mission availability and update current mission to completed
            var missionAvailability = JsonUtility.FromJson<bool[]>(Utils.ReadFile("missionsAvailable"));
            var currentMission = PlayerPrefs.GetInt("mission", 1);
            missionAvailability[currentMission] = true;
            //Store mission availability
            Utils.WriteFile("missionsAvailable", JsonUtility.ToJson(missionAvailability));
        }

        SceneManager.LoadScene("MissionSelection");
    }
}
