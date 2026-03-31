using System.Collections;
using UnityEngine;

public class Shield : MonoBehaviour, IDamageable
{
    public bool IsPlayerSide => true;
    public Target TargetType => Target.Shield;

    [SerializeField] Player player;
    [SerializeField] float fadeDuration = 0.15f;

    public bool isShieldActive { get; private set; }

    private bool fading = false;
    private bool fadeOutQueued = false;
    private Collider shieldCollider;
    private Renderer shieldRenderer;

    private const string shieldAlphaName = "_Alpha";

    void Awake()
    {
        // Get attached components
        shieldCollider = GetComponent<Collider>();
        shieldRenderer = GetComponent<Renderer>();
    }

    void Start()
    {
        // Disable shield by default at start
        isShieldActive = false;
        shieldCollider.enabled = false;
        shieldRenderer.material.SetFloat(shieldAlphaName, 0f);
    }

    public void OnHit(float damage)
    {
        player.ModifyEnergy(-damage);
    }

    public void Toggle(bool shieldState)
    {
        if (shieldState)
        {
            if (player.energy > 0)
            {
                StartCoroutine(ToggleShield(shieldState));
                GameEvents.ShieldAppear();
            }
            else
                return;
        }

        else 
            StartCoroutine(ToggleShield(shieldState));
    }

    // Coroutine to enable or disable shield
    IEnumerator ToggleShield(bool newState)
    {
        // This block is meant to queue up a single fade-out after a fade-in
        // Runs only if right-click is released too quickly
        if (fading)
        {
            if (!fadeOutQueued)
            {
                fadeOutQueued = true;
                while (fading)
                    yield return null;
            }
            else
                yield break;
        }

        // Set flag to indicate shield is fading
        fading = true;

        float startAlpha;
        float endAlpha;

        float timeElapsed = 0f;

        // Set start and end values of alpha for fading in
        if (newState)
        {
            startAlpha = 0f;
            endAlpha = 1f;

            // Activate the shield game object
            isShieldActive = true;
            shieldCollider.enabled = true;
            shieldRenderer.enabled = true;
        }

        // Set start and end values of alpha for fading out
        // We don't deactivate shield here because it's done after the fade out animation
        else
        {
            startAlpha = 1f;
            endAlpha = 0f;
        }

        // Set gradually increasing or decrease alpha value per frame until end value
        while (timeElapsed < fadeDuration)
        {
            // Calculate the new alpha value for this frame
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, timeElapsed / fadeDuration);

            // Clamp the min and max values of alpha
            if (newAlpha < 0f)
                newAlpha = 0f;
            else if (newAlpha > 1f)
                newAlpha = 1f;

            // Set the new alpha value for this frame
            shieldRenderer.material.SetFloat(shieldAlphaName, newAlpha);

            // Increase progress indicator of the fade for next rerun of the loop
            timeElapsed += Time.deltaTime;

            // Pause the coroutine until next frame
            yield return null;
        }

        // If fading out, deactivate shield object and reset queue flag
        if (!newState)
        {
            isShieldActive = false;
            shieldCollider.enabled = false;
            shieldRenderer.enabled = false;

            fadeOutQueued = false;
        }

        // Set flag to indicate that fading is complete
        fading = false;
    }
}
