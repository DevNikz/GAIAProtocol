using System;
using System.Collections.Generic;
using UnityEngine;

public class RepairAction : BaseAction
{
    public static event EventHandler<OnRepairEventArgs> OnAnyRepair;
    public event EventHandler<OnRepairEventArgs> OnRepair;

    public class OnRepairEventArgs : EventArgs
    {
        public Unit repairingUnit;
    }

    private enum State
    {
        Preparing,
        Repairing,
        Cooloff,
    }

    private State state;
    private float stateTimer;

    [SerializeField, Range(1, 100)]
    private int minRepairAmount = 15;

    public void SetMinRepairAmount(int value)
    {
        minRepairAmount = value;
    }

    [SerializeField, Range(1, 100)]
    private int maxRepairAmount = 30;

    public void SetMaxRepairAmount(int value)
    {
        maxRepairAmount = value;
    }

    private void Update()
    {
        if (!isActive)
            return;

        stateTimer -= Time.deltaTime;

        switch (state)
        {
            case State.Preparing:
                // Face forward (no target to rotate toward)
                break;
            case State.Repairing:
                Repair();
                state = State.Cooloff; // Repair fires once
                break;
            case State.Cooloff:
                break;
        }

        if (stateTimer <= 0f)
        {
            NextState();
        }
    }

    private void NextState()
    {
        switch (state)
        {
            case State.Preparing:
                state = State.Repairing;
                stateTimer = 0.1f;
                break;
            case State.Repairing:
                state = State.Cooloff;
                stateTimer = 0.5f;
                break;
            case State.Cooloff:
                ActionComplete();
                break;
        }
    }

    private void Repair()
    {
        OnAnyRepair?.Invoke(this, new OnRepairEventArgs { repairingUnit = unit });
        OnRepair?.Invoke(this, new OnRepairEventArgs { repairingUnit = unit });

        unit.GetComponent<HealthSystem>()
            .Heal(UnityEngine.Random.Range(minRepairAmount, maxRepairAmount));
    }

    public override string GetActionName()
    {
        return "Repair";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction { gridPosition = gridPosition, actionValue = 0 };
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        return new List<GridPosition> { unit.GetGridPosition() };
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        state = State.Preparing;
        stateTimer = 0.4f;

        ActionStart(onActionComplete);
    }

    //Costs four points
    public override int GetActionPointsCost()
    {
        return 4;
    }

    public override EnemyAIAction GetEnemyAIAction(
        GridPosition gridPosition,
        List<GridPosition> validPositions
    )
    {
        return new EnemyAIAction { gridPosition = gridPosition, actionValue = 0 };
    }
}
