using UnityEngine;

public class SessionStats : MonoBehaviour
{
    public int score { get; private set; }
    public float timeAlive { get; private set; }
    public bool newHighScore { get; private set; }

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
        score = 0;
        timeAlive = 0;
        newHighScore = false;

        if (DataManager.Instance != null)
            Globals.highScore = 0;
    }

    void Update()
    {
        timeAlive += Time.deltaTime;
    }

    void AddScore(int score)
    {
        this.score += score;
        GameEvents.RaiseOnScoreChanged(this.score);
    }

    void TryHighScore()
    {
        if (score > Globals.highScore)
        {
            Globals.highScore = score;
            if (DataManager.Instance != null)
                DataManager.Instance.SaveHighScore();
            newHighScore = true;
        }
    }
}