using System;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;
    public event EventHandler<OnChangeFloorsStartedEventArgs> OnChangedFloorsStarted;

    public class OnChangeFloorsStartedEventArgs : EventArgs
    {
        public GridPosition unitGridPosition;
        public GridPosition targetGridPosition;
    }

    public List<Vector3> pathL;

    [SerializeField]
    public LineRenderer Path;

    [SerializeField]
    public float heightOffset;

    [SerializeField]
    public int maxMoveDistance = 4;

    public void SetMoveDist(int value)
    {
        maxMoveDistance = value;
    }

    [SerializeField, Range(0.1f, 10f)]
    public float moveSpeed = 5f;

    [SerializeField]
    private int patrolRadius = 3;

    [SerializeField]
    bool hasPathLineVisual = true;

    protected List<Vector3> positionList;
    protected int currentPositionIndex;
    protected bool isChangingFloors;
    protected float differentFloorsTeleportTimer;
    protected float differentFloorsTeleportTimerMax = .5f;

    private void Update()
    {
        if (!isActive)
        {
            if (hasPathLineVisual)
                Path.transform.gameObject.SetActive(false);
            return;
        }
        else
        {
            if (hasPathLineVisual)
                Path.transform.gameObject.SetActive(true);
        }

        Vector3 targetPosition = positionList[currentPositionIndex];

        if (isChangingFloors)
        {
            // Stop and Teleport Logic
            Vector3 targetSameFloorPosition = targetPosition;
            targetSameFloorPosition.y = transform.position.y;

            Vector3 rotateDirection = (targetSameFloorPosition - transform.position).normalized;

            float rotateSpeed = 10f;
            transform.forward = Vector3.Slerp(
                transform.forward,
                rotateDirection,
                Time.deltaTime * rotateSpeed
            );

            differentFloorsTeleportTimer -= Time.deltaTime;
            if (differentFloorsTeleportTimer < 0f)
            {
                isChangingFloors = false;
                transform.position = targetPosition;
            }
        }
        else
        {
            //Debug.Log($"{name} is moving");
            // Regular move logic
            Vector3 moveDirection = (targetPosition - transform.position).normalized;

            float rotateSpeed = 10f;
            transform.forward = Vector3.Slerp(
                transform.forward,
                moveDirection,
                Time.deltaTime * rotateSpeed
            );

            transform.position += moveDirection * moveSpeed * Time.deltaTime;
            //Debug.Log($"{pathL}");
        }

        float stoppingDistance = .1f;
        if (Vector3.Distance(transform.position, targetPosition) < stoppingDistance)
        {
            currentPositionIndex++;
            if (currentPositionIndex >= positionList.Count)
            {
                OnStopMoving?.Invoke(this, EventArgs.Empty);

                ActionComplete();
            }
            else
            {
                targetPosition = positionList[currentPositionIndex];
                GridPosition targetGridPosition = LevelGrid.Instance.GetGridPosition(
                    targetPosition
                );
                GridPosition unitGridPosition = LevelGrid.Instance.GetGridPosition(
                    transform.position
                );

                if (targetGridPosition.floor != unitGridPosition.floor)
                {
                    // Different floors
                    isChangingFloors = true;
                    differentFloorsTeleportTimer = differentFloorsTeleportTimerMax;

                    OnChangedFloorsStarted?.Invoke(
                        this,
                        new OnChangeFloorsStartedEventArgs
                        {
                            unitGridPosition = unitGridPosition,
                            targetGridPosition = targetGridPosition,
                        }
                    );
                }
            }
        }

        if (hasPathLineVisual)
        {
            Path.positionCount = pathL.Count;
            for (int i = 0; i < pathL.Count; i++)
            {
                Path.SetPosition(i, pathL[i] + Vector3.up * heightOffset);
            }
        }
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        List<GridPosition> pathGridPositionList = Pathfinding.Instance.FindPath(
            unit.GetGridPosition(),
            gridPosition,
            out int pathLength
        );

        // Guard: no valid path found
        if (unit.IsEnemy() == true)
        {
            if (CheckNullPos(pathGridPositionList))
            {
                Debug.Log("Null Position. Action Ended.");
                ActionComplete();
            }
        }

        currentPositionIndex = 0;
        positionList = new List<Vector3>();

        OnStartMoving?.Invoke(this, EventArgs.Empty);

        foreach (GridPosition pathGridPosition in pathGridPositionList)
        {
            positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
        }

        //Debug.Log($"{positionList.Count}");
        if (hasPathLineVisual)
        {
            pathL = new List<Vector3>();
            pathL.Clear();
            pathL = positionList;
        }

        ActionStart(onActionComplete);
    }

    bool CheckNullPos(List<GridPosition> pathGridPositionList)
    {
        if (pathGridPositionList == null || pathGridPositionList.Count == 0)
        {
            return true;
        }
        else
            return false;
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
        {
            for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {
                for (int floor = -maxMoveDistance; floor <= maxMoveDistance; floor++)
                {
                    GridPosition offsetGridPosition = new GridPosition(x, z, floor);
                    GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                    if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                    {
                        continue;
                    }

                    if (unitGridPosition == testGridPosition)
                    {
                        // Same Grid Position where the unit is already at
                        continue;
                    }

                    if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                    {
                        // Grid Position already occupied with another Unit
                        continue;
                    }

                    if (!Pathfinding.Instance.IsWalkableGridPosition(testGridPosition))
                    {
                        continue;
                    }

                    if (!Pathfinding.Instance.HasPath(unitGridPosition, testGridPosition))
                    {
                        continue;
                    }

                    int pathfindingDistanceMultiplier = 10;
                    if (
                        Pathfinding.Instance.GetPathLength(unitGridPosition, testGridPosition)
                        > maxMoveDistance * pathfindingDistanceMultiplier
                    )
                    {
                        // Path length is too long
                        continue;
                    }

                    validGridPositionList.Add(testGridPosition);
                    //Debug.Log($"Pos: {testGridPosition}");
                }
            }
        }

        return validGridPositionList;
    }

    public override string GetActionName()
    {
        return "Move";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        List<GridPosition> validPositions = GetValidActionGridPositionList();
        int targetCountAtGridPosition;
        GridPosition chosenPosition;

        if (GetComponent<KaijuUnit>() != null)
        {
            if (!GetComponent<KaijuUnit>().IsPlayerDetected())
            {
                //Debug.Log("Enemy is Patrolling");
                GridPosition unitGridPosition = unit.GetGridPosition();
                List<GridPosition> positionsInRadius = new List<GridPosition>();

                foreach (GridPosition pos in validPositions)
                {
                    int dx = Mathf.Abs(pos.x - unitGridPosition.x);
                    int dz = Mathf.Abs(pos.z - unitGridPosition.z);
                    if (dx <= patrolRadius && dz <= patrolRadius)
                    {
                        positionsInRadius.Add(pos);
                    }
                }

                // Pick a random position from the radius, fall back to any valid pos
                // GridPosition chosenPosition;
                if (positionsInRadius.Count > 0)
                {
                    chosenPosition = positionsInRadius[
                        UnityEngine.Random.Range(0, positionsInRadius.Count)
                    ];
                }
                else if (validPositions.Count > 0)
                {
                    chosenPosition = validPositions[
                        UnityEngine.Random.Range(0, validPositions.Count)
                    ];
                }
                else
                {
                    return new EnemyAIAction { gridPosition = gridPosition, actionValue = 0 };
                }

                return new EnemyAIAction
                {
                    gridPosition = chosenPosition,
                    actionValue = 10, // flat value so AI treats all patrol moves equally
                };
            }
            else
            {
                // Prefer SwordAction if present, then ShootAction, then fall back to a
                // neutral value so the AI can still move even without a combat action.
                // Debug.Log("Not Patrolling");
                SwordAction swordAction = unit.GetAction<SwordAction>();
                if (swordAction != null)
                {
                    targetCountAtGridPosition = swordAction.GetTargetCountAtPosition(gridPosition);
                }
                else
                {
                    ShootAction shootAction = unit.GetAction<ShootAction>();
                    if (shootAction != null)
                    {
                        targetCountAtGridPosition = shootAction.GetTargetCountAtPosition(
                            gridPosition
                        );
                    }
                    else
                    {
                        // No attack action — assign a small constant so the enemy will
                        // still move (toward the player via patrol) rather than score 0.
                        targetCountAtGridPosition = 1;
                    }
                }

                return new EnemyAIAction
                {
                    gridPosition = gridPosition,
                    actionValue = targetCountAtGridPosition * 10, // flat value so AI treats all patrol moves equally
                };
            }
        }

        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 10, // flat value so AI treats all patrol moves equally
        };
    }

    /*
    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        // Prefer SwordAction if present, then ShootAction, then fall back to a
        // neutral value so the AI can still move even without a combat action.
        int targetCountAtGridPosition;

        SwordAction swordAction = unit.GetAction<SwordAction>();
        if (swordAction != null)
        {
            targetCountAtGridPosition = swordAction.GetTargetCountAtPosition(gridPosition);
        }
        else
        {
            ShootAction shootAction = unit.GetAction<ShootAction>();
            if (shootAction != null)
            {
                targetCountAtGridPosition = shootAction.GetTargetCountAtPosition(gridPosition);
            }
            else
            {
                // No attack action — assign a small constant so the enemy will
                // still move (toward the player via patrol) rather than score 0.
                targetCountAtGridPosition = 1;
            }
        }

        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = targetCountAtGridPosition * 10,
        };
    }
    */

    public int GetTargetCountAtPosition(GridPosition gridPosition)
    {
        // This lets other actions (and the AI) query MoveAction symmetrically.
        return GetValidActionGridPositionList().Count;
    }
}
