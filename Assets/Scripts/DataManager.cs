using UnityEngine;

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
        Application.targetFrameRate = 60;
    }

    public int GetHighScore()
    {
        return Globals.highScore;
    }    

    public void SaveHighScore()
    {
        PlayerPrefs.SetInt("HighScore", Globals.highScore);
        PlayerPrefs.Save();
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("MusicVolume", Globals.musicVolume);
        PlayerPrefs.SetFloat("EffectsVolume", Globals.effectsVolume);
        PlayerPrefs.Save();
    }

    public void LoadHighScore()
    {
        if (PlayerPrefs.HasKey("HighScore"))
            Globals.highScore = PlayerPrefs.GetInt("HighScore");
    }

    public void LoadSettings()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
            Globals.musicVolume = PlayerPrefs.GetFloat("MusicVolume");
        if (PlayerPrefs.HasKey("EffectsVolume"))
            Globals.effectsVolume = PlayerPrefs.GetFloat("EffectsVolume");
    }
}
