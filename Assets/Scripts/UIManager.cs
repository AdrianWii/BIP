using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private ImageTrackedCollectibles collectibles;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private CountdownTimer countdownTimer;

    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text collectedText;
    [SerializeField] private TMP_Text pointsText;


    private void OnEnable()
    {
        if (collectibles != null)
        {
            collectibles.StatusChanged += UpdateStatus;
        }

        if (playerStats != null)
        {
            playerStats.StatsChanged += UpdateStats;
        }
    }

    private void Start()
    {
        UpdateStats();

        if (statusText != null)
        {
            statusText.text = "Collectibles: Ready";
        }
    }

    private void OnDisable()
    {
        if (collectibles != null)
        {
            collectibles.StatusChanged -= UpdateStatus;
        }

        if (playerStats != null)
        {
            playerStats.StatsChanged -= UpdateStats;
        }
    }

    public void ResetStats()
    {
        playerStats.ResetAllProgressInThisApp();
        countdownTimer.ResetTimer();
    }

    private void UpdateStatus(string status)
    {
        if (statusText != null)
        {
            statusText.text = $"Collectibles: {status}";
        }
    }

    private void UpdateStats()
    {
        if (playerStats == null)
        {
            return;
        }

        if (collectedText != null)
        {
            collectedText.text = $"Collected: {playerStats.CollectedCount}";
        }

        if (pointsText != null)
        {
            pointsText.text = $"Points: {playerStats.TotalPoints}";
        }
    }
}