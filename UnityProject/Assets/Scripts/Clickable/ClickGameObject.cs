using UnityEngine;

public class ClickGameObject : MonoBehaviour
{
    [SerializeField] Camera cameraGO;
    private int clickableLayer = 1 << 3;

    // Update is called once per frame
    void Update()
    {
        DetectObjectWithRaycast();
    }

    public void DetectObjectWithRaycast()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cameraGO.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, clickableLayer))
            {
                hit.collider.gameObject.GetComponent<Clickable>().OnClick();
            }
        }
    }
}
