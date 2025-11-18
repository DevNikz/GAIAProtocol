using System;
using System.Collections;
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
    private List<Vector3> pathL;
    [SerializeField] private LineRenderer Path;
    [SerializeField] private float heightOffset;


    [SerializeField] private int maxMoveDistance = 4;
    [SerializeField, Range(0.1f, 10f)] private float moveSpeed = 5f;

    private List<Vector3> positionList;
    private int currentPositionIndex;
    private bool isChangingFloors;
    private float differentFloorsTeleportTimer;
    private float differentFloorsTeleportTimerMax = .5f;

    private void Update()
    {
        if (!isActive)
        {
            Path.transform.gameObject.SetActive(false);
            return;
        }
        else Path.transform.gameObject.SetActive(true);

        Vector3 targetPosition = positionList[currentPositionIndex];

        if (isChangingFloors)
        {
            // Stop and Teleport Logic
            Vector3 targetSameFloorPosition = targetPosition;
            targetSameFloorPosition.y = transform.position.y;

            Vector3 rotateDirection = (targetSameFloorPosition - transform.position).normalized;

            float rotateSpeed = 10f;
            transform.forward = Vector3.Slerp(transform.forward, rotateDirection, Time.deltaTime * rotateSpeed);

            differentFloorsTeleportTimer -= Time.deltaTime;
            if (differentFloorsTeleportTimer < 0f)
            {
                isChangingFloors = false;
                transform.position = targetPosition;
            }
        }

        else
        {
            // Regular move logic
            Vector3 moveDirection = (targetPosition - transform.position).normalized;

            float rotateSpeed = 10f;
            transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);

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
            } else
            {
                targetPosition = positionList[currentPositionIndex];
                GridPosition targetGridPosition = LevelGrid.Instance.GetGridPosition(targetPosition);
                GridPosition unitGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);

                if (targetGridPosition.floor != unitGridPosition.floor)
                {
                    // Different floors
                    isChangingFloors = true;
                    differentFloorsTeleportTimer = differentFloorsTeleportTimerMax;

                    OnChangedFloorsStarted?.Invoke(this, new OnChangeFloorsStartedEventArgs{
                         unitGridPosition = unitGridPosition,
                         targetGridPosition = targetGridPosition,
                    });
                }
            }
        }

        Path.positionCount = pathL.Count;
        for(int i = 0; i < pathL.Count; i++)
        {
            Path.SetPosition(i, pathL[i] + Vector3.up * heightOffset);
        }
    }


    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        List<GridPosition> pathGridPositionList = Pathfinding.Instance.FindPath(unit.GetGridPosition(), gridPosition, out int pathLength);

        currentPositionIndex = 0;
        positionList = new List<Vector3>();

        foreach (GridPosition pathGridPosition in pathGridPositionList)
        {
            positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
        }

        //Debug.Log($"{positionList.Count}");
        pathL = new List<Vector3>();
        pathL.Clear();
        pathL = positionList;

        OnStartMoving?.Invoke(this, EventArgs.Empty);

        ActionStart(onActionComplete);
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
                    if (Pathfinding.Instance.GetPathLength(unitGridPosition, testGridPosition) > maxMoveDistance * pathfindingDistanceMultiplier)
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
        //int targetCountAtGridPosition = unit.GetAction<SwordAction>().GetValidActionGridPositionList().Count;
        int targetCountAtGridPosition = unit.GetAction<SwordAction>().GetTargetCountAtPosition(gridPosition);
        Debug.Log($"Target Count: {targetCountAtGridPosition}");
        // int targetCountAtGridPosition;

        // if(GetComponent<ShootAction>() != null) 
        // {
        //     targetCountAtGridPosition = unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition);
        // }

        // else if(GetComponent<SwordAction>() != null)
        // {
        //     targetCountAtGridPosition = unit.GetAction<SwordAction>().GetValidActionGridPositionList().Count;
        // }

        // else targetCountAtGridPosition = 1;

        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = targetCountAtGridPosition * 10,
        };
    }
    
}