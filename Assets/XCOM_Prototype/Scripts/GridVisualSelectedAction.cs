using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridVisualSelectedAction : MonoBehaviour {


    private void Start() {
        UnitActionSystem.Instance.OnSelectedChanged += UnitActionSystem_OnSelectedChanged;
        UnitActionSystem.Instance.OnBusyChanged += UnitActionSystem_OnBusyChanged;
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    private void UnitActionSystem_OnBusyChanged(object sender, bool e) {
        UpdateGridVisual();
    }

    private void TurnSystem_OnTurnChanged(object sender, System.EventArgs e) {
        UpdateGridVisual();
    }

    private void UnitActionSystem_OnSelectedChanged(object sender, UnitActionSystem.OnSelectedChangedEventArgs e) {
        UpdateGridVisual();
    }

    private void UpdateGridVisual() {
        GridVisual.Instance.HideAllGridPositions();

        if (!TurnSystem.Instance.IsPlayerTurn()) return; // Enemy taking a turn, don't show any grid visual
        if (UnitActionSystem.Instance.IsBusy()) return; // Action busy, don't show any grid visual
        if (!UnitActionSystem.Instance.HasSelectedUnit()) return; // No selected unit, don't show any grid visual

        IUnitAction unitAction = UnitActionSystem.Instance.GetSelectedUnitAction();
        switch (unitAction.GetActionType()) {
            case ActionType.Move:
                List<Vector2Int> validMoveGridPositionList = unitAction.GetUnit().GetAction<MoveAction>().GetValidMoveGridPositionList();
                GridVisual.Instance.ShowGridPositions(validMoveGridPositionList, GridVisual.GridVisualType.White);
                break;
            case ActionType.Spin:
                List<Vector2Int> validActionGridPositionList = unitAction.GetUnit().GetAction<SpinAction>().GetValidActionGridPositionList();
                GridVisual.Instance.ShowGridPositions(validActionGridPositionList, GridVisual.GridVisualType.Blue);
                break;
            case ActionType.Overwatch:
                validActionGridPositionList = unitAction.GetUnit().GetAction<OverwatchAction>().GetValidActionGridPositionList();
                GridVisual.Instance.ShowGridPositions(validActionGridPositionList, GridVisual.GridVisualType.Blue);
                break;
            case ActionType.Shoot:
                GridVisual.Instance.ShowGridPositionsRange(
                    unitAction.GetUnit().GetGridPosition(), 
                    unitAction.GetUnit().GetAction<ShootAction>().GetMaxShootDistance(), 
                    GridVisual.GridVisualType.RedSoft);

                List <Vector2Int> validShootGridPositionList = unitAction.GetUnit().GetAction<ShootAction>().GetValidShootGridPositionList();
                GridVisual.Instance.ShowGridPositions(validShootGridPositionList, GridVisual.GridVisualType.Red);
                break;
            case ActionType.Grenade:
                validActionGridPositionList = unitAction.GetUnit().GetAction<GrenadeAction>().GetValidActionGridPositionList();
                GridVisual.Instance.ShowGridPositions(validActionGridPositionList, GridVisual.GridVisualType.Blue);
                break;
        }
    }

}