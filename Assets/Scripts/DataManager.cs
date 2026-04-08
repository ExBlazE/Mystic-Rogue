using UnityEngine;

/// <summary>
/// Singleton class used to call methods that save/load stats and settings to/from disk.
/// </summary>
public class DataManager : MonoBehaviour
{
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

        // Set max frame rate
        // Application.targetFrameRate = -1;
    }

    public void SaveHighScore()
    {
        PlayerPrefs.SetInt("HighScore", Globals.HighScore);
        PlayerPrefs.Save();
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("MusicVolume", Globals.MusicVolume);
        PlayerPrefs.SetFloat("EffectsVolume", Globals.EffectsVolume);
        PlayerPrefs.SetInt("FrameRateLevel", Globals.FrameRateLevel);
        PlayerPrefs.Save();
    }

    public void LoadHighScore()
    {
        if (PlayerPrefs.HasKey("HighScore"))
            Globals.HighScore = PlayerPrefs.GetInt("HighScore");
    }

    public void LoadSettings()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
            Globals.MusicVolume = PlayerPrefs.GetFloat("MusicVolume");
        if (PlayerPrefs.HasKey("EffectsVolume"))
            Globals.EffectsVolume = PlayerPrefs.GetFloat("EffectsVolume");
        if (PlayerPrefs.HasKey("FrameRateLevel"))
            Globals.FrameRateLevel = PlayerPrefs.GetInt("FrameRateLevel");
    }
}
