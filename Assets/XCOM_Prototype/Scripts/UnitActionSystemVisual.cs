using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitActionSystemVisual : MonoBehaviour {

    [SerializeField] private Transform pfGrenadeRange;


    private Transform grenadeRangeTransform;


    private void Start() {
        UnitActionSystem.Instance.OnSelectedChanged += UnitActionSystem_OnSelectedChanged;
    }

    private void UnitActionSystem_OnSelectedChanged(object sender, UnitActionSystem.OnSelectedChangedEventArgs eventArgs) {
        switch (eventArgs.unitAction.GetActionType()) {
            case ActionType.Grenade:
                break;
        }
    }

}