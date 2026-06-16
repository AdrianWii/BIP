using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] private int healthToRestore = 20;
    [SerializeField] private bool hideAfterCollect = true;

    private bool collected;

    public void Collect()
    {
        if (collected)
        {
            return;
        }

        collected = true;

        if (PlayerHealth.Instance != null)
        {
            PlayerHealth.Instance.AddHealth(healthToRestore);
        }

        if (hideAfterCollect)
        {
            gameObject.SetActive(false);
        }
    }
}