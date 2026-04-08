using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Translates all signals from the Input System to events for other systems to listen to.<br/>
/// <i>MANDATORY for every scene with controllable player.</i>
/// </summary>
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

    void Update()
    {
        SetMove();
        if (IsShooting)
            InputEvents.RaiseOnShoot();
    }

    // Saves the directional input data to a variable
    void HandleMove(InputAction.CallbackContext context)
    {
        if (context.performed)
            rawMoveInput = context.ReadValue<Vector2>();
        else if (context.canceled)
            rawMoveInput = Vector2.zero;
    }

    // Smooths the snap movement data provided by the input into a gradual speed-up, slow-down movement
    // Because it's a process that happens even when there isn't immediate input, this needs to be in Update()
    void SetMove()
    {
        // Smooth movement logic
        float currentDirSnapSpeed = rawMoveInput.sqrMagnitude > 0 ? dirSnapSpeed * 2 : dirSnapSpeed;
        moveDirection = Vector2.MoveTowards(moveDirection, rawMoveInput, currentDirSnapSpeed * Time.deltaTime);

        // Guard against movement event being fired even when no input is detected and all smoothing is finished
        if (lastMoveDirection == Vector2.zero && moveDirection == Vector2.zero)
            return;

        InputEvents.RaiseOnMove(moveDirection);
        lastMoveDirection = moveDirection;
    }

    // Extract the Vector2 data and the control device and call SetAim
    void HandleAim(InputAction.CallbackContext context)
    {
        Vector2 rawAimInput = context.ReadValue<Vector2>();
        SetAim(rawAimInput, context.control.device);
        if (context.control.device is Gamepad && context.canceled)
            IsShooting = false;
    }

    // Distinguishes between input device and sets aim direction accordingly
    // Player object reference required for mouse input aim direction calculations
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
            InputEvents.RaiseOnAim(aimDirection);
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
            InputEvents.RaiseOnShield(true);
        else if (context.canceled)
            InputEvents.RaiseOnShield(false);
    }

    void HandlePauseRequest(InputAction.CallbackContext _)
    { InputEvents.RaiseOnPauseRequest(); }
}
