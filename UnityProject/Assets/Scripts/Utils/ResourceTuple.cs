using UnityEngine;

public class ResourceTuple {
    public GameObject gameObject;
    public bool isBeingGathered;

    public ResourceTuple(GameObject gameObject, bool isBeingGathered) {
        this.gameObject = gameObject;
        this.isBeingGathered = isBeingGathered;
    }
}
