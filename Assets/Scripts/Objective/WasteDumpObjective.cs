using System;
using UnityEngine;

public class WasteDumpObjective : ObjectiveBase, IInteractable
{
    [SerializeField, Min(1)]
    private int requiredWastePiles = 5;

    [SerializeField]
    private int currentWastePiles;

    void Start()
    {
        RegisterOnGrid();
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            isBeingInteracted = false;
        }
    }

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

    protected override void UnregisterFromGrid()
    {
        foreach (GridPosition gridPosition in occupiedGridPositions)
        {
            LevelGrid.Instance.ClearInteractableAtGridPosition(gridPosition);
            LevelGrid.Instance.ClearIngameObjectAtGridPosition(gridPosition);
            // Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, true);
        }
        if (occupiedGridPositions != null)
            occupiedGridPositions.Clear();
    }

    public override float GetProgress() => (float)currentWastePiles / requiredWastePiles;

    public void Interact(Action onInteractionComplete) { }

    public void Interact(Action onInteractionComplete, float percentageAdd) { }

    public void Interact(Action onInteractionComplete, Unit unit)
    {
        if (isComplete)
        {
            onInteractionComplete?.Invoke();
            return;
        }
        UnitWasteCarrier carrier = unit.GetComponent<UnitWasteCarrier>();
        if (carrier == null || !carrier.TryDrop())
        {
            onInteractionComplete?.Invoke();
            return;
        }
        SoundManager.Instance.PlaySFX("Harvest");

        currentWastePiles++;

        if (currentWastePiles >= requiredWastePiles)
        {
            SoundManager.Instance.PlaySFX("Objective Complete");

            UnregisterFromGrid();
            CompleteObjective();
        }
        onInteractionComplete?.Invoke();
    }
}
