using UnityEngine;
using UnityEngine.UI;

public class OreBehaviour : MonoBehaviour {
    public ResourceEnum resourceType;
    public float GATHERING_TIME_REQUIRED = 3.5f;
    public int gatheredTimes = 0;
    public int MAXGATHEREDTIMES = 15;
    public Image minimapImage;

    public void SetResourceType(ResourceEnum rType) {
        resourceType = rType;
        minimapImage.color = Constants.ORE_COLOR_MAP[resourceType];
    }
}