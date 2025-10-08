using UnityEngine;
using UnityEngine.EventSystems;

public class AimStick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public Vector2 Direction { get; private set; } = Vector2.zero;
    public bool IsShooting { get; private set; } = false;

    RectTransform aimRect;
    float maxRadius;

    void Awake()
    {
        aimRect = GetComponent<RectTransform>();
        maxRadius = aimRect.sizeDelta.x / 2f;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(aimRect, eventData.position, eventData.pressEventCamera, out position))
        {
            Vector2 scaledDirection = position / maxRadius;

            if (scaledDirection.magnitude > 1f)
                IsShooting = true;
            else
                IsShooting = false;

            Direction = scaledDirection.normalized;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsShooting = false;
    }
}
