using UnityEngine;

public class ToxicStatusEffect : MonoBehaviour
{
    private int turnsRemaining;
    private float slowAmount;
    private bool isActive;

    public void Apply(int turnsRemaining, float slowAmount)
    {
        // Refresh if already applied
        this.turnsRemaining = Mathf.Max(this.turnsRemaining, turnsRemaining);
        this.slowAmount = slowAmount;
        isActive = true;

        TurnSystem.Instance.OnTurnChanged -= OnTurnChanged; // prevent double-subscribe
        TurnSystem.Instance.OnTurnChanged += OnTurnChanged;
    }

    // Call this from MoveAction or wherever move speed is read
    public float GetSpeedMultiplier() => isActive ? (1f - slowAmount) : 1f;

    private void OnTurnChanged(object sender, System.EventArgs e)
    {
        if (!isActive) return;

        turnsRemaining--;
        if (turnsRemaining <= 0)
        {
            isActive = false;
            TurnSystem.Instance.OnTurnChanged -= OnTurnChanged;
            Destroy(this);
        }
    }
}