using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnitAction {

    ActionType GetActionType();
    void Setup(Unit unit);
    Unit GetUnit();
    bool IsActive();

}

public enum ActionType {
    Move,
    Spin,
    Shoot,
    Grenade,
    Overwatch,
}
