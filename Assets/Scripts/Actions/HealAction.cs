using System;
using System.Collections.Generic;
using UnityEngine;

public class HealAction : BaseAction
{
    public static event EventHandler<OnHealEventArgs> OnAnyHeal;
    public event EventHandler<OnHealEventArgs> OnHeal;

    public class OnHealEventArgs : EventArgs
    {
        public Unit targetUnit;
        public Unit healingUnit;
    }

    private enum State
    {
        Preparing,
        Healing,
        Cooloff,
    }

    [SerializeField]
    private LayerMask obstaclesLayerMask;

    private State state;
    private float stateTimer;
    private Unit targetUnit;
    private bool canHeal;

    [SerializeField, Range(1, 100)]
    private int minHealAmount = 5;

    public void SetMinHealAmount(int value)
    {
        minHealAmount = value;
    }

    [SerializeField, Range(1, 100)]
    private int maxHealAmount = 10;

    public void SetMaxHealAmount(int value)
    {
        maxHealAmount = value;
    }

    [SerializeField]
    private int maxHealDistance = 5;

    public void SetHealRange(int value)
    {
        maxHealDistance = value;
    }

    // If true, this unit can also heal itself
    [SerializeField]
    private bool canHealSelf = true;

    public void SetCanHealSelf(bool value)
    {
        canHealSelf = value;
    }

    private void Update()
    {
        if (!isActive)
            return;

        stateTimer -= Time.deltaTime;

        switch (state)
        {
            case State.Preparing:
                // Rotate toward target
                Vector3 aimDir = (
                    targetUnit.GetWorldPosition() - unit.GetWorldPosition()
                ).normalized;
                aimDir.y = 0f;
                transform.forward = Vector3.Slerp(transform.forward, aimDir, Time.deltaTime * 10f);
                break;
            case State.Healing:
                if (canHeal)
                {
                    Heal();
                    canHeal = false;
                }
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
                state = State.Healing;
                stateTimer = 0.1f;
                break;
            case State.Healing:
                state = State.Cooloff;
                stateTimer = 0.5f;
                break;
            case State.Cooloff:
                ActionComplete();
                break;
        }
    }

    private void Heal()
    {
        OnAnyHeal?.Invoke(
            this,
            new OnHealEventArgs { targetUnit = targetUnit, healingUnit = unit }
        );

        OnHeal?.Invoke(this, new OnHealEventArgs { targetUnit = targetUnit, healingUnit = unit });

        targetUnit
            .GetComponent<HealthSystem>()
            .Heal(UnityEngine.Random.Range(minHealAmount, maxHealAmount));
    }

    public override string GetActionName() => "Heal";

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        return GetValidActionGridPositionList(unit.GetGridPosition());
    }

    public List<GridPosition> GetValidActionGridPositionList(GridPosition unitGridPosition)
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        for (int x = -maxHealDistance; x <= maxHealDistance; x++)
        {
            for (int z = -maxHealDistance; z <= maxHealDistance; z++)
            {
                for (int floor = -maxHealDistance; floor <= maxHealDistance; floor++)
                {
                    GridPosition offsetGridPosition = new GridPosition(x, z, floor);
                    GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                    if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                        continue;

                    int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                    if (testDistance > maxHealDistance)
                        continue;

                    if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                        continue;

                    Unit testUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                    // Skip enemies — only heal friendly units
                    if (testUnit.IsEnemy() != unit.IsEnemy())
                        continue;

                    // Skip self if canHealSelf is disabled
                    bool isSelf = testGridPosition == unitGridPosition;
                    if (isSelf && !canHealSelf)
                        continue;

                    // Optional: skip units at full health (comment out if not needed)
                    if (testUnit.GetHealthNormalized() >= 1f)
                        continue;

                    // Line-of-sight check (mirrors ShootAction pattern)
                    Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(
                        unitGridPosition
                    );
                    Vector3 healDir = (testUnit.GetWorldPosition() - unitWorldPosition).normalized;
                    float unitShoulderHeight = 1.7f;

                    if (
                        Physics.Raycast(
                            unitWorldPosition + Vector3.up * unitShoulderHeight,
                            healDir,
                            Vector3.Distance(unitWorldPosition, testUnit.GetWorldPosition()),
                            obstaclesLayerMask
                        )
                    )
                    {
                        continue; // Blocked by obstacle
                    }

                    validGridPositionList.Add(testGridPosition);
                }
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        state = State.Preparing;
        stateTimer = 0.8f;
        canHeal = true;

        ActionStart(onActionComplete);
    }

    public Unit GetTargetUnit() => targetUnit;

    public int GetMaxHealDistance() => maxHealDistance;

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        Unit testUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        // Prioritize healing units that are more injured
        return new EnemyAIAction { gridPosition = gridPosition, actionValue = 0 };
    }

    public int GetTargetCountAtPosition(GridPosition gridPosition)
    {
        return GetValidActionGridPositionList(gridPosition).Count;
    }

    public override int GetActionPointsCost()
    {
        return 1;
    }

    public override EnemyAIAction GetEnemyAIAction(
        GridPosition gridPosition,
        List<GridPosition> validPositions
    )
    {
        return new EnemyAIAction { gridPosition = gridPosition, actionValue = 0 };
    }
}
