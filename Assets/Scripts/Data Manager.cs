using UnityEngine;

public class DataManager : MonoBehaviour
{
    private int highScore = 0;
    public float volume = 0.5f;

    public static DataManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public void TryHighScore(int score)
    {
        if (score > highScore)
            highScore = score;
    }

    public int GetHighScore()
    {
        return highScore;
    }

    public void SetMusicVolume(float volume)
    {
        this.volume = volume;
    }
}
