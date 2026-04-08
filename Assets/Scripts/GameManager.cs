using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the state of the game and its game-over transition.<br/>
/// <i>MANDATORY for every scene with controllable player.</i>
/// </summary>
public class GameManager : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] float gameEndSlowTime = 3f;

    public GameState gameState { get; private set; }
    public static GameManager Instance;

    void OnEnable()
    {
        GameEvents.OnPlayerDeath += EndGame;
        InputEvents.OnPauseRequest += HandlePause;
    }

    void OnDisable()
    {
        GameEvents.OnPlayerDeath -= EndGame;
        InputEvents.OnPauseRequest -= HandlePause;
    }

    void Awake()
    {
        // Singleton reference to this script
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Set max frame rate
        // Application.targetFrameRate = -1;
    }

    void Start()
    {
        // Set initial state of the game
        gameState = GameState.Playing;
    }

    // Method for game over
    public void EndGame()
    {
        gameState = GameState.GameOver;
        StartCoroutine(GameOver(gameEndSlowTime));
    }

    // Inspector-assigned method for returning to menu
    public void BackToMenu()
    {
        Time.timeScale = 1f;
        if (DataManager.Instance != null)
            DataManager.Instance.SaveSettings();
        SceneManager.LoadScene(0);
    }

    // Inspector-assigned method for replaying the game
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
        GameEvents.RaiseOnGameOver();
    }

    // Invoked on pressing the pause button
    void HandlePause()
    {
        if (gameState == GameState.Playing)
        {
            gameState = GameState.Paused;
            GameEvents.RaiseOnGamePause();
            Time.timeScale = 0;
        }
        else if (gameState == GameState.Paused)
        {
            gameState = GameState.Playing;
            GameEvents.RaiseOnGameResume();
            Time.timeScale = 1f;
        }
    }
}

public enum GameState
{
    Playing,
    Paused,
    GameOver
}