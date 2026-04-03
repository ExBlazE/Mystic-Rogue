using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI highScoreText;
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider effectsVolumeSlider;
    [SerializeField] Button exitButton;
    [SerializeField] GameObject creditsScreen;

    void Start()
    {
        // Load saved high score and volumes
        DataManager.Instance.LoadHighScore();
        DataManager.Instance.LoadSettings();

        musicVolumeSlider.value = Globals.musicVolume;
        effectsVolumeSlider.value = Globals.effectsVolume;

        if (Globals.highScore > 0)
            highScoreText.SetText(Globals.highScore.ToString("D2"));
        else
            highScoreText.gameObject.SetActive(false);

#if UNITY_WEBGL
        // On WebGL build, hide the exit button
        exitButton.gameObject.SetActive(false);
#endif
    }

    public void StartButton()
    {
        DataManager.Instance.SaveSettings();
        SceneManager.LoadScene(1);
    }

    // Conditional compilation code written to test exit button in the editor
    public void ExitButton()
    {
        DataManager.Instance.SaveSettings();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    public void ToggleCredits()
    {
        creditsScreen.SetActive(!creditsScreen.activeSelf);
    }

    // To be used by main menu music volume slider
    // Changes stored music volume value to be used in game scene
    public void SetMusicVolume(float volume)
    {
        Globals.musicVolume = volume;
    }

    // To be used by main menu effects volume slider
    // Changes stored effects volume value to be used in game scene
    public void SetEffectsVolume(float volume)
    {
        Globals.effectsVolume = volume;
    }
}
