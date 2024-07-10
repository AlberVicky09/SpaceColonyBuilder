using System;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MissionSelectionManager : MonoBehaviour
{
    public NavMeshAgent agent;
    private int currentPosition = -1;
    private int objectiveMission;
    public Transform[] missionPositions;
    public TMP_Text titleText, descriptionText;
    public Button startMissionBtn;
    private String[] titleTexts, descriptionTexts;

    void Start() {
        titleTexts = new string[missionPositions.Length];
        descriptionTexts = new string[missionPositions.Length];
        for (int i = 0; i < missionPositions.Length; i++) {
            var missionText = Utils.ReadFile("missionDescription" + i);
            titleTexts[i] = missionText[0];
            descriptionTexts[i] = missionText[1];
        }
    }
    void Update()
    {
        // Check if the agent has reached its destination
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                MoveToMission(objectiveMission);
            }
        }
    }

    public void MoveToMission(int mission) {
        objectiveMission = mission;
        if (objectiveMission > currentPosition) {
            agent.SetDestination(missionPositions[++currentPosition].position);
        }else if (objectiveMission < currentPosition) {
            agent.SetDestination(missionPositions[--currentPosition].position);
        }
    }

    public void DisplayCurrentMission(int mission) {
        titleText.text = titleTexts[mission];
        descriptionText.text = descriptionTexts[mission];
        startMissionBtn.onClick.RemoveAllListeners();
        startMissionBtn.onClick.AddListener(() => { PlayMission(mission); });
    }
    
    public void PlayMission(int mission) {
        PlayerPrefs.SetInt("mission", mission);
        SceneManager.LoadScene("MainScene");
    }
}
