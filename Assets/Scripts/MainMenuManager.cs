using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages all UI behaviour in the main menu.
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI highScoreText;
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider effectsVolumeSlider;
    [SerializeField] Button exitButton;
    [SerializeField] GameObject creditsScreen;
    [SerializeField] GameObject frameRateScreen;

    void Start()
    {
        // Load saved high score and settings
        DataManager.Instance.LoadHighScore();
        DataManager.Instance.LoadSettings();

        musicVolumeSlider.value = Globals.MusicVolume;
        effectsVolumeSlider.value = Globals.EffectsVolume;
        SetFrameRate(Globals.FrameRateLevel);

        // Check to prevent a high score from displaying zero
        if (Globals.HighScore > 0)
            highScoreText.SetText(Globals.HighScore.ToString("D2"));
        else
            highScoreText.gameObject.SetActive(false);

        // Hide Exit button if on WebGL
        if (Application.platform == RuntimePlatform.WebGLPlayer)
            exitButton.gameObject.SetActive(false);
    }

    // To be used by start button in Inspector
    public void StartButton()
    {
        DataManager.Instance.SaveSettings();
        SceneManager.LoadScene(1);
    }

    // To be used by exit button in Inspector
    public void ExitButton()
    {
        DataManager.Instance.SaveSettings();
        
#if UNITY_EDITOR
        // Conditional compilation code written to test exit button in the editor
        UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    // To be used by credits button in Inspector
    public void ToggleCredits()
    {
        creditsScreen.SetActive(!creditsScreen.activeSelf);
    }

    // To be used by main menu music volume slider in Inspector via Dynamic Float
    public void SetMusicVolume(float volume)
    {
        Globals.MusicVolume = volume;
    }

    // To be used by main menu effects volume slider in Inspector via Dynamic Float
    public void SetEffectsVolume(float volume)
    {
        Globals.EffectsVolume = volume;
    }

    public void ToggleFrameRateScreen()
    {
        frameRateScreen.SetActive(!frameRateScreen.activeSelf);
    }

    public void SetFrameRate(int level)
    {
        switch (level)
        {
            case 0:
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = 30;
                Globals.FrameRateLevel = level;
                break;

            case 1:
                QualitySettings.vSyncCount = 2;
                Application.targetFrameRate = -1;
                if (Application.isMobilePlatform)
                {
                    QualitySettings.vSyncCount = 0;
                    Application.targetFrameRate = ((int)Screen.currentResolution.refreshRateRatio.value) / 2;
                }
                Globals.FrameRateLevel = level;
                break;

            case 2:
                QualitySettings.vSyncCount = 1;
                Application.targetFrameRate = -1;
                if (Application.isMobilePlatform)
                {
                    QualitySettings.vSyncCount = 0;
                    Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;
                }
                Globals.FrameRateLevel = level;
                break;

            case 3:
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = -1;
                Globals.FrameRateLevel = level;
                break;

            default:
                break;
        }
    }
}
