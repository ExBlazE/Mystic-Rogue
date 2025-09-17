using UnityEngine;

public class DataManager : MonoBehaviour
{
    private int highScore = 0;
    public float volume = 0.5f;

    public static DataManager Instance;

    void Awake()
    {
        // Create singleton reference to this script
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        // Set max frame rate to avoid errors in editor
        Application.targetFrameRate = 60;
    }

    // Try to set new high score and get success or failure result via bool return
    public bool TryHighScore(int score)
    {
        if (score > highScore)
        {
            highScore = score;
            return true;
        }
        else
            return false;
    }

    public int GetHighScore()
    {
        return highScore;
    }

    // To be used by main menu volume slider
    // Doesn't actually change volume, just its stored value
    public void SetMusicVolume(float volume)
    {
        this.volume = volume;
    }
}
