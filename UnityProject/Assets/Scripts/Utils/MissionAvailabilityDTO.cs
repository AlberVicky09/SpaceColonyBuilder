using System;

[Serializable]
public class MissionAvailabilityDTO {
    public bool[] boolArray;

    public MissionAvailabilityDTO(bool[] boolArray) {
        this.boolArray = boolArray;
    }

    public MissionAvailabilityDTO() {
        boolArray = new bool[3];
    }
}