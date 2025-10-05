using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

/*
 * Handles Unit Action Switching and Activation
 * */
public class UnitActionSystem : MonoBehaviour {

    public static UnitActionSystem Instance { get; private set; }


    public event EventHandler<OnSelectedChangedEventArgs> OnSelectedChanged;
    public event EventHandler<bool> OnBusyChanged;


    public class OnSelectedChangedEventArgs : EventArgs {
        public Unit unit;
        public IUnitAction unitAction;
    }


    [SerializeField] private Unit startingSelectedUnit;

    private Unit selectedUnit;
    private IUnitAction selectedUnitAction;
    private bool isBusy; // Is it busy with the last action?

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        SetSelectedUnit(startingSelectedUnit, startingSelectedUnit.GetAction<MoveAction>());

        TurnSystem.Instance.OnTurnChanged += Instance_OnTurnChanged;
    }

    private void Instance_OnTurnChanged(object sender, EventArgs e) {
        if (TurnSystem.Instance.IsPlayerTurn()) {
            // Players turn
            Unit firstFriendlyUnit = UnitManager.Instance.GetFriendlyUnitList()[0];
            SetSelectedUnit(firstFriendlyUnit, firstFriendlyUnit.GetAction<MoveAction>());
        }
    }

    private void Update() {
        if (isBusy) return; // Busy with an action
        if (!TurnSystem.Instance.IsPlayerTurn()) return; // Enemy taking a turn, wait...

        // Activate Action with Mouse
        if (Input.GetMouseButtonDown(0) && !UtilsClass.IsPointerOverUI()) {
            Vector3 worldPosition = UtilsClass.GetMouseWorldPosition();
            Vector2Int gridPosition = LevelGrid.Instance.GetGridPosition(worldPosition);

            if (LevelGrid.Instance.IsValidGridPosition(gridPosition)) {
                Unit unit = LevelGrid.Instance.GetUnit(gridPosition);

                switch (selectedUnitAction.GetActionType()) {
                    case ActionType.Move:
                        if (unit != null) {
                            // Select this unit if friendly
                            if (!unit.IsEnemy()) {
                                SetSelectedUnit(unit, unit.GetAction<MoveAction>());
                            }
                        } else {
                            // Clicked on somewhere with no Unit, move action
                            if (selectedUnit.GetAction<MoveAction>().IsValidMovePosition(worldPosition)) {
                                // Valid move position, Try to Move
                                if (selectedUnit.TrySpendActionPointsToTakeAction(selectedUnitAction)) {
                                    // Did have action points to spend, Move
                                    SetBusy();
                                    selectedUnit.GetAction<MoveAction>()
                                        .Move(worldPosition, ClearBusy);
                                }
                            }
                        }
                        break;
                    case ActionType.Shoot:
                        if (selectedUnit.GetAction<ShootAction>().IsValidShootPosition(worldPosition)) {
                            // Shoot this Unit
                            if (selectedUnit.TrySpendActionPointsToTakeAction(selectedUnitAction)) {
                                SetBusy();
                                selectedUnit.GetAction<ShootAction>()
                                    .Shoot(unit, ClearBusy);
                            }
                        } else {
                            // Clicked on invalid shoot position, do nothing
                        }
                        break;
                    case ActionType.Spin:
                        if (selectedUnit.GetAction<SpinAction>().IsValidActionPosition(worldPosition)) {
                            if (selectedUnit.TrySpendActionPointsToTakeAction(selectedUnitAction)) {
                                SetBusy();
                                selectedUnit.GetAction<SpinAction>()
                                .Spin(ClearBusy);
                            }
                        }
                        break;
                    case ActionType.Grenade:
                        if (selectedUnit.GetAction<GrenadeAction>().IsValidActionPosition(worldPosition)) {
                            if (selectedUnit.TrySpendActionPointsToTakeAction(selectedUnitAction)) {
                                SetBusy();
                                selectedUnit.GetAction<GrenadeAction>()
                                .ThrowGrenade(worldPosition, ClearBusy);
                            }
                        }
                        break;
                    case ActionType.Overwatch:
                        if (selectedUnit.GetAction<OverwatchAction>().IsValidActionPosition(worldPosition)) {
                            if (selectedUnit.TrySpendActionPointsToTakeAction(selectedUnitAction)) {
                                SetBusy();
                                selectedUnit.GetAction<OverwatchAction>()
                                .Overwatch(ClearBusy);
                            }
                        }
                        break;
                }
            }
        }


        if (Input.GetMouseButtonDown(1)) {
            // "Deselect" by going back to Move Action
            SetSelectedUnit(selectedUnit, selectedUnit.GetAction<MoveAction>());
        }
    }

    private void SetSelectedUnit(Unit unit, IUnitAction unitAction) {
        selectedUnit = unit;
        selectedUnitAction = unitAction;
        OnSelectedChanged?.Invoke(this, new OnSelectedChangedEventArgs { unit = unit, unitAction = unitAction });
    }

    public void SetSelectedAction(IUnitAction unitAction) {
        selectedUnitAction = unitAction;
        OnSelectedChanged?.Invoke(this, new OnSelectedChangedEventArgs { unit = selectedUnit, unitAction = unitAction });
    }

    public IUnitAction GetSelectedUnitAction() {
        return selectedUnitAction;
    }

    public Unit GetSelectedUnit() {
        return selectedUnit;
    }

    public bool HasSelectedUnit() {
        return selectedUnit != null;
    }

    private void SetBusy() {
        isBusy = true;
        OnBusyChanged?.Invoke(this, isBusy);
    }

    private void ClearBusy() {
        isBusy = false;
        OnBusyChanged?.Invoke(this, isBusy);
    }

    public bool IsBusy() {
        return isBusy;
    }

}