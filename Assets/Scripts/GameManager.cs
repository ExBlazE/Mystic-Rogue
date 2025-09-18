using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Game UI")]
    [SerializeField] GameObject gameUI;
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] Slider healthSlider;
    [SerializeField] Slider shieldSlider;

    [Header("Tutorial UI")]
    [SerializeField] GameObject tutorialUI;

    [Header("Pause Menu UI")]
    [SerializeField] GameObject pauseUI;
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider effectsVolumeSlider;

    [Header("Game Over UI")]
    [SerializeField] GameObject gameOverUI;
    [SerializeField] TextMeshProUGUI finalScoreText;
    [SerializeField] GameObject newHighScoreText;

    [Header("Parent Transforms for Instantiation")]
    public Transform enemyGroup;
    public Transform projectileGroup;
    public Transform particlesGroup;

    public int score {  get; private set; }
    public float timeAlive { get; private set; }
    public int maxEnemies { get; private set; }

    [Header("Settings")]
    [SerializeField] int setMaxEnemies = 20;
    [SerializeField] float gameEndSlowTime = 3f;

    [Space]
    [Header("Info (Do not change)")]
    public int enemiesOnScreen;

    public bool isGameActive { get; private set; }
    public bool isGamePaused { get; private set; }

    public static GameManager Instance;

    void Awake()
    {
        // Singleton reference to this script
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // Set initial state of the game
        isGameActive = true;
        isGamePaused = false;
        timeAlive = 0;
        score = 0;
        maxEnemies = setMaxEnemies;

        // Set health UI
        healthSlider.minValue = 0f;
        healthSlider.maxValue = PlayerControl.Instance.maxHealth;
        healthSlider.value = PlayerControl.Instance.health;

        // Set shield UI
        shieldSlider.minValue = 0f;
        shieldSlider.maxValue = PlayerControl.Instance.maxShield;
        shieldSlider.value = PlayerControl.Instance.shield;

        // Enable and disable all relevant UI as needed
        gameUI.SetActive(true);
        tutorialUI.SetActive(true);
        pauseUI.SetActive(false);
        gameOverUI.SetActive(false);
        newHighScoreText.SetActive(false);

        // Set initial volume from main menu
        if (DataManager.Instance != null)
        {
            musicVolumeSlider.value = DataManager.Instance.volume;
            effectsVolumeSlider.value = DataManager.Instance.volume;
        }
    }

    void Update()
    {
        // Update Health and Shield UI
        healthSlider.value = PlayerControl.Instance.health;
        shieldSlider.value = PlayerControl.Instance.shield;

        // Update the time and score UI every frame
        UpdateTime();
        UpdateScore();

        // Press Esc to pause and unpause
        if (Input.GetKeyDown(KeyCode.Escape) && isGameActive)
        {
            if (!isGamePaused)
            {
                isGamePaused = true;
                Time.timeScale = 0;
                pauseUI.SetActive(true);
                AudioManager.Instance.PauseBGM();
            }
            else
            {
                isGamePaused = false;
                Time.timeScale = 1f;
                pauseUI.SetActive(false);
                AudioManager.Instance.UnPauseBGM();
            }
        }
    }

    // Method to add score
    public void AddScore(int addScore)
    {
        score += addScore;
    }

    // Method to update time UI
    public void UpdateTime()
    {
        timeAlive += Time.deltaTime;

        // Split time into minutes and seconds
        int timeMinute = (int)timeAlive / 60;
        int timeSeconds = (int)timeAlive % 60;
        
        // Show time text in a two digit format
        timeText.SetText(timeMinute.ToString("D2") + ":" +  timeSeconds.ToString("D2"));
    }

    // Method to update score UI
    public void UpdateScore()
    {
        // Show score text in a two digit format
        scoreText.SetText("Score: " + score.ToString("D2"));
    }

    // Method for game over
    public void EndGame()
    {
        isGameActive = false;
        AudioManager.Instance.EndBGM();
        StartCoroutine(GameOver(gameEndSlowTime));
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void Replay()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }

    // Game over process
    IEnumerator GameOver(float slowOverTime)
    {
        // Slow time to a stop
        float currentTimeScale = Time.timeScale;
        float timeElapsed = 0;

        while (timeElapsed < slowOverTime)
        {
            Time.timeScale = Mathf.Lerp(currentTimeScale, 0f, timeElapsed / slowOverTime);
            timeElapsed += Time.unscaledDeltaTime;

            yield return null;
        }

        Time.timeScale = 0f;

        // Show game over screen
        gameOverUI.SetActive(true);
        finalScoreText.SetText(score.ToString("D2"));

        // Check for new high score
        if (DataManager.Instance != null)
        {
            if (DataManager.Instance.TryHighScore(score))
                newHighScoreText.SetActive(true);
        }
    }
}
