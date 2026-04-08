using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the in-game heads-up display (HUD).
/// </summary>
public class HUDController : MonoBehaviour
{
    [SerializeField] Slider healthSlider;
    [SerializeField] Slider energySlider;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI timeText;

    [Space]
    [SerializeField] SessionStats sessionStats;

    private int lastTimeInSeconds;

    void OnEnable()
    {
        GameEvents.OnHealthChanged += SetHealthDisplay;
        GameEvents.OnEnergyChanged += SetEnergyDisplay;
        GameEvents.OnScoreChanged += SetScoreDisplay;
    }

    void OnDisable()
    {
        GameEvents.OnHealthChanged -= SetHealthDisplay;
        GameEvents.OnEnergyChanged -= SetEnergyDisplay;
        GameEvents.OnScoreChanged -= SetScoreDisplay;
    }

    void Start()
    {
        Player player = FindFirstObjectByType<Player>();

        // Set initial health UI
        healthSlider.minValue = 0f;
        healthSlider.maxValue = player.MaxHealth;
        healthSlider.value = player.Health;

        // Set initial shield UI
        energySlider.minValue = 0f;
        energySlider.maxValue = player.MaxEnergy;
        energySlider.value = player.Energy;

        lastTimeInSeconds = 0;
    }

    // Update timer every second
    // Additional check on if seconds actually changed to prevent UI redraw on every frame
    void Update()
    {
        int currentTimeInSeconds = (int)sessionStats.TimeAlive;
        if (currentTimeInSeconds != lastTimeInSeconds)
        {
            SetTimerDisplay(currentTimeInSeconds);
            lastTimeInSeconds = currentTimeInSeconds;
        }
    }

    void SetHealthDisplay(float health)
    {
        healthSlider.value = health;
    }

    void SetEnergyDisplay(float energy)
    {
        energySlider.value = energy;
    }

    void SetScoreDisplay(int score)
    {
        scoreText.SetText("Score: " + score.ToString("D2"));
    }

    void SetTimerDisplay(int timeInSeconds)
    {
        // Split time into minutes and seconds
        int timeMinute = (int)timeInSeconds / 60;
        int timeSecond = (int)timeInSeconds % 60;

        // Show time text in a two digit format
        timeText.SetText(timeMinute.ToString("D2") + ":" + timeSecond.ToString("D2"));
    }
}
