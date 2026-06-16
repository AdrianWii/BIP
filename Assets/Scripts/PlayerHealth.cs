using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance { get; private set; }

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float loseHealthEverySeconds = 2f;
    [SerializeField] private int healthLostPerTick = 1;

    [Header("UI")]
    [SerializeField] private TMP_Text healthText;

    private int currentHealth;
    private float timer;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    private void Update()
    {
        if (currentHealth <= 0)
        {
            return;
        }

        timer += Time.deltaTime;

        if (timer >= loseHealthEverySeconds)
        {
            timer = 0f;
            LoseHealth(healthLostPerTick);
        }
    }

    public void AddHealth(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthUI();
    }

    private void LoseHealth(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = "Health: " + currentHealth + " / " + maxHealth;
        }
    }
}