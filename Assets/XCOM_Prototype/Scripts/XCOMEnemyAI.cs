using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XCOMEnemyAI : MonoBehaviour {


    private enum State {
        WaitingForEnemyTurn,
        TakingTurn,
        Busy,
    }


    private State state;
    private float timer;

    private void Awake() {
        state = State.WaitingForEnemyTurn;
    }

    private void Start() {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    private void Update() {
        switch (state) {
            case State.WaitingForEnemyTurn:
                break;
            case State.TakingTurn:
                timer -= Time.deltaTime;
                if (timer <= 0f) {
                    if (TryTakeEnemyAIAction(SetStateTakingTurn)) {
                        state = State.Busy;
                    } else {
                        // No more enemies have actions they can take, end Enemy turn
                        TurnSystem.Instance.NextTurn();
                    }
                }
                break;
            case State.Busy:
                // Waiting for current action to complete
                break;
        }
    }

    private void SetStateTakingTurn() {
        timer = .5f;
        state = State.TakingTurn;
    }

    private void TurnSystem_OnTurnChanged(object sender, System.EventArgs e) {
        if (!TurnSystem.Instance.IsPlayerTurn()) {
            // Enemy turn
            timer = .5f;
            state = State.TakingTurn;
        } else {
            state = State.WaitingForEnemyTurn;
        }
    }

    private bool TryTakeEnemyAIAction(Action onEnemyAIActionComplete) {
        List<Unit> enemyUnitList = UnitManager.Instance.GetEnemyUnitList();
        foreach (Unit enemyUnit in enemyUnitList) {
            if (enemyUnit.IsEnemyAIActive() && enemyUnit.GetActionPoints() > 0) {
                if (TryTakeEnemyAIAction(enemyUnit, onEnemyAIActionComplete)) {
                    //CameraManager.Instance.TeleportCamera(enemyUnit.GetPosition());
                    return true;
                }
            }
        }
        return false;
    }

    private bool TryTakeEnemyAIAction(Unit enemyUnit, Action onEnemyAIActionComplete) {
        MoveAction.EnemyAIAction moveAIAction = enemyUnit.GetAction<MoveAction>().GetEnemyAIAction();
        ShootAction.EnemyAIAction shootAIAction = enemyUnit.GetAction<ShootAction>().GetEnemyAIAction();

        // Try shooting
        if (enemyUnit.IsVisible() && shootAIAction != null) {
            Unit targetShootUnit = LevelGrid.Instance.GetUnit(shootAIAction.actionGridPosition);
            if (enemyUnit.TrySpendActionPointsToTakeAction(enemyUnit.GetAction<ShootAction>())) {
                // Take the action
                enemyUnit.GetAction<ShootAction>()
                    .Shoot(targetShootUnit, onEnemyAIActionComplete);
                return true;
            }
        }

        // Try moving
        if (moveAIAction != null) {
            Vector3 actionPosition = LevelGrid.Instance.GetWorldPosition(moveAIAction.actionGridPosition);
            if (enemyUnit.TrySpendActionPointsToTakeAction(enemyUnit.GetAction<MoveAction>())) {
                // Take the action
                enemyUnit.GetAction<MoveAction>()
                    .Move(actionPosition, onEnemyAIActionComplete);
                return true;
            }
        }

        /*
        Vector3 actionPosition = enemyUnit.GetPosition();
        if (enemyUnit.GetAction<SpinAction>().IsValidActionPosition(actionPosition)) {
            if (enemyUnit.TrySpendActionPointsToTakeAction(enemyUnit.GetAction<SpinAction>())) {
                // Take the action
                enemyUnit.GetAction<SpinAction>()
                    .Spin(onEnemyAIActionComplete);
                return true;
            }
        }
        */
        return false;
    }

}