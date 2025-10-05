using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeAction : BaseAction {

    public override ActionType GetActionType() => ActionType.Grenade;


    public const int GRENADE_THROW_RANGE = 5;
    public const int GRENADE_DAMAGE_RANGE = 2;
    public const int GRENADE_DAMAGE = 40;

    [SerializeField] private Transform pfGrenade;
    [SerializeField] private Transform pfGrenadeExplosion;
    [SerializeField] private AnimationCurve grenadeVerticalAnimationCurve;


    private Vector3 grenadeStartPosition;
    private Transform grenadeTransform;
    private Vector3 grenadeTargetPosition;

    private void Update() {
        if (!isActive) return;

        float totalDistance = Vector3.Distance(grenadeStartPosition, grenadeTargetPosition);
        Vector3 grenadePositionIgnoringY = grenadeTransform.position;
        grenadePositionIgnoringY.y = grenadeStartPosition.y;
        float currentDistance = Vector3.Distance(grenadePositionIgnoringY, grenadeTargetPosition);
        float normalizedDistance = currentDistance / totalDistance;

        float grenadeThrowHeight = 4f;
        grenadeTransform.position = new Vector3(grenadeTransform.position.x, grenadeVerticalAnimationCurve.Evaluate(normalizedDistance) * grenadeThrowHeight, grenadeTransform.position.z);

        Vector3 moveDir = (grenadeTargetPosition - grenadePositionIgnoringY).normalized;
        float grenadeSpeed = 10f;
        grenadeTransform.position += moveDir * grenadeSpeed * Time.deltaTime;

        float rotationSpeed = 10f;
        transform.forward = Vector3.Lerp(transform.forward, moveDir, Time.deltaTime * rotationSpeed);


        if (normalizedDistance <= 0.05f) {
            // Done
            Instantiate(pfGrenadeExplosion, grenadeTargetPosition, Quaternion.identity);
            Destroy(grenadeTransform.gameObject);

            DamageAllInRange();

            ActionComplete();
        }
    }

    public void ThrowGrenade(Vector3 grenadeTargetPosition, Action onActionComplete) {
        ActionStarted(onActionComplete);

        grenadeStartPosition = unit.GetPosition();
        grenadeTransform = Instantiate(pfGrenade, unit.GetPosition(), Quaternion.identity);
        this.grenadeTargetPosition = grenadeTargetPosition;
        this.grenadeTargetPosition.y = 0;
    }

    private void DamageAllInRange() {
        Vector2Int currentGridPosition = LevelGrid.Instance.GetGridPosition(grenadeTargetPosition);

        int damageRange = GRENADE_DAMAGE_RANGE;

        for (int x = -damageRange; x <= damageRange; x++) {
            for (int y = -damageRange; y <= damageRange; y++) {
                int totalRange = Mathf.Abs(x) + Mathf.Abs(y);
                if (totalRange <= damageRange) {
                    Vector2Int grenadeGridPosition = currentGridPosition + new Vector2Int(x, y);
                    if (LevelGrid.Instance.IsValidGridPosition(grenadeGridPosition)) {
                        // Valid grid position, is someone here?
                        Unit unit = LevelGrid.Instance.GetUnit(grenadeGridPosition);
                        if (unit != null) {
                            // There's a unit in range
                            unit.GetHealthSystem().Damage(GRENADE_DAMAGE);
                        }
                    }
                }
            }
        }

        float worldGrenadeRange = damageRange * LevelGrid.Instance.GetCellSize();
        Collider[] colliderArray = Physics.OverlapSphere(grenadeTargetPosition, worldGrenadeRange);
        foreach (Collider collider in colliderArray) {
            if (collider.TryGetComponent(out Crate crate)) {
                crate.Damage();
            }
        }
    }

    public bool IsValidActionPosition(Vector3 actionWorldPosition) {
        return IsValidActionPosition(LevelGrid.Instance.GetGridPosition(actionWorldPosition));
    }

    public bool IsValidActionPosition(Vector2Int actionGridPosition) {
        List<Vector2Int> validActionGridPositionList = GetValidActionGridPositionList();
        return validActionGridPositionList.Contains(actionGridPosition);
    }

    public List<Vector2Int> GetValidActionGridPositionList() {
        Vector2Int currentGridPosition = unit.GetGridPosition();
        List<Vector2Int> validGridPositionList = new List<Vector2Int>();

        int grenadeRange = GRENADE_THROW_RANGE;

        for (int x = -grenadeRange; x <= grenadeRange; x++) {
            for (int y = -grenadeRange; y <= grenadeRange; y++) {
                int totalRange = Mathf.Abs(x) + Mathf.Abs(y);
                if (totalRange <= grenadeRange) {
                    Vector2Int grenadeGridPosition = currentGridPosition + new Vector2Int(x, y);
                    if (LevelGrid.Instance.IsValidGridPosition(grenadeGridPosition)) {
                        validGridPositionList.Add(grenadeGridPosition);
                    }
                }
            }
        }

        return validGridPositionList;
    }

}