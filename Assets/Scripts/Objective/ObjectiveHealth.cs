using UnityEngine;

public class ObjectiveHealth : ObjectiveBase
{
    [SerializeField]
    private float maxHealth = 100f;

    private float currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public override float GetProgress() => 1f - Mathf.Clamp01(currentHealth / maxHealth);

    public void TakeDamage(float amount)
    {
        if (isComplete)
            return;

        currentHealth = Mathf.Max(0f, currentHealth - amount);

        if (currentHealth <= 0f)
            CompleteObjective();
    }

    public float GetCurrentHealth() => currentHealth;

    public float GetMaxHealth() => maxHealth;
}
