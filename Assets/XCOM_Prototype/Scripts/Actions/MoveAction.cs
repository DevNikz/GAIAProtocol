using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction {

    //public static MoveAction GetAction(IUnitAction unitAction) => unitAction as MoveAction;

    public override ActionType GetActionType() => ActionType.Move;


    private int maxMoveDistance = 4;
    private List<Vector3> pathPositionList;
    private int currentPositionIndex;

    private void Awake() {
        currentPositionIndex = -1;
    }


    private void Update() {
        if (!isActive) return;

        Vector3 targetPosition = pathPositionList[currentPositionIndex];

        Vector3 moveDir = (targetPosition - transform.position).normalized;

        if (Vector3.Dot(transform.forward, moveDir) == -1) {
            // Moving straight back, rotation bug, add some slight rotation to force real rotation
            transform.eulerAngles += new Vector3(0, 5f, 0);
        }

        float rotationSpeed = 10f;
        transform.forward = Vector3.Lerp(transform.forward, moveDir, Time.deltaTime * rotationSpeed);

        float moveSpeed = 4f;
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        float reachedDistance = .1f;
        if (Vector3.Distance(transform.position, targetPosition) < reachedDistance) {
            // Reached current target position
            // Is there more?
            currentPositionIndex++;
            if (currentPositionIndex >= pathPositionList.Count) {
                // Reached end of position list
                ActionComplete();
            } else {
                // Still more positions to go
            }
        }
    }

    public void Move(Vector3 targetPosition, Action onActionComplete) {
        ActionStarted(onActionComplete);

        pathPositionList = LevelPathfinding.Instance.FindPath(unit.GetPosition(), targetPosition, out int pathLength);

        currentPositionIndex = 0;
    }

    public bool IsMoving() {
        return isActive;
    }

    public bool IsValidMovePosition(Vector3 moveWorldPosition) {
        return IsValidMovePosition(LevelGrid.Instance.GetGridPosition(moveWorldPosition));
    }

    public bool IsValidMovePosition(Vector2Int moveGridPosition) {
        List<Vector2Int> validMoveGridPositionList = GetValidMoveGridPositionList();
        return validMoveGridPositionList.Contains(moveGridPosition);
    }

    public List<Vector2Int> GetValidMoveGridPositionList() {
        Vector2Int currentGridPosition = unit.GetGridPosition();

        List<Vector2Int> validMoveGridPositionList = new List<Vector2Int>();

        for (int x = -maxMoveDistance; x <= maxMoveDistance; x++) {
            for (int y = -maxMoveDistance; y <= maxMoveDistance; y++) {
                Vector2Int moveGridPosition = currentGridPosition + new Vector2Int(x, y);
                if (moveGridPosition == currentGridPosition) continue; // Same position, skip
                if (LevelGrid.Instance.IsValidGridPosition(moveGridPosition)) {
                    // Valid grid position (not off bounds)
                    if (!LevelGrid.Instance.HasUnitAtPosition(moveGridPosition)) {
                        // No unit on this position
                        List<Vector3> path = LevelPathfinding.Instance.FindPath(unit.GetPosition(), LevelGrid.Instance.GetWorldPosition(moveGridPosition.x, moveGridPosition.y), out int pathLength);
                        if (path != null) {
                            // There is a path here
                            //Debug.Log(currentGridPosition + " " + moveGridPosition + ": " + path.Count + " : " + pathLength);
                            float moveCost = (float)PathfindingXCOM.Pathfinding.MOVE_STRAIGHT_COST;
                            if ((pathLength / moveCost) <= maxMoveDistance) {
                                // Within valid move distance
                                validMoveGridPositionList.Add(moveGridPosition);
                            }
                        }
                    }
                }
            }
        }

        return validMoveGridPositionList;
    }

    public EnemyAIAction GetEnemyAIAction() {
        List<Vector2Int> validMoveGridPositionList = GetValidMoveGridPositionList();

        List<EnemyAIAction> enemyAIActionList = new List<EnemyAIAction>();

        ShootAction shootAction = unit.GetAction<ShootAction>();

        foreach (Vector2Int gridPosition in validMoveGridPositionList) {
            // Calculate AI Action Value for moving to this position
            int targetCount = shootAction.GetTargetCountAtPosition(gridPosition);
            enemyAIActionList.Add(new EnemyAIAction {
                actionGridPosition = gridPosition,
                actionValue = 10 * targetCount,
            });
        }

        if (enemyAIActionList.Count > 0) {
            // Sort by actionValue
            enemyAIActionList.Sort((EnemyAIAction a, EnemyAIAction b) => b.actionValue - a.actionValue);
            return enemyAIActionList[0];
        } else {
            // Cannot move anywhere
            return null;
        }
    }

    public class EnemyAIAction {
        public Vector2Int actionGridPosition;
        public int actionValue;
    }

}