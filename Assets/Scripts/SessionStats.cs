using UnityEngine;

/// <summary>
/// Manages the stats for every game session.<br/>
/// <i>MANDATORY for every scene with controllable player.</i>
/// </summary>
public class SessionStats : MonoBehaviour
{
    public int Score { get; private set; }
    public float TimeAlive { get; private set; }

    void OnEnable()
    {
        GameEvents.OnEnemyDeath += AddScore;
        GameEvents.OnGameOver += TryHighScore;
    }

    void OnDisable()
    {
        GameEvents.OnEnemyDeath -= AddScore;
        GameEvents.OnGameOver -= TryHighScore;
    }

    void Start()
    {
        Score = 0;
        TimeAlive = 0;

        if (DataManager.Instance == null)
            Globals.HighScore = 0;
    }

    void Update()
    {
        TimeAlive += Time.deltaTime;
    }

    void AddScore(int score)
    {
        this.Score += score;
        GameEvents.RaiseOnScoreChanged(this.Score);
    }

    // Check for new high score and if true, save to disk immediately
    void TryHighScore()
    {
        if (Score > Globals.HighScore)
        {
            Globals.HighScore = Score;
            if (DataManager.Instance != null)
                DataManager.Instance.SaveHighScore();

            GameEvents.RaiseOnHighScore();
        }
    }
}