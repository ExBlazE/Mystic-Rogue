using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
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
    }

    void OnDisable()
    {
        GameEvents.OnGamePause -= ShowPauseMenu;
        GameEvents.OnGameResume -= HidePauseMenu;
        GameEvents.OnGameOver -= ShowGameOverScreen;
    }

    void Start()
    {
        pauseMenu.SetActive(false);
        gameOverScreen.SetActive(false);
        newHighScoreText.SetActive(false);

#if UNITY_STANDALONE || UNITY_WEBGL
        keyboardTutorial.SetActive(true);
        mobileTutorial.SetActive(false);
        mobileControls.SetActive(false);
        aimingArrow.SetActive(false);
#elif UNITY_ANDROID
        keyboardTutorial.SetActive(false);
        mobileTutorial.SetActive(true);
        mobileControls.SetActive(true);
        aimingArrow.SetActive(true);
#endif
    }

    void ShowPauseMenu()
    { pauseMenu.SetActive(true); }

    void HidePauseMenu()
    { pauseMenu.SetActive(false); }

    void ShowGameOverScreen()
    {
        gameOverScreen.SetActive(true);

        SessionStats stats = FindFirstObjectByType<SessionStats>();
        finalScoreText.SetText(stats.score.ToString("D2"));
        if (stats.newHighScore)
            newHighScoreText.SetActive(true);
    }
}
