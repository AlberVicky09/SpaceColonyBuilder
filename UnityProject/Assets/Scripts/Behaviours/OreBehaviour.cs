using UnityEngine;

public class OreBehaviour : MonoBehaviour
{
    public ResourceEnum resourceType;
    public float gatheringTimeRequired = 3.5f;
    public int gatheredTimes = 0;
    public int MAXGATHEREDTIMES = 15;
}
