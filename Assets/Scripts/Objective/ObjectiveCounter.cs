using UnityEngine;

/// <summary>
/// Objective type 4: a single logical objective that multiple scattered world
/// instances count toward (e.g. "collect 5 crystals", "activate 3 beacons").
/// This component holds the SHARED state - place exactly one in the scene per
/// objectiveIndex (it doesn't need to live on any specific pickup). Scattered
/// instances use ObjectiveCounterTarget to report increments back to this.
/// </summary>
public class ObjectiveCounter : ObjectiveBase
{
    [SerializeField, Min(1)]
    private int targetCount = 5;

    private int currentCount;

    public override float GetProgress() =>
        targetCount <= 0 ? 1f : Mathf.Clamp01((float)currentCount / targetCount);

    public int GetCurrentCount() => currentCount;

    public int GetTargetCount() => targetCount;

    /// <summary>Called by an ObjectiveCounterTarget when its instance is interacted with.</summary>
    public void Increment(int amount = 1)
    {
        if (isComplete)
            return;

        currentCount = Mathf.Min(targetCount, currentCount + amount);
        ObjectiveManager.Instance.NotifyProgress(GetObjectiveIndex());

        if (currentCount >= targetCount)
        {
            CompleteObjective();
        }
    }
}
