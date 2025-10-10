using UnityEngine;
using UnityEngine.EventSystems;

public enum ShootMode
{
    OnMaxDrag,
    OnAim
}

public class AimStick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public Vector2 Direction { get; private set; } = Vector2.zero;
    public bool IsShooting { get; private set; } = false;
    public bool IsBeingUsed { get; private set; } = false;

    ShootMode shootMode = ShootMode.OnMaxDrag;
    RectTransform aimRect;
    float maxRadius;

    void Awake()
    {
        aimRect = GetComponent<RectTransform>();
        maxRadius = aimRect.sizeDelta.x / 2f;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (shootMode == ShootMode.OnAim)
            IsShooting = true;
        IsBeingUsed = true;
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(aimRect, eventData.position, eventData.pressEventCamera, out position))
        {
            Vector2 scaledDirection = position / maxRadius;

            if (shootMode == ShootMode.OnMaxDrag)
                IsShooting = scaledDirection.magnitude > 1f ? true : false;

            Direction = scaledDirection.normalized;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsShooting = false;
        IsBeingUsed = false;
    }

    public void ChangeShootMode()
    {
        if (shootMode == ShootMode.OnMaxDrag)
            shootMode = ShootMode.OnAim;
        else if (shootMode == ShootMode.OnAim)
            shootMode= ShootMode.OnMaxDrag;
    }
}
