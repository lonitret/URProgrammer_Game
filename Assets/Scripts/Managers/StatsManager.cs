using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance { get; private set; }

    [Header("Гнев")]
    public float currentAnger = 0f;
    public float maxAnger = 100f;
    [SerializeField] private Slider angerSlider;
    [SerializeField] private Image dangerOverlay;

    [Header("Репутация")]
    public int reputation = 0;
    [SerializeField] private TextMeshProUGUI reputationText;

    void Awake() => Instance = this;

    void Start()
    {
        if (dangerOverlay != null) dangerOverlay.gameObject.SetActive(true);

        UpdateUI();
    }

    public void ChangeAnger(float amount)
    {
        currentAnger = Mathf.Clamp(currentAnger + amount, 0, maxAnger);
        UpdateUI();

        if (currentAnger >= maxAnger) GameOver();
    }

    public void ChangeReputation(int amount)
    {
        reputation += amount;
        UpdateUI();
    }

    public void SetAnger(float value)
    {
        currentAnger = Mathf.Clamp(value, 0, maxAnger);
        UpdateUI();
        if (currentAnger >= maxAnger) GameOver();
    }

    private void UpdateUI()
    {
        if (angerSlider != null)
        {
            float targetValue = currentAnger / maxAnger;
            angerSlider.value = targetValue;
            Debug.Log($"Slider updated to: {targetValue}");
        }

        if (reputationText != null) reputationText.text = $"Репутация: {reputation}";

        if (dangerOverlay != null)
        {
            var color = dangerOverlay.color;
            color.a = (currentAnger / maxAnger) * 0.5f;
            dangerOverlay.color = color;
        }

    }

    private void GameOver()
    {
        Debug.Log("Нервный срыв! Игра окончена.");
        // Здесь можно вызвать SceneManager.LoadScene для перезапуска
    }
}