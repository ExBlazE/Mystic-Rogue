using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [SerializeField] GameObject orbFocus;
    [SerializeField] LayerMask targetLayers;

    [Space]
    [SerializeField] MoveStick moveStick;
    [SerializeField] AimStick aimStick;
    [SerializeField] ShieldButton shieldButton;

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
#elif UNITY_ANDROID
    float moveSensitivity = 3f;
#endif

    public Vector2 MoveDirection {  get; private set; }
    public Vector3 AimDirection { get; private set; }

    public bool IsShooting { get; private set; }
    public bool IsShieldStarted { get; private set; }
    public bool IsShieldReleased { get; private set; }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
        SetMove();
        SetAim();
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
            IsShooting = true;
        else
            IsShooting = false;
        
        if (Input.GetKeyDown(KeyCode.Mouse1))
            IsShieldStarted = true;

        if (Input.GetKeyUp(KeyCode.Mouse1))
            IsShieldReleased = true;
#elif UNITY_ANDROID
        SetSmoothMove();
        SetSmoothAim();

        IsShooting = aimStick.IsShooting;

        IsShieldStarted = shieldButton.IsShieldStarted;
        IsShieldReleased = shieldButton.IsShieldReleased;
#endif
    }

    void LateUpdate()
    {
        if (IsShieldStarted)
            IsShieldStarted = false;
        if (IsShieldReleased)
            IsShieldReleased = false;
    }

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
    void SetMove()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        MoveDirection = new Vector2(horizontal, vertical);
    }

    void SetAim()
    {
        // Draw a ray from camera through the mouse pointer
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Cast the ray until it hits an object in the selected layers
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayers))
        {
            // Get mouse position on ground level and direction from player
            Vector3 mousePosition = hit.point;
            AimDirection = mousePosition - orbFocus.transform.position;
        }
    }
#elif UNITY_ANDROID
    void SetSmoothMove()
    {
        float maxMovePerFrame = Time.deltaTime * moveSensitivity;
        float newX = Mathf.MoveTowards(MoveDirection.x, moveStick.Direction.x, maxMovePerFrame);
        float newY = Mathf.MoveTowards(MoveDirection.y, moveStick.Direction.y, maxMovePerFrame);
        MoveDirection = new Vector2 (newX, newY);
    }

    void SetSmoothAim()
    {
        float xDirection = aimStick.Direction.x;
        float yDirection = aimStick.Direction.y;
        AimDirection = new Vector3 (xDirection, 0, yDirection);
    }
#endif
}
