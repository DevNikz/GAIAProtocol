using System;
using System.Collections.Generic;
using UnityEngine;

public class InteractAction : BaseAction
{
    [SerializeField]
    private int actionpointcost;
    private int maxInteractDistance = 1;

    [SerializeReference]
    private GameObject objective;

    [SerializeField]
    float percentageAdd = 0f;

    public void SetInteractEfficiency(float value)
    {
        percentageAdd = value;
    }

    private void Update()
    {
        if (!isActive)
        {
            return;
        }
    }

    public override string GetActionName()
    {
        return "Interact";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction { gridPosition = gridPosition, actionValue = 0 };
    }

    public override EnemyAIAction GetEnemyAIAction(
        GridPosition gridPosition,
        List<GridPosition> validPositions
    )
    {
        return new EnemyAIAction { gridPosition = gridPosition, actionValue = 0 };
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxInteractDistance; x <= maxInteractDistance; x++)
        {
            for (int z = -maxInteractDistance; z <= maxInteractDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z, 0);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                IInteractable interactable = LevelGrid.Instance.GetInteractableAtGridPosition(
                    testGridPosition
                );

                if (interactable == null)
                {
                    // No interactable on this GridPosition
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        IInteractable interactable = LevelGrid.Instance.GetInteractableAtGridPosition(gridPosition);

        if (interactable is ObjectiveInteractFill objectiveInteractFill)
            objectiveInteractFill.Interact(OnInteractComplete, percentageAdd);
        else if (interactable is ObjectiveCounterTarget objectiveCounterTarget)
            objectiveCounterTarget.Interact(OnInteractComplete);

        ActionStart(onActionComplete);
    }

    private void OnInteractComplete()
    {
        ActionComplete();
    }

    public override int GetActionPointsCost()
    {
        return GetComponent<Unit>().actionPoints;
    }
}
