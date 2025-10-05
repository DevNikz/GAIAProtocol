using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitActionSystemVisual_OLD : MonoBehaviour {

    [SerializeField] private Material redMaterial;
    [SerializeField] private Material greenMaterial;
    [SerializeField] private Material yellowMaterial;
    [SerializeField] private Material blueMaterial;

    [SerializeField] private Transform pfUnitSelectedVisual;


    private Transform lastUnitSelectedVisual;

    private void Start() {
        UnitActionSystem.Instance.OnSelectedChanged += UnitActionSystem_OnSelectedChanged;
    }

    private void UnitActionSystem_OnSelectedChanged(object sender, UnitActionSystem.OnSelectedChangedEventArgs eventArgs) {
        if (lastUnitSelectedVisual != null) {
            Destroy(lastUnitSelectedVisual.gameObject);
        }

        lastUnitSelectedVisual = Instantiate(pfUnitSelectedVisual, eventArgs.unit.GetPosition(), Quaternion.identity);

        Material selectedMaterial = greenMaterial;

        switch (eventArgs.unitAction.GetActionType()) {
            default:
            case ActionType.Move:
                selectedMaterial = greenMaterial;
                break;
            case ActionType.Spin:
                selectedMaterial = yellowMaterial;
                break;
            case ActionType.Shoot:
                selectedMaterial = redMaterial;
                break;
            case ActionType.Grenade:
                selectedMaterial = yellowMaterial;
                break;
        }

        lastUnitSelectedVisual.Find("Visual").GetComponent<MeshRenderer>().material = selectedMaterial;

        lastUnitSelectedVisual.parent = eventArgs.unit.transform;
    }

}