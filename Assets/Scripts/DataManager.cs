using System.IO;
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

    // To be used by main menu volume slider
    // Doesn't actually change volume, just its stored value
    public void SetMusicVolume(float volume)
    {
        this.volume = volume;
    }

    public void SaveData()
    {
        GameData data = new GameData();

        data.highScore = this.highScore;
        data.initialVolume = this.volume;

        string jsonText = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/gamedata.json", jsonText);
    }

    public void LoadData()
    {
        string path = Application.persistentDataPath + "/gamedata.json";
        if (File.Exists(path))
        {
            string jsonText = File.ReadAllText(path);
            GameData data = JsonUtility.FromJson<GameData>(jsonText);

            this.highScore = data.highScore;
            this.volume = data.initialVolume;
        }
    }
}

// Class for storing high score on a json file
[System.Serializable]
public class GameData
{
    public int highScore;
    public float initialVolume;
}
