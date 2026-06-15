using UnityEngine;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text timerText;

    [Header("Timer Settings")]
    [SerializeField] private float startTimeInSeconds = 3600f; // 1 hour

    private float currentTime;
    private bool isRunning = true;

    private const string TimerPrefsKey = "RemainingTime";

    private void Start()
    {
        if (PlayerPrefs.HasKey(TimerPrefsKey))
        {
            currentTime = PlayerPrefs.GetFloat(TimerPrefsKey);
        }
        else
        {
            currentTime = startTimeInSeconds;
            SaveTime();
        }

        UpdateTimerUI();
    }

    private void Update()
    {
        if (!isRunning)
            return;

        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;

            if (currentTime < 0)
                currentTime = 0;

            UpdateTimerUI();
            SaveTime();
        }
        else
        {
            isRunning = false;
            OnTimerFinished();
        }
    }

    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);

        timerText.text = $"Remainig Time: {minutes:00}:{seconds:00}";
    }

    private void SaveTime()
    {
        PlayerPrefs.SetFloat(TimerPrefsKey, currentTime);
        PlayerPrefs.Save();
    }

    private void OnTimerFinished()
    {
        timerText.text = "00:00";

        Debug.Log("Time is over!");

    }

    public void ResetTimer()
    {
        currentTime = startTimeInSeconds;
        isRunning = true;
        SaveTime();
        UpdateTimerUI();
    }

    public void ClearSavedTimer()
    {
        PlayerPrefs.DeleteKey(TimerPrefsKey);
        PlayerPrefs.Save();
    }
}