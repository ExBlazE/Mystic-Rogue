using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Space]
    [SerializeField] AudioSource backgroundMusic;
    [SerializeField][Range(0f, 1f)]
    private float musicVolume = 0.5f;
    [SerializeField] float musicEndFadeTime = 3f;

    [Space]
    [SerializeField] AudioSource shotStart;
    [SerializeField] AudioSource shotHit;
    [SerializeField]
    [Range(0f, 1f)]
    private float effectsVolume = 0.5f;

    [Space]
    [SerializeField] AudioClip shotStartClip;
    [SerializeField] AudioClip shotHitClip;

    public static AudioManager Instance;

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
        musicVolume = DataManager.Instance.volume;
        effectsVolume = DataManager.Instance.volume;
    }

    void Update()
    {
        backgroundMusic.volume = musicVolume;
    }

    public void StartBGM()
    {
        backgroundMusic.Play();
        backgroundMusic.volume = musicVolume;
    }

    public void EndBGM()
    {
        StartCoroutine(FadeBGM());
    }

    public void PlayShotStart()
    {
        shotStart.PlayOneShot(shotStartClip);
        shotStart.volume = effectsVolume;
    }

    public void PlayShotHit()
    {
        shotHit.PlayOneShot(shotHitClip);
        shotHit.volume = effectsVolume;
    }

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
    }
}
