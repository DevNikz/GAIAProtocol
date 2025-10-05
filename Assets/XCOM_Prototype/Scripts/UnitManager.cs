using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour {

    public static UnitManager Instance { get; private set; }

    public event EventHandler OnAnyUnitMovedGridPosition;


    private List<Unit> unitList;
    private List<Unit> friendlyUnitList;
    private List<Unit> enemyUnitList;

    private void Awake() {
        Instance = this;

        unitList = new List<Unit>();
        friendlyUnitList = new List<Unit>();
        enemyUnitList = new List<Unit>();

        Unit.OnUnitSpawned += Unit_OnUnitSpawned;
        Unit.OnUnitDead += Unit_OnUnitDead;
    }

    private void Unit_OnUnitDead(object sender, System.EventArgs e) {
        RemoveUnit(sender as Unit);
    }

    private void Unit_OnUnitSpawned(object sender, System.EventArgs e) {
        AddUnit(sender as Unit);
    }

    private void AddUnit(Unit unit) {
        unitList.Add(unit);

        unit.OnUnitMovedGridPosition += Unit_OnUnitMovedGridPosition;

        if (unit.IsEnemy()) {
            enemyUnitList.Add(unit);
        } else {
            friendlyUnitList.Add(unit);
        }
    }

    private void Unit_OnUnitMovedGridPosition(object sender, EventArgs e) {
        OnAnyUnitMovedGridPosition?.Invoke(sender, EventArgs.Empty);
    }

    private void RemoveUnit(Unit unit) {
        unitList.Remove(unit);

        unit.OnUnitMovedGridPosition -= Unit_OnUnitMovedGridPosition;

        if (unit.IsEnemy()) {
            enemyUnitList.Remove(unit);
        } else {
            friendlyUnitList.Remove(unit);
        }
    }

    public List<Unit> GetUnitList() {
        return unitList;
    }

    public List<Unit> GetEnemyUnitList() {
        return enemyUnitList;
    }

    public List<Unit> GetFriendlyUnitList() {
        return friendlyUnitList;
    }

}