using System;
using System.Collections.Generic;
using UnityEngine;

public class KaijuUnit : MonoBehaviour
{

    public event EventHandler<OnPlayerDetectedEventArgs> OnPlayerDetected;
    public event EventHandler<OnPlayerLostEventArgs> OnPlayerLost;

    public class OnPlayerDetectedEventArgs : EventArgs
    {
        public Unit detectedUnit;
    }

    public class OnPlayerLostEventArgs : EventArgs
    {
        public GridPosition lastSeenGridPosition;
    }
    
    [SerializeField] bool isAwake;
    public bool IsAwake() { return isAwake; }
    public void SetAwake(bool value) { isAwake = value;}

    [Header("Detection Settings")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float losCheckInterval = 0.2f;   // seconds between raycasts
    [SerializeField] private LayerMask obstacleLayerMask;
    [SerializeField] private bool requireLineOfSight = true;

    private Unit ownerUnit;
    private bool isPlayerDetected = false;
    private Unit currentTarget;
    private float losTimer;

    private void Awake()
    {
        ownerUnit = GetComponent<Unit>();
    }

    private void Update()
    {
        // Only run detection checks on the enemy's turn (or always, depending on your preference).
        // Running every frame is fine for small unit counts; use losCheckInterval to throttle raycasts.
        losTimer -= Time.deltaTime;
        if (losTimer > 0f) return;
        losTimer = losCheckInterval;

        CheckDetection();
    }

    private void CheckDetection()
    {
        Unit closestVisibleEnemy = FindClosestVisiblePlayerUnit();

        if (closestVisibleEnemy != null)
        {
            if (!isPlayerDetected)
            {
                // Just spotted a player unit
                isPlayerDetected = true;
                if(isAwake == false) isAwake = true;
                currentTarget = closestVisibleEnemy;
                OnPlayerDetected?.Invoke(this, new OnPlayerDetectedEventArgs { detectedUnit = currentTarget });
            }
            else
            {
                // Already tracking — update target if a closer one appeared
                currentTarget = closestVisibleEnemy;
            }
        }
        else
        {
            if (isPlayerDetected)
            {
                // Lost the player
                GridPosition lastSeen = currentTarget != null
                    ? currentTarget.GetGridPosition()
                    : ownerUnit.GetGridPosition();

                isPlayerDetected = false;
                OnPlayerLost?.Invoke(this, new OnPlayerLostEventArgs { lastSeenGridPosition = lastSeen });
                currentTarget = null;
            }
        }
    }

    private Unit FindClosestVisiblePlayerUnit()
    {
        List<Unit> playerUnits = UnitManager.Instance.GetFriendlyUnitList();
        Unit closest = null;
        float closestDist = float.MaxValue;

        foreach (Unit playerUnit in playerUnits)
        {
            float dist = Vector3.Distance(transform.position, playerUnit.transform.position);
            if (dist > detectionRange) continue;

            if (requireLineOfSight && !HasLineOfSight(playerUnit)) continue;

            if (dist < closestDist)
            {
                closestDist = dist;
                closest = playerUnit;
            }
        }

        return closest;
    }

    private bool HasLineOfSight(Unit target)
    {
        Vector3 origin = transform.position + Vector3.up * 1.5f;   // eye-level
        Vector3 targetPos = target.transform.position + Vector3.up * 1.5f;
        Vector3 direction = (targetPos - origin).normalized;
        float distance = Vector3.Distance(origin, targetPos);

        return !Physics.Raycast(origin, direction, distance, obstacleLayerMask);
    }

    // ── Public helpers used by EnemyAI ─────────────────────────────────────

    public bool IsPlayerDetected() => isPlayerDetected;

    public Unit GetCurrentTarget() => currentTarget;

    /// <summary>Draws the detection range in the Scene view for easier debugging.</summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = isPlayerDetected ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}