using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private int highScore = 0;
    [HideInInspector] public float musicVolume = 0.5f;
    [HideInInspector] public float effectsVolume = 0.5f;

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

    // Try to set new high score and get success or failure result via bool return
    public bool TryHighScore(int score)
    {
        if (score > highScore)
        {
            highScore = score;
            SaveData();
            return true;
        }
        else
            return false;
    }

    public int GetHighScore()
    {
        return highScore;
    }

    // To be used by main menu music volume slider
    // Changes stored music volume value to be used in game scene
    public void SetMusicVolume(float volume)
    {
        this.musicVolume = volume;
    }

    // To be used by main menu effects volume slider
    // Changes stored effects volume value to be used in game scene
    public void SetEffectsVolume(float volume)
    {
        this.effectsVolume = volume;
    }

    public void SaveData()
    {
        GameData data = new GameData();

        data.highScore = this.highScore;
        data.musicVolume = this.musicVolume;
        data.effectsVolume = this.effectsVolume;

        string jsonText = JsonUtility.ToJson(data);

#if UNITY_WEBGL
        PlayerPrefs.SetString("gameData", jsonText);
        PlayerPrefs.Save();
#else
        File.WriteAllText(Application.persistentDataPath + "/gamedata.json", jsonText);
#endif
    }

    public void LoadData()
    {
        string jsonText = null;
#if UNITY_WEBGL
        if (PlayerPrefs.HasKey("gameData"))
            jsonText = PlayerPrefs.GetString("gameData");
#else
        string path = Application.persistentDataPath + "/gamedata.json";
        if (File.Exists(path))
            jsonText = File.ReadAllText(path);
#endif
        if (jsonText != null)
        {
            GameData data = JsonUtility.FromJson<GameData>(jsonText);

            this.highScore = data.highScore;
            this.musicVolume = data.musicVolume;
            this.effectsVolume = data.effectsVolume;
        }
    }
}

// Class for storing high score on a json file
[System.Serializable]
public class GameData
{
    public int highScore;
    public float musicVolume;
    public float effectsVolume;
}
