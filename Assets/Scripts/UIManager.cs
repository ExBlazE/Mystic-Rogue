using TMPro;
using UnityEngine;

/// <summary>
/// Manages all UI in the game scene except the HUD.
/// </summary>
public class UIManager : MonoBehaviour
{
    [SerializeField] bool autoUISwitch;
    [SerializeField] bool forceMobileUI;

    [Space]
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject gameOverScreen;
    [SerializeField] TextMeshProUGUI finalScoreText;
    [SerializeField] GameObject newHighScoreText;

    [Space]
    [SerializeField] GameObject keyboardTutorial;
    [SerializeField] GameObject mobileTutorial;

    [Space]
    [SerializeField] GameObject mobileControls;
    [SerializeField] GameObject aimingArrow;

    void OnEnable()
    {
        GameEvents.OnGamePause += ShowPauseMenu;
        GameEvents.OnGameResume += HidePauseMenu;
        GameEvents.OnGameOver += ShowGameOverScreen;
        GameEvents.OnHighScore += ShowNewHighScore;
    }

    void OnDisable()
    {
        GameEvents.OnGamePause -= ShowPauseMenu;
        GameEvents.OnGameResume -= HidePauseMenu;
        GameEvents.OnGameOver -= ShowGameOverScreen;
        GameEvents.OnHighScore -= ShowNewHighScore;
    }

    void Start()
    {
        pauseMenu.SetActive(false);
        gameOverScreen.SetActive(false);
        newHighScoreText.SetActive(false);

        if (autoUISwitch)
        {
            bool isMobile = Application.isMobilePlatform || forceMobileUI;

            keyboardTutorial.SetActive(!isMobile);
            mobileTutorial.SetActive(isMobile);
            mobileControls.SetActive(isMobile);
            aimingArrow.SetActive(isMobile);
        }
    }

    void ShowPauseMenu()
    { pauseMenu.SetActive(true); }

    void HidePauseMenu()
    { pauseMenu.SetActive(false); }

    void ShowGameOverScreen()
    {
        gameOverScreen.SetActive(true);

        SessionStats stats = FindFirstObjectByType<SessionStats>();
        finalScoreText.SetText(stats.Score.ToString("D2"));
    }

    void ShowNewHighScore()
    { newHighScoreText.SetActive(true); }
}
