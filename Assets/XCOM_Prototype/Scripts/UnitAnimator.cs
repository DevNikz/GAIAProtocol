using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimator : MonoBehaviour {

    [SerializeField] private Animator animator;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private Transform pfBulletProjectileRaycast;

    private Unit unit;
    private MoveAction moveAction;
    private bool isAiming;
    private bool isCrouching;

    private void Awake() {
        unit = GetComponent<Unit>();
    }

    private void Start() {
        moveAction = unit.GetAction<MoveAction>();
        unit.GetAction<ShootAction>().OnActionStarted += StartAiming;
        unit.GetAction<ShootAction>().OnActionComplete += StopAiming;
        unit.GetAction<ShootAction>().OnShoot += OnShoot;
        unit.GetAction<OverwatchAction>().OnShoot += OnShootOverwatch;

        //SetIsCrouching(true);
    }

    private void Update() {
        if (moveAction.IsMoving()) {
            animator.SetBool("IsCrouching", false);
            animator.SetFloat("Speed", 5f, .1f, Time.deltaTime);
        } else {
            if (!isAiming) {
                animator.SetBool("IsCrouching", isCrouching);
            }
            animator.SetFloat("Speed", 0f, .1f, Time.deltaTime);
        }

        if (isAiming) {
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * 5f));
        } else {
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0f, Time.deltaTime * 5f));
        }

        SetIsCrouching(!(unit.GetCoverType() == CoverType.None));
    }

    private void OnShoot(object sender, ShootAction.OnShootEventArgs e) {
        animator.SetTrigger("ShootSingle");

        Vector3 unitShootPosition = e.shotUnit.GetPosition();
        unitShootPosition.y = shootPoint.position.y;

        if (!e.hit) {
            // MISS!
            Vector3 missShootPosition = unitShootPosition;
            Vector3 dirToMissShootPosition = (missShootPosition - shootPoint.position).normalized;
            Vector3 missDir = Vector3.Cross(dirToMissShootPosition, Vector3.down);

            missShootPosition += missDir * .25f;
            dirToMissShootPosition = (missShootPosition - shootPoint.position).normalized;

            unitShootPosition = missShootPosition + dirToMissShootPosition * 40f;
        }

        Instantiate(pfBulletProjectileRaycast, shootPoint.position, Quaternion.identity).GetComponent<BulletProjectileRaycast>()
            .Setup(unitShootPosition);
    }

    private void OnShootOverwatch(object sender, OverwatchAction.OnShootEventArgs e) {
        animator.SetTrigger("ShootSingle");

        Vector3 unitShootPosition = e.shotUnit.GetPosition();
        unitShootPosition.y = shootPoint.position.y;

        if (!e.hit) {
            // MISS!
            Vector3 missShootPosition = unitShootPosition;
            Vector3 dirToMissShootPosition = (missShootPosition - shootPoint.position).normalized;
            Vector3 missDir = Vector3.Cross(dirToMissShootPosition, Vector3.down);

            missShootPosition += missDir * .25f;
            dirToMissShootPosition = (missShootPosition - shootPoint.position).normalized;

            unitShootPosition = missShootPosition + dirToMissShootPosition * 40f;
        }

        Instantiate(pfBulletProjectileRaycast, shootPoint.position, Quaternion.identity).GetComponent<BulletProjectileRaycast>()
            .Setup(unitShootPosition);
    }

    private void StartAiming(object sender, System.EventArgs e) {
        isAiming = true;
        animator.SetBool("IsCrouching", false);
    }

    private void StopAiming(object sender, System.EventArgs e) {
        isAiming = false;
        animator.SetBool("IsCrouching", isCrouching);
    }

    private void SetIsCrouching(bool isCrouching) {
        this.isCrouching = isCrouching;
    }
}