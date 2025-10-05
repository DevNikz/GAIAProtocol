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


    [SerializeField] private State state;
    [SerializeField] private float timer;
    [SerializeField, Range(0f, 10f)] private float setTimer;

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
                        Debug.Log("Enemy is busy");
                        state = State.Busy;
                    } else {
                        // No more enemies have actions they can take, end Enemy turn
                        Debug.Log("End Enemy Turn");
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
        //timer = setTimer;
        state = State.TakingTurn;
    }

    private void TurnSystem_OnTurnChanged(object sender, System.EventArgs e) {
        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            Debug.Log("Enemy's turn");
            // Enemy turn
            //timer = setTimer;
            timer = .5f;
            state = State.TakingTurn;
        }
        else
        {
            state = State.WaitingForEnemyTurn;
        }
    }

    private bool TryTakeEnemyAIAction(Action onEnemyAIActionComplete) {
        List<Unit> enemyUnitList = UnitManager.Instance.GetEnemyUnitList();
        foreach (Unit enemyUnit in enemyUnitList)
        {
            if (enemyUnit.IsEnemyAIActive() && enemyUnit.GetActionPoints() > 0)
            {
                if (TryTakeEnemyAIAction(enemyUnit, onEnemyAIActionComplete))
                {
                    //Debug.Log($"TryTakeEnemyAIAction1 by {enemyUnit.name}");
                    //CameraManager.Instance.TeleportCamera(enemyUnit.GetPosition());
                    return true;
                }
            }
        }   
        return false;
    }

    private bool TryTakeEnemyAIAction(Unit enemyUnit, Action onEnemyAIActionComplete) {
        //Debug.Log($"TryTakeEnemyAIAction2 by {enemyUnit.name}");
        MoveAction.EnemyAIAction moveAIAction = enemyUnit.GetAction<MoveAction>().GetEnemyAIAction();
        //Debug.Log(moveAIAction);
        ShootAction.EnemyAIAction shootAIAction = enemyUnit.GetAction<ShootAction>().GetEnemyAIAction();

        // Try shooting
        if (enemyUnit.IsVisible() && shootAIAction != null) {
            Unit targetShootUnit = LevelGrid.Instance.GetUnit(shootAIAction.actionGridPosition);
            if (enemyUnit.TrySpendActionPointsToTakeAction(enemyUnit.GetAction<ShootAction>())) {
                // Take the action
                // Debug.Log("Pew");
                enemyUnit.GetAction<ShootAction>().Shoot(targetShootUnit, onEnemyAIActionComplete);
                return true;
            }
        }

        // Try moving
        if (moveAIAction != null)
        {
            Debug.Log("Enemy Unit is trying to move.");
            Vector3 actionPosition = LevelGrid.Instance.GetWorldPosition(moveAIAction.actionGridPosition);
            if (enemyUnit.TrySpendActionPointsToTakeAction(enemyUnit.GetAction<MoveAction>()))
            {
                // Take the action
                enemyUnit.GetAction<MoveAction>().Move(actionPosition, onEnemyAIActionComplete);
                return true;
            }
        }
        else Debug.Log("moveAIAction null");

        // Vector3 spinPosition = enemyUnit.GetPosition();
        // if (enemyUnit.GetAction<SpinAction>().IsValidActionPosition(spinPosition)) {
        //     if (enemyUnit.TrySpendActionPointsToTakeAction(enemyUnit.GetAction<SpinAction>())) {
        //         // Take the action
        //         enemyUnit.GetAction<SpinAction>().Spin(onEnemyAIActionComplete);
        //         return true;
        //     }
        // }
        return false;
    }

}