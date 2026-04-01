using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

        // Set health UI
        healthSlider.minValue = 0f;
        healthSlider.maxValue = player.maxHealth;
        healthSlider.value = player.health;

        // Set shield UI
        energySlider.minValue = 0f;
        energySlider.maxValue = player.maxEnergy;
        energySlider.value = player.energy;

        lastTimeInSeconds = 0;
    }

    void Update()
    {
        int currentTimeInSeconds = (int)sessionStats.timeAlive;
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
