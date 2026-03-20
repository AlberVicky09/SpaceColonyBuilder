using System;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MissionSelectionManager : MonoBehaviour {
    
    public NavMeshAgent agent;
    public GameObject[] missionPositions;
    public TMP_Text titleText, descriptionText;
    public Button startMissionBtn;
    public Boolean[] missionsAvailable;
    public Color missionAvailableColor, missionNotAvailableColor;
    public GameObject demoCompletedCanvas;
    public Sprite startMissionActivatedSprite, startMissionDeactivatedSprite;

    private Image startMissionBtnImage;
    private int currentPosition;
    private int objectiveMission;
    private String[] titleTexts, descriptionTexts;
    public bool isAlradyInMission;
    private float timeSinceStart;
    
    void Start() {
        AudioManager.Instance.SetMusic(MusicTrackNamesEnum.MissionSelectionBG);
        
        titleTexts = new string[missionPositions.Length];
        descriptionTexts = new string[missionPositions.Length];
        missionsAvailable = new bool[missionPositions.Length];
        
        //var missionTexts = JsonUtility.FromJson<MissionDescriptionListDTO>(Utils.ReadFile("missionDescriptions"));
        //Retrieve all mission descriptions from file
        for (int i = 0; i < missionPositions.Length; i++) {
            missionsAvailable[i] = MissionInformationController.Instance.missionAvailability.boolArray[i];
            missionPositions[i].GetComponent<Renderer>().material.color = missionsAvailable[i] ? missionAvailableColor : missionNotAvailableColor;
            titleTexts[i] = MissionInformationController.Instance.missionDescriptionList.missionDescriptions[i].missionTitle;
            descriptionTexts[i] = MissionInformationController.Instance.missionDescriptionList.missionDescriptions[i].missionDescription;
        }

        //If latest is true, its because the demo has been completed
        if (MissionInformationController.Instance.missionAvailability.boolArray[3]) {
            demoCompletedCanvas.SetActive(true);
        }

        startMissionBtnImage = startMissionBtn.GetComponent<Image>();
        startMissionBtnImage.sprite = startMissionDeactivatedSprite;

        currentPosition = -1;
        objectiveMission = 0;
        isAlradyInMission = false;
        agent.SetDestination(missionPositions[objectiveMission].transform.position);
    }
    
    void Update() {
        if (!isAlradyInMission) {
            if (timeSinceStart > Constants.TIME_TO_AVOID_AGENT_STUCK) {
                // Check if the agent has reached its destination
                if (Utils.HasAgentArrivedOrItsStuck(agent)) {
                    MoveToMission(objectiveMission);
                }
            } else {
                timeSinceStart += Time.deltaTime;
            }
        }
    }

    public void MoveToMission(int mission) {
        timeSinceStart = 0f;
        objectiveMission = mission;
        if (objectiveMission > currentPosition) {
            agent.SetDestination(missionPositions[++currentPosition].transform.position);
            ActivateInMission(false);
        }else if (objectiveMission < currentPosition) {
            agent.SetDestination(missionPositions[--currentPosition].transform.position);
            ActivateInMission(false);
        } else { ActivateInMission(true); }
    }

    public void DisplayCurrentMission(int mission) {
        titleText.text = titleTexts[mission];
        descriptionText.text = descriptionTexts[mission];
        startMissionBtn.onClick.RemoveAllListeners();
        startMissionBtn.onClick.AddListener(() => { PlayMission(mission); });
    }
    
    public void PlayMission(int mission) {
        PlayerPrefs.SetInt("mission", mission);
        MissionInformationController.Instance.UpdateSavedFile();
        StartCoroutine(AudioManager.Instance.UpdateScene(1.25f, "MainScene"));
    }

    public void ReturnToMenu() {
        StartCoroutine(AudioManager.Instance.UpdateScene(1.25f, "MainMenu"));
    }

    private void ActivateInMission(bool activated) {
        isAlradyInMission = activated;
        startMissionBtnImage.sprite = activated ? startMissionActivatedSprite : startMissionDeactivatedSprite;
        startMissionBtn.interactable = activated;
    }
}
