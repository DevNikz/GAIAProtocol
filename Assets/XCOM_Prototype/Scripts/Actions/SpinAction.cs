using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAction : BaseAction {

    public override ActionType GetActionType() => ActionType.Spin;



    private float timer = 0f;


    private void Update() {
        if (!isActive) return;

        transform.eulerAngles += new Vector3(0, 360 * Time.deltaTime, 0);

        timer -= Time.deltaTime;
        if (timer <= 0f) {
            // Done

            ActionComplete();
        }
    }

    public void Spin(Action onActionComplete) {
        ActionStarted(onActionComplete);

        timer = 1f;
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