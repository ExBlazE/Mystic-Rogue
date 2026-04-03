using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [SerializeField] GameObject orbFocus;
    [SerializeField] LayerMask targetLayers;
    [SerializeField] float dirSnapSpeed = 3f;

    private Vector2 moveDirection;
    private Vector3 aimDirection;
    private bool IsShooting;

    private Vector2 rawMoveInput;
    private Vector2 lastMoveDirection;
    private DefaultInputActions gameInput;

    public event Action<Vector2> OnMove;
    public event Action<Vector3> OnAim;
    public event Action OnShoot;
    public event Action<bool> OnShield;

    void Awake()
    { gameInput = new DefaultInputActions(); }

    void OnEnable()
    {
        gameInput.Enable();

        gameInput.Player.Move.performed += HandleMove;
        gameInput.Player.Move.canceled += HandleMove;

        gameInput.Player.Aim.performed += HandleAim;
        gameInput.Player.Aim.canceled += HandleAim;

        gameInput.Player.Shoot.started += HandleShoot;
        gameInput.Player.Shoot.canceled += HandleShoot;

        gameInput.Player.Shield.started += HandleShield;
        gameInput.Player.Shield.canceled += HandleShield;

        gameInput.UI.Pause.performed += HandlePauseRequest;
    }

    void OnDisable()
    {
        gameInput.Player.Move.performed -= HandleMove;
        gameInput.Player.Move.canceled -= HandleMove;

        gameInput.Player.Aim.performed -= HandleAim;
        gameInput.Player.Aim.canceled -= HandleAim;

        gameInput.Player.Shoot.started -= HandleShoot;
        gameInput.Player.Shoot.canceled -= HandleShoot;

        gameInput.Player.Shield.started -= HandleShield;
        gameInput.Player.Shield.canceled -= HandleShield;

        gameInput.UI.Pause.performed -= HandlePauseRequest;

        gameInput.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        SetMove();
        if (IsShooting)
            OnShoot?.Invoke();
    }

    void HandleMove(InputAction.CallbackContext context)
    {
        if (context.performed)
            rawMoveInput = context.ReadValue<Vector2>();
        else if (context.canceled)
            rawMoveInput = Vector2.zero;
    }

    void SetMove()
    {
        // Smooth movement logic
        float currentDirSnapSpeed = rawMoveInput.sqrMagnitude > 0 ? dirSnapSpeed * 2 : dirSnapSpeed;
        moveDirection = Vector2.MoveTowards(moveDirection, rawMoveInput, currentDirSnapSpeed * Time.deltaTime);

        if (lastMoveDirection == Vector2.zero && moveDirection == Vector2.zero)
            return;

        OnMove?.Invoke(moveDirection);
        lastMoveDirection = moveDirection;
    }

    void HandleAim(InputAction.CallbackContext context)
    {
        Vector2 rawAimInput = context.ReadValue<Vector2>();
        SetAim(rawAimInput, context.control.device);
        if (context.control.device is Gamepad && context.canceled)
            IsShooting = false;
    }

    void SetAim(Vector2 rawAimInput, InputDevice device)
    {
        if (device is Mouse)
        {
            // Draw a ray from camera through the mouse pointer
            Ray ray = Camera.main.ScreenPointToRay(rawAimInput);
            RaycastHit hit;

            // Cast the ray until it hits an object in the selected layers
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayers))
            {
                // Get mouse position on ground level and direction from player
                Vector3 mousePosition = hit.point;
                aimDirection = mousePosition - orbFocus.transform.position;
            }
        }
        
        else if (device is Gamepad)
        {
            IsShooting = rawAimInput.sqrMagnitude > 0.9f ? true : false;
            aimDirection = new Vector3(rawAimInput.x, 0, rawAimInput.y);
        }
        
        if (aimDirection != Vector3.zero)
        {
            aimDirection.Normalize();
            OnAim?.Invoke(aimDirection);
        }
    }

    void HandleShoot(InputAction.CallbackContext context)
    {
        if (context.started)
            IsShooting = true;
        else if (context.canceled)
            IsShooting = false;
    }

    void HandleShield(InputAction.CallbackContext context)
    {
        if (context.started)
            OnShield?.Invoke(true);
        else if (context.canceled)
            OnShield?.Invoke(false);
    }

    void HandlePauseRequest(InputAction.CallbackContext _)
    { GameEvents.RaiseOnPauseRequest(); }
}
