using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [SerializeField] GameObject orbFocus;
    [SerializeField] LayerMask targetLayers;
    [SerializeField] float dirSnapSpeed = 3f;

    [Space]
    [SerializeField] MoveStick moveStick;
    [SerializeField] AimStick aimStick;
    [SerializeField] ShieldButton shieldButton;

#if UNITY_ANDROID
    [Space]
    [SerializeField] float moveSensitivity = 3f;
    [SerializeField] bool alwaysMaxSpeed = false;
#endif

    public Vector2 MoveDirection {  get; private set; }
    public Vector3 AimDirection { get; private set; }

    public bool IsShooting { get; private set; }

    private Vector2 rawMoveInput;
    private Vector2 rawAimInput;
    private Vector2 lastMoveDirection;
    private Vector3 lastAimDirection;
    private DefaultInputActions gameInput;

    public event Action<Vector2> OnMove;
    public event Action<Vector3> OnAim;
    public event Action OnShoot;
    public event Action<bool> OnShield;

    void Awake()
    {
        gameInput = new DefaultInputActions();

        gameInput.Player.Move.performed += context => rawMoveInput = context.ReadValue<Vector2>();
        gameInput.Player.Move.canceled += _ => rawMoveInput = Vector2.zero;

        gameInput.Player.Aim.performed += context => rawAimInput = context.ReadValue<Vector2>();

        gameInput.Player.Shoot.started += _ => IsShooting = true;
        gameInput.Player.Shoot.canceled += _ => IsShooting = false;

        gameInput.Player.Shield.started += _ => OnShield?.Invoke(true);
        gameInput.Player.Shield.canceled += _ => OnShield?.Invoke(false);
    }

    void OnEnable()
    { gameInput.Enable(); }

    void OnDisable()
    { gameInput.Disable(); }

    // Update is called once per frame
    void Update()
    {
#if UNITY_STANDALONE || UNITY_WEBGL
        SetMove();
        SetAim();
        if (IsShooting)
            OnShoot?.Invoke();
#elif UNITY_ANDROID
        SetSmoothMove();
        if (aimStick.IsBeingUsed)
        {
            SetJoystickAim();
            IsShooting = aimStick.IsShooting;
        }
        else
            SetTapAim();

        IsShieldStarted = shieldButton.IsShieldStarted;
        IsShieldReleased = shieldButton.IsShieldReleased;
#endif
    }

#if UNITY_STANDALONE || UNITY_WEBGL
    void SetMove()
    {
        float currentDirSnapSpeed = rawMoveInput.sqrMagnitude > 0 ? dirSnapSpeed * 2 : dirSnapSpeed;
        MoveDirection = Vector2.MoveTowards(MoveDirection, rawMoveInput, currentDirSnapSpeed * Time.deltaTime);

        if (lastMoveDirection == Vector2.zero && MoveDirection == Vector2.zero)
            return;

        OnMove?.Invoke(MoveDirection);
        lastMoveDirection = MoveDirection;
    }

    void SetAim()
    {
        // Draw a ray from camera through the mouse pointer
        Vector3 mousePosOnGame = Camera.main.ScreenToViewportPoint(rawAimInput);
        Ray ray = Camera.main.ViewportPointToRay(mousePosOnGame);
        RaycastHit hit;

        // Cast the ray until it hits an object in the selected layers
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayers))
        {
            // Get mouse position on ground level and direction from player
            Vector3 mousePosition = hit.point;
            AimDirection = mousePosition - orbFocus.transform.position;

            if (lastAimDirection == AimDirection)
                return;

            OnAim?.Invoke(AimDirection);
            lastAimDirection = AimDirection;
        }
    }
#elif UNITY_ANDROID
    void SetSmoothMove()
    {
        float maxMovePerFrame = Time.deltaTime * moveSensitivity;
        Vector2 newDirection;
        if (alwaysMaxSpeed)
            newDirection = moveStick.Direction.normalized;
        else
            newDirection = moveStick.Direction;
        float newX = Mathf.MoveTowards(MoveDirection.x, newDirection.x, maxMovePerFrame);
        float newY = Mathf.MoveTowards(MoveDirection.y, newDirection.y, maxMovePerFrame);
        MoveDirection = new Vector2 (newX, newY);
    }

    void SetJoystickAim()
    {
        float xDirection = aimStick.Direction.x;
        float yDirection = aimStick.Direction.y;
        AimDirection = new Vector3 (xDirection, 0, yDirection);
    }

    void SetTapAim()
    {
        if (Input.touchCount > 0)
        {
            Touch touch;
            for (int i = 0; i < Input.touchCount; i++)
            {
                touch = Input.GetTouch(i);
                if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId) && touch.fingerId != moveStick.pointerId)
                {
                    if (touch.phase != TouchPhase.Ended)
                    {
                        Ray ray = Camera.main.ScreenPointToRay(touch.position);
                        RaycastHit hit;

                        // Cast the ray until it hits an object in the selected layers
                        if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayers))
                        {
                            Vector3 touchPosition = hit.point;
                            AimDirection = touchPosition - orbFocus.transform.position;
                            IsShooting = true;
                        }
                    }
                    else
                        IsShooting = false;

                    return;
                }
            }
        }
        else
            IsShooting = false;
    }
#endif
}
