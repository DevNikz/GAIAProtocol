using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : BaseAction {

    public override ActionType GetActionType() => ActionType.Shoot;


    public event EventHandler<OnShootEventArgs> OnShoot;
    public class OnShootEventArgs : EventArgs {
        public Unit shotUnit;
        public bool hit;
    }


    private enum State {
        Aiming,
        Shooting,
        Cooloff,
    }


    private int maxShootDistance = 6;
    private Unit targetUnit;
    private State state;
    private float stateTimer;
    private float maxAccuracy;


    private void Awake() {
        maxAccuracy = UnityEngine.Random.Range(.8f, 1f);
    }

    private void Update() {
        if (!isActive) return;

        switch (state) {
            default:
            case State.Aiming: // Aim at target
                Vector3 aimDir = (targetUnit.GetPosition() - transform.position).normalized;

                float rotationSpeed = 10f;
                transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * rotationSpeed);

                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f) {
                    state = State.Shooting;
                    stateTimer = .5f;

                    // Calc hit or miss
                    bool hit = UnityEngine.Random.Range(0, 1f) < GetHitPercent(targetUnit);

                    OnShoot?.Invoke(this, new OnShootEventArgs { shotUnit = targetUnit, hit = hit });

                    if (hit) {
                        // Hit the unit!
                        int damageAmount = UnityEngine.Random.Range(30, 60);
                        targetUnit.GetHealthSystem().Damage(damageAmount);
                    }
                }
                break;
            case State.Shooting: // Shoot target
                state = State.Cooloff;
                stateTimer = .5f;
                break;
            case State.Cooloff: // Cool down after firing
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f) {
                    ActionComplete();
                }
                break;
        }
    }

    public void Shoot(Unit targetUnit, Action onActionComplete) {
        this.targetUnit = targetUnit;

        ActionStarted(onActionComplete);

        state = State.Aiming;
        stateTimer = 1f;
    }

    public Unit GetTargetUnit() {
        return targetUnit;
    }

    public bool IsValidShootPosition(Vector3 moveWorldPosition) {
        return IsValidShootPosition(LevelGrid.Instance.GetGridPosition(moveWorldPosition));
    }

    public bool IsValidShootPosition(Vector2Int moveGridPosition) {
        List<Vector2Int> validShootGridPositionList = GetValidShootGridPositionList();
        return validShootGridPositionList.Contains(moveGridPosition);
    }

    public int GetTargetCountAtPosition(Vector2Int currentGridPosition) {
        return GetValidShootGridPositionList(currentGridPosition).Count;
    }

    public List<Vector2Int> GetValidShootGridPositionList() {
        Vector2Int currentGridPosition = unit.GetGridPosition();
        return GetValidShootGridPositionList(currentGridPosition);
    }

    public List<Vector2Int> GetValidShootGridPositionList(Vector2Int currentGridPosition) {
        List<Vector2Int> validShootGridPositionList = new List<Vector2Int>();

        for (int x = -maxShootDistance; x <= maxShootDistance; x++) {
            for (int y = -maxShootDistance; y <= maxShootDistance; y++) {
                Vector2Int shootGridPosition = currentGridPosition + new Vector2Int(x, y);
                Vector2Int shootVector = shootGridPosition - currentGridPosition;
                int shootDistance = Mathf.Abs(shootVector.x) + Mathf.Abs(shootVector.y);
                if (shootDistance <= maxShootDistance) {
                    // Within valid shoot distance
                    if (LevelGrid.Instance.IsValidGridPosition(shootGridPosition)) {
                        // Valid grid position (not off bounds)
                        // Is there an enemy on this position?
                        Unit shootUnit = LevelGrid.Instance.GetUnit(shootGridPosition);
                        if (shootUnit != null && shootUnit != unit && shootUnit.IsEnemy() != unit.IsEnemy() && shootUnit.IsVisible()) {
                            // There is a unit, is not self, is opposite enemy, is visible
                            validShootGridPositionList.Add(shootGridPosition);
                        }
                    }
                }
            }
        }

        return validShootGridPositionList;
    }

    public int GetMaxShootDistance() {
        return maxShootDistance;
    }

    private int GetShootDistance(Vector2Int shootGridPosition) {
        Vector2Int currentGridPosition = unit.GetGridPosition();
        Vector2Int shootVector = shootGridPosition - currentGridPosition;
        int shootDistance = Mathf.Abs(shootVector.x) + Mathf.Abs(shootVector.y);
        return shootDistance;
    }

    public bool IsWithinShootingDistance(Vector2Int shootGridPosition) {
        return GetShootDistance(shootGridPosition) <= maxShootDistance;
    }


    public float GetHitPercent(Unit shootUnit) {
        if (IsWithinShootingDistance(shootUnit.GetGridPosition())) {
            // Within shoot range
            float hitPercent = maxAccuracy;

            int shootDistance = GetShootDistance(shootUnit.GetGridPosition());
            int fullAccuracyShootDistance = 3;
            int remainingShootDistance = Mathf.Max(0, shootDistance - fullAccuracyShootDistance);
            hitPercent -= .05f * remainingShootDistance;

            switch (shootUnit.GetCoverType()) {
                case CoverType.Full:
                    hitPercent -= .3f;
                    break;
                case CoverType.Half:
                    hitPercent -= .1f;
                    break;
            }

            return hitPercent;

        } else {
            // Not within shoot range
            return 0f;
        }
    }

    public EnemyAIAction GetEnemyAIAction() {
        List<Vector2Int> validShootGridPositionList = GetValidShootGridPositionList();

        List<EnemyAIAction> enemyAIActionList = new List<EnemyAIAction>();

        foreach (Vector2Int gridPosition in validShootGridPositionList) {
            // Calculate AI Action Value for shooting this position
            Unit shootUnit = LevelGrid.Instance.GetUnit(gridPosition);
            int actionValue = 100 - shootUnit.GetHealthSystem().GetHealth();
            enemyAIActionList.Add(new EnemyAIAction {
                actionGridPosition = gridPosition,
                actionValue = actionValue,
            });
        }

        if (enemyAIActionList.Count > 0) {
            // Sort by actionValue
            enemyAIActionList.Sort((EnemyAIAction a, EnemyAIAction b) => b.actionValue - a.actionValue);
            return enemyAIActionList[0];
        } else {
            // Cannot shoot anywhere
            return null;
        }
    }

    public class EnemyAIAction {
        public Vector2Int actionGridPosition;
        public int actionValue;
    }

}