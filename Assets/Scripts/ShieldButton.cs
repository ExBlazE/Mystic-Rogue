using UnityEngine;
using UnityEngine.EventSystems;

public class ShieldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool IsShieldStarted { get; private set; } = false;
    public bool IsShieldReleased { get; private set; } = false;

    void LateUpdate()
    {
        IsShieldStarted = false;
        IsShieldReleased = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        IsShieldStarted = true;
        IsShieldReleased = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsShieldStarted = false;
        IsShieldReleased = true;
    }
}
