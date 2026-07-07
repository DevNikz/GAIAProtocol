using UnityEngine;

/// <summary>
/// Sits on each scattered world instance of a counter objective (e.g. one
/// specific crystal out of five). Not an ObjectiveBase itself - just points at
/// the shared ObjectiveCounter by index. On interact, reports one increment,
/// then disables itself so it can't be triggered twice.
/// </summary>
public class ObjectiveCounterTarget : MonoBehaviour
{
    [SerializeField]
    private int objectiveIndex;

    [SerializeField, Min(1)]
    private int incrementAmount = 1;

    private bool hasBeenCollected;

    /// <summary>Used by ObjectiveWorldUI to find what progress to display for this instance.</summary>
    public ObjectiveBase GetSharedObjective() =>
        ObjectiveManager.Instance.GetObjective(objectiveIndex);

    public void Interact()
    {
        if (hasBeenCollected)
            return;

        var counter = GetSharedObjective() as ObjectiveCounter;
        if (counter == null)
        {
            Debug.LogWarning(
                $"ObjectiveCounterTarget on '{name}' couldn't find an ObjectiveCounter registered with index {objectiveIndex}. Make sure one exists in the scene.",
                this
            );
            return;
        }

        hasBeenCollected = true;
        counter.Increment(incrementAmount);

        // Swap for Destroy(gameObject) if you want the instance to disappear
        // entirely rather than just go inactive.
        gameObject.SetActive(false);
    }
}
