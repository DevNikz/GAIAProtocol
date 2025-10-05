using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverwatchAction : BaseAction {

    public override ActionType GetActionType() => ActionType.Overwatch;

    public event EventHandler OnPassiveActiveChanged;

    public event EventHandler<OnShootEventArgs> OnShoot;
    public class OnShootEventArgs : EventArgs {
        public Unit shotUnit;
        public bool hit;
    }


    private int maxShootDistance = 6;
    private bool passiveActive;

    private void Start() {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        UnitManager.Instance.OnAnyUnitMovedGridPosition += UnitManager_OnAnyUnitMovedGridPosition;
    }

    private void UnitManager_OnAnyUnitMovedGridPosition(object sender, EventArgs e) {
        if (passiveActive) {
            Unit movedUnit = sender as Unit;
            if (!movedUnit.GetHealthSystem().IsDead() && movedUnit.IsEnemy() && movedUnit.IsVisible()) {
                // It's an alive enemy and it's visible!
                passiveActive = false;

                // Calc hit or miss
                bool hit = true;// UnityEngine.Random.Range(0, 1f) < GetHitPercent(targetUnit);

                OnShoot?.Invoke(this, new OnShootEventArgs { shotUnit = movedUnit, hit = hit });

                if (hit) {
                    // Hit the unit!
                    int damageAmount = UnityEngine.Random.Range(30, 60);
                    movedUnit.GetHealthSystem().Damage(damageAmount);
                }
            }
        }
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e) {
        if (TurnSystem.Instance.IsPlayerTurn()) {
            // Back into the Player's turn, stop Overwatching
            passiveActive = false;
        }
    }

    private void Update() {
        if (passiveActive) HandlePassiveActive();

        if (!isActive) return;

        ActionComplete();
    }

    public void Overwatch(Action onActionComplete) {
        ActionStarted(onActionComplete);

        passiveActive = true;
    }

    private void HandlePassiveActive() {

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

    public bool IsPassiveActive() {
        return passiveActive;
    }

    public bool IsValidActionPosition(Vector3 actionWorldPosition) {
        return IsValidActionPosition(LevelGrid.Instance.GetGridPosition(actionWorldPosition));
    }

    public bool IsValidActionPosition(Vector2Int actionGridPosition) {
        List<Vector2Int> validActionGridPositionList = GetValidActionGridPositionList();
        return validActionGridPositionList.Contains(actionGridPosition);
    }

    public List<Vector2Int> GetValidActionGridPositionList() {
        return new List<Vector2Int> { unit.GetGridPosition() };
    }
}