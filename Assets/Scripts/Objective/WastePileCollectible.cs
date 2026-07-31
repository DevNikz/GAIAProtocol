using System;
using UnityEngine;

public class WastePileCollectible : ObjectiveBase, IInteractable
{
    private bool hasBeenCollected;

    void Start() => RegisterOnGrid();

    protected override void RegisterOnGrid()
    {
        Bounds bounds = GetObjectiveBounds();
        GridPosition minGridPosition = LevelGrid.Instance.GetGridPosition(bounds.min);
        GridPosition maxGridPosition = LevelGrid.Instance.GetGridPosition(bounds.max);

        int minX = Mathf.Min(minGridPosition.x, maxGridPosition.x);
        int maxX = Mathf.Max(minGridPosition.x, maxGridPosition.x);
        int minZ = Mathf.Min(minGridPosition.z, maxGridPosition.z);
        int maxZ = Mathf.Max(minGridPosition.z, maxGridPosition.z);

        for (int x = minX; x <= maxX; x++)
        {
            for (int z = minZ; z <= maxZ; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z, 0);

                LevelGrid.Instance.SetInteractableAtGridPosition(gridPosition, this);
                LevelGrid.Instance.SetIngameObjectAtGridPosition(gridPosition, this.gameObject);
                Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, false);
                occupiedGridPositions.Add(gridPosition);
            }
        }
    }

    public override float GetProgress() => hasBeenCollected ? 1f : 0f;

    public void Interact(Action onInteractionComplete) { }

    public void Interact(Action onInteractionComplete, float percentageAdd) { }

    public void Interact(Action onInteractionComplete, Unit unit)
    {
        if (hasBeenCollected)
        {
            onInteractionComplete?.Invoke();
            return;
        }

        var counter = GetSharedObjective() as ObjectiveCounter;
        if (counter == null)
        {
            Debug.LogWarning(
                $"ObjectiveCounterTarget on '{name}' couldn't find an ObjectiveCounter registered with index {objectiveIndex}. Make sure one exists in the scene.",
                this
            );
            return;
        }

        UnitWasteCarrier carrier = unit.GetComponent<UnitWasteCarrier>();
        if (carrier == null || !carrier.TryPickUp())
        {
            Debug.Log("Can't Pick Up. Returning AP");
            unit.actionPoints += 2;
            onInteractionComplete?.Invoke();
            return;
        }
        SoundManager.Instance.PlaySFX("Harvest");
        hasBeenCollected = true;
        counter.Increment(1);
        UnregisterFromGrid();
        gameObject.SetActive(false);
        onInteractionComplete?.Invoke();
    }

    /// <summary>Used by ObjectiveWorldUI to find what progress to display for this instance.</summary>
    public ObjectiveBase GetSharedObjective() =>
        ObjectiveManager.Instance.GetObjective(objectiveIndex);
}
