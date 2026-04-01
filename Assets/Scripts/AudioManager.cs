using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("BGM")]
    [SerializeField] AudioSource backgroundMusic;
    [SerializeField] float musicEndFadeTime = 3f;

    [Header("Effects")]
    [SerializeField] AudioSource shotStart;
    [SerializeField] AudioSource shotHit;
    [SerializeField] AudioSource shield;

    [Header("Clips to use")]
    [SerializeField] AudioClip shotStartClip;
    [SerializeField] AudioClip shotHitClip;
    [SerializeField] AudioClip shieldClip;

    void OnEnable()
    {
        GameEvents.OnShotFired += PlayShotStart;
        GameEvents.OnShotHit += PlayShotHit;
        GameEvents.OnShieldAppear += PlayShield;

        GameEvents.OnGameStart += StartBGM;
        GameEvents.OnGamePause += PauseBGM;
        GameEvents.OnGameResume += UnPauseBGM;
        GameEvents.OnPlayerDeath += EndBGM;
    }

    void OnDisable()
    {
        GameEvents.OnShotFired -= PlayShotStart;
        GameEvents.OnShotHit -= PlayShotHit;
        GameEvents.OnShieldAppear -= PlayShield;

        GameEvents.OnGameStart -= StartBGM;
        GameEvents.OnGamePause -= PauseBGM;
        GameEvents.OnGameResume -= UnPauseBGM;
        GameEvents.OnPlayerDeath -= EndBGM;
    }

    void Start()
    {
        if (DataManager.Instance != null)
        {
            Globals.musicVolume = 0.5f;
            Globals.effectsVolume = 0.5f;
        }
    }

    public void StartBGM()
    {
        backgroundMusic.Play();
        backgroundMusic.volume = Globals.musicVolume;
    }

    public void PauseBGM()
    { backgroundMusic.Pause(); }

    public void UnPauseBGM()
    { backgroundMusic.UnPause(); }

    public void EndBGM()
    { StartCoroutine(FadeBGM()); }

    // Invoked when shooting a projectile
    public void PlayShotStart()
    {
        shotStart.PlayOneShot(shotStartClip);
        shotStart.volume = Globals.effectsVolume;
    }

    // Invoked when a projectile hits something
    // Ignore parameters
    public void PlayShotHit(Vector3 _, Quaternion __, Target ___)
    {
        shotHit.PlayOneShot(shotHitClip);
        shotHit.volume = Globals.effectsVolume;
    }

    // Invoked when activating shield
    public void PlayShield()
    {
        shield.PlayOneShot(shieldClip);
        shield.volume = Globals.effectsVolume;
    }

    // Method for use by music volume slider. Assign in inspector with Dynamic Float.
    public void SetMusicVolume(float volume)
    { 
        Globals.musicVolume = volume;
        backgroundMusic.volume = volume;
    }

    // Method for use by effects volume slider. Assign in inspector with Dynamic Float.
    public void SetEffectsVolume(float volume)
    { Globals.effectsVolume = volume; }

    // Fade out the BGM over given duration. To be called on game over.
    IEnumerator FadeBGM()
    {
        float startVolume = backgroundMusic.volume;
        float timeElapsed = 0;

        while (timeElapsed < musicEndFadeTime)
        {
            backgroundMusic.volume = Mathf.Lerp(startVolume, 0f, timeElapsed / musicEndFadeTime);
            timeElapsed += Time.unscaledDeltaTime;

            yield return null;
        }

        backgroundMusic.volume = 0f;
        backgroundMusic.Stop();
    }
}
