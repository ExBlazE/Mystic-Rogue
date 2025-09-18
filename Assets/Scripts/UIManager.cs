using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI highScoreText;
    [SerializeField] Slider volumeSlider;

    DataManager dm;

    void Start()
    {
        // Get singleton reference to DataManager
        dm = DataManager.Instance;

        // Load saved high score and volume
        dm.LoadData();

        // Set volume slider at previously set value
        volumeSlider.value = dm.volume;
    }

    void Update()
    {
        // Update high score display in real time
        highScoreText.SetText(dm.GetHighScore().ToString("D2"));
    }

    public void StartButton()
    {
        dm.SaveData();
        SceneManager.LoadScene(1);
    }

    // Conditional compilation code written to test exit button in the editor
    public void ExitButton(Button button)
    {
        dm.SaveData();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#elif UNITY_WEBGL
        button.interactable = false;
#else
        Application.Quit();
#endif
    }
}
