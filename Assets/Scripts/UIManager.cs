using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI highScoreText;
    [SerializeField] Slider volumeSlider;

    void Start()
    {
        // Set volume slider at previously set value
        volumeSlider.value = DataManager.Instance.volume;
    }

    void Update()
    {
        // Update high score display in real time
        highScoreText.SetText(DataManager.Instance.GetHighScore().ToString("D2"));
    }

    public void StartButton()
    {
        SceneManager.LoadScene(1);
    }

    // Conditional compilation code written to test exit button in the editor
    public void ExitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
