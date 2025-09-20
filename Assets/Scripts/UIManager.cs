using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI highScoreText;
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider effectsVolumeSlider;
    [SerializeField] Button exitButton;
    [SerializeField] GameObject creditsScreen;

    DataManager dm;

    void Start()
    {
        // Get singleton reference to DataManager
        dm = DataManager.Instance;

        // Load saved high score and volume
        dm.LoadData();

        // Set volume sliders at previously set value
        musicVolumeSlider.value = dm.musicVolume;
        effectsVolumeSlider.value = dm.effectsVolume;

#if UNITY_WEBGL
        // On WebGL build, hide the exit button
        exitButton.interactable = false;
#endif
    }

    void Update()
    {
        // Update high score display in real time
        highScoreText.SetText(dm.GetHighScore().ToString("D2"));
    }

    public void StartButton()
    {
        dm.SaveData();
        SceneManager.LoadScene(1);
    }

    // Conditional compilation code written to test exit button in the editor
    public void ExitButton()
    {
        dm.SaveData();
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
}
