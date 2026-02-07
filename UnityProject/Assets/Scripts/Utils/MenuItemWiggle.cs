using UnityEngine;
using UnityEngine.EventSystems;

public class MenuItemWiggle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler {

    private bool isHovering;
    public int wiggleDirection = 1;
    public bool isButtonCompletelyFronted = true;

    private Quaternion startingRotation;
    private float currentAngle;

    void Awake() {
        startingRotation = transform.localRotation;
    }

    void OnEnable() {
        ResetWiggle();
    }

    void OnDisable() {
        ResetWiggle();
    }

    void Update() {
        if (!isHovering)
            return;

        currentAngle += Constants.MENU_ITEM_WIGGLE_SPEED
                        * wiggleDirection
                        * Time.unscaledDeltaTime;

        if (Mathf.Abs(currentAngle) > Constants.MENU_ITEM_WIGGLE_DISPLACEMENT) {
            wiggleDirection *= -1;
            currentAngle = Mathf.Clamp(
                currentAngle,
                -Constants.MENU_ITEM_WIGGLE_DISPLACEMENT,
                Constants.MENU_ITEM_WIGGLE_DISPLACEMENT
            );
        }

        transform.localRotation =
            startingRotation *
            (isButtonCompletelyFronted ?
                Quaternion.Euler(0f, 0f, currentAngle) :
                Quaternion.AngleAxis(currentAngle, transform.forward));
    }

    public void OnPointerEnter(PointerEventData eventData) {
        isHovering = true;
        AudioManager.Instance.PlaySfx(SfxTrackNamesEnum.OnHoverMenu);
    }

    public void OnPointerExit(PointerEventData eventData) {
        ResetWiggle();
    }

    public void OnPointerDown(PointerEventData eventData) {
        AudioManager.Instance.PlaySfx(SfxTrackNamesEnum.OnClickMenu);
        ResetWiggle();
    }

    private void ResetWiggle() {
        isHovering = false;
        currentAngle = 0f;
        transform.localRotation = startingRotation;
    }
}