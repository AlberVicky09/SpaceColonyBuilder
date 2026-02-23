using System;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MissionSelectionManager : MonoBehaviour {
    public static MissionSelectionManager Instance;
    
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
    private bool isAlradyInMission;

    void Start() {
        AudioManager.Instance.SetMusic(MusicTrackNamesEnum.MissionSelectionBG);

        Instance = this;
        
        titleTexts = new string[missionPositions.Length];
        descriptionTexts = new string[missionPositions.Length];
        missionsAvailable = new bool[missionPositions.Length];
        
        
        //Retrieve completed missions from file
        MissionAvailabilityDTO missionAvailability;
        try {
            missionAvailability = JsonUtility.FromJson<MissionAvailabilityDTO>(Utils.ReadFile("missionsAvailable"));
        } catch {
            missionAvailability = new MissionAvailabilityDTO(new []{true, false, false});
        }
        
        var missionTextFile = Resources.Load<TextAsset>("missionDescriptions");
        var missionTexts = JsonUtility.FromJson<MissionDescriptionListDTO>(missionTextFile.text);
        //var missionTexts = JsonUtility.FromJson<MissionDescriptionListDTO>(Utils.ReadFile("missionDescriptions"));
        //Retrieve all mission descriptions from file
        for (int i = 0; i < missionPositions.Length; i++) {
            missionsAvailable[i] = missionAvailability.boolArray[i];
            missionPositions[i].GetComponent<Renderer>().material.color = missionsAvailable[i] ? missionAvailableColor : missionNotAvailableColor;
            titleTexts[i] = missionTexts.missionDescriptions[i].missionTitle;
            descriptionTexts[i] = missionTexts.missionDescriptions[i].missionDescription;
        }

        //If latest is true, its because the demo has been completed
        if (missionAvailability.boolArray[2]) {
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
            // Check if the agent has reached its destination
            if (Utils.HasAgentArrivedOrItsStuck(agent)) {
                MoveToMission(objectiveMission);
            }
        }
    }

    public void MoveToMission(int mission) {
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
