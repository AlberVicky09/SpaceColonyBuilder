using System;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MissionSelectionManager : MonoBehaviour {
    public static MissionSelectionManager Instance;
    
    public NavMeshAgent agent;
    private int currentPosition = -1;
    private int objectiveMission;
    public GameObject[] missionPositions;
    public TMP_Text titleText, descriptionText;
    public Button startMissionBtn;
    private String[] titleTexts, descriptionTexts;
    public Boolean[] missionsAvailable;
    public Color missionAvailableColor, missionNotAvailableColor;

    void Start() {

        Instance = this;
        
        titleTexts = new string[missionPositions.Length];
        descriptionTexts = new string[missionPositions.Length];
        missionsAvailable = new bool[missionPositions.Length];
        
        //Retrieve completed missions from file
        var missionAvailability = JsonUtility.FromJson<MissionAvailabilityDTO>(Utils.ReadFile("missionsAvailable"));
        var missionTexts = JsonUtility.FromJson<MissionDescriptionListDTO>(Utils.ReadFile("missionDescriptions"));
        //Retrieve all mission descriptions from file
        for (int i = 0; i < missionPositions.Length; i++) {
            var missionText = 
            missionsAvailable[i] = missionAvailability.boolArray[i];
            missionPositions[i].GetComponent<Renderer>().material.color = missionsAvailable[i] ? missionAvailableColor : missionNotAvailableColor;
            titleTexts[i] = missionTexts.missionDescriptions[i].missionTitle;
            descriptionTexts[i] = missionTexts.missionDescriptions[i].missionDescription;
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
            agent.SetDestination(missionPositions[++currentPosition].transform.position);
        }else if (objectiveMission < currentPosition) {
            agent.SetDestination(missionPositions[--currentPosition].transform.position);
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

    public void ReturnToMenu() {
        SceneManager.LoadScene("MainMenu");
    }
}
