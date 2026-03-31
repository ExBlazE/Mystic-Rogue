using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("BGM")]
    [SerializeField] AudioSource backgroundMusic;
    [SerializeField] float musicEndFadeTime = 3f;
    [SerializeField][Range(0, 1)]
    float musicVolume = 0.5f;

    [Header("Effects")]
    [SerializeField] AudioSource shotStart;
    [SerializeField] AudioSource shotHit;
    [SerializeField] AudioSource shield;
    [SerializeField][Range(0, 1)]
    float effectsVolume = 0.5f;

    [Header("Clips to use")]
    [SerializeField] AudioClip shotStartClip;
    [SerializeField] AudioClip shotHitClip;
    [SerializeField] AudioClip shieldClip;

    public static AudioManager Instance;

    void OnEnable()
    {
        GameEvents.OnShotFired += PlayShotStart;
        GameEvents.OnShotHit += PlayShotHit;
        GameEvents.OnShieldAppear += PlayShield;
    }

    void OnDisable()
    {
        GameEvents.OnShotFired -= PlayShotStart;
        GameEvents.OnShotHit -= PlayShotHit;
        GameEvents.OnShieldAppear -= PlayShield;
    }

    void Awake()
    {
        // Create a singleton instance for this script
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // If started game from main menu, get initial volumes
        if (DataManager.Instance != null)
        {
            musicVolume = DataManager.Instance.musicVolume;
            effectsVolume = DataManager.Instance.effectsVolume;
        }
    }

    void Update()
    {
        // Change music volume in real time
        if (GameManager.Instance.isGameActive)
            backgroundMusic.volume = musicVolume;
    }

    public void StartBGM()
    {
        backgroundMusic.Play();
        backgroundMusic.volume = musicVolume;
    }

    public void PauseBGM()
    {
        backgroundMusic.Pause();
    }

    public void UnPauseBGM()
    {
        backgroundMusic.UnPause();
    }

    public void EndBGM()
    {
        StartCoroutine(FadeBGM());
    }

    // Invoked when shooting a projectile
    public void PlayShotStart()
    {
        shotStart.PlayOneShot(shotStartClip);
        shotStart.volume = effectsVolume;
    }

    // Invoked when a projectile hits something
    // Ignore parameters
    public void PlayShotHit(Vector3 _, Quaternion __, Target ___)
    {
        shotHit.PlayOneShot(shotHitClip);
        shotHit.volume = effectsVolume;
    }

    // Invoked when activating shield
    public void PlayShield()
    {
        shield.PlayOneShot(shieldClip);
        shield.volume = effectsVolume;
    }

    // Method for use by music volume slider
    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
    }

    // Method for use by effects volume slider
    public void SetEffectsVolume(float volume)
    {
        effectsVolume = volume;
    }

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
