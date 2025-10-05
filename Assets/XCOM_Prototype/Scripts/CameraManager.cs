using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    public static CameraManager Instance { get; private set; }


    [SerializeField] private Transform actionCameraTransform;
    [SerializeField] private Transform cameraTargetTransform;


    private void Awake() {
        Instance = this;
    }

    private void Start() {
        Unit.OnUnitSpawned += Unit_OnUnitSpawned;

        foreach (Unit unit in UnitManager.Instance.GetUnitList()) {
            UnitSpawned(unit);
        }

        UnitActionSystem.Instance.OnSelectedChanged += UnitActionSystem_OnSelectedChanged;
    }

    private void UnitActionSystem_OnSelectedChanged(object sender, UnitActionSystem.OnSelectedChangedEventArgs e) {
        //TeleportCamera(UnitActionSystem.Instance.GetSelectedUnit().GetPosition());
    }

    private void Unit_OnUnitSpawned(object sender, System.EventArgs e) {
        Unit unit = sender as Unit;
        UnitSpawned(unit);
    }

    private void UnitSpawned(Unit unit) {
        ShootAction shootAction = unit.GetAction<ShootAction>();
        shootAction.OnActionStarted += ShootAction_OnActionStarted;
        shootAction.OnActionComplete += ShootAction_OnActionComplete;
    }

    private void ShootAction_OnActionComplete(object sender, System.EventArgs e) {
        actionCameraTransform.gameObject.SetActive(false);
    }

    private void ShootAction_OnActionStarted(object sender, System.EventArgs e) {
        ShootAction shootAction = sender as ShootAction;

        Unit shooterUnit = shootAction.GetUnit();
        Unit targetUnit = shootAction.GetTargetUnit();

        Vector3 shootDir = (targetUnit.GetPosition() - shooterUnit.GetPosition()).normalized;

        Vector3 cameraCharacterHeight = Vector3.up * 1.7f;
        actionCameraTransform.position = (shooterUnit.GetPosition() + cameraCharacterHeight) + (shootDir * -1f) + (Quaternion.Euler(0, +90, 0) * shootDir * .5f);
        actionCameraTransform.LookAt(targetUnit.GetPosition() + cameraCharacterHeight);

        actionCameraTransform.gameObject.SetActive(true);
    }

    public void TeleportCamera(Vector3 targetPosition) {
        cameraTargetTransform.position = targetPosition;
    }

}