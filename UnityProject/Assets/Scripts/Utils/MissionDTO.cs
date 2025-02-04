using System;
using UnityEngine.UI;

[Serializable]
public class MissionDTO {
    public MissionTypeEnum missionType;
    public string objectiveName;
    public int objectiveQuantity;
    public string missionDescription;
    public bool completed;
}