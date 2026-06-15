using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject uiRoot;
    [SerializeField] private TMP_Text paintingsText;
    [SerializeField] private TMP_Text banknotesText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        RefreshStatsUI();
    }

    public void ToggleUI()
    {
        uiRoot.SetActive(!uiRoot.activeSelf);
    }

    public void RefreshStatsUI()
    {
        int paintings = PlayerPrefs.GetInt("PaintingsCollected", 0);
        int banknotes = PlayerPrefs.GetInt("BanknotesCollected", 0);

        paintingsText.text = $"Paintings: {paintings}/5";
        banknotesText.text = $"Banknotes: {banknotes}/5";
    }
}