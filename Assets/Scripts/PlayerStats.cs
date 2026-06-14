using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private const string TotalPointsKey = "player_total_points";
    private const string CollectedCountKey = "player_collected_count";

    public int TotalPoints { get; private set; }
    public int CollectedCount { get; private set; }

    public event Action StatsChanged;

    private void Awake()
    {
        Load();
    }

    public void Load()
    {
        TotalPoints = PlayerPrefs.GetInt(TotalPointsKey, 0);
        CollectedCount = PlayerPrefs.GetInt(CollectedCountKey, 0);
        StatsChanged?.Invoke();
    }

    public bool IsCollected(string collectibleId)
    {
        if (string.IsNullOrWhiteSpace(collectibleId))
        {
            return false;
        }

        return PlayerPrefs.GetInt(GetCollectedKey(collectibleId), 0) == 1;
    }

    public bool Collect(string collectibleId, int points)
    {
        if (string.IsNullOrWhiteSpace(collectibleId))
        {
            Debug.LogWarning("PlayerStats: collectibleId is empty.");
            return false;
        }

        if (IsCollected(collectibleId))
        {
            return false;
        }

        PlayerPrefs.SetInt(GetCollectedKey(collectibleId), 1);

        TotalPoints += points;
        CollectedCount++;

        PlayerPrefs.SetInt(TotalPointsKey, TotalPoints);
        PlayerPrefs.SetInt(CollectedCountKey, CollectedCount);
        PlayerPrefs.Save();

        StatsChanged?.Invoke();
        Debug.Log($"PlayerStats: collected '{collectibleId}'. Points: {TotalPoints}, collected: {CollectedCount}.");
        return true;
    }

    public void ResetAllProgressInThisApp()
    {
        PlayerPrefs.DeleteAll();
        TotalPoints = 0;
        CollectedCount = 0;
        PlayerPrefs.Save();
        StatsChanged?.Invoke();
        Debug.Log("PlayerStats: progress reset.");
    }

    private static string GetCollectedKey(string collectibleId)
    {
        return $"collected_{collectibleId}";
    }
}
