using UnityEngine;
using UnityEngine.EventSystems;

public class MoveStick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public Vector2 Direction { get; private set; } = Vector2.zero;

    RectTransform moveRect;
    float maxRadius;
    float maxArea = 0.75f;

    void Awake()
    {
        moveRect = GetComponent<RectTransform>();
        maxRadius = moveRect.sizeDelta.x / 2f;
        maxRadius *= maxArea;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(moveRect, eventData.position, eventData.pressEventCamera, out position))
        {
            Vector2 scaledDirection = position / maxRadius;
            Direction = Vector2.ClampMagnitude(scaledDirection, 1f);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Direction = Vector2.zero;
    }
}
