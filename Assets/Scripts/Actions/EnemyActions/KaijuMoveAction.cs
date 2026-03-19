using System;
using System.Collections.Generic;
using UnityEngine;

public class KaijuMoveAction : MoveAction
{
    [Header("Toxic Trail")]
    [SerializeField] private GameObject toxicPuddlePrefab;
    [SerializeField] private ParticleSystem trailVFX;
    [SerializeField] private int puddleDamagePerTurn = 10;
    [SerializeField] private int puddleTurnsUntilExpiry = 3;
    [SerializeField] private float particleHeightOffset = 0.05f; // sits just above the floor

    private List<GridPosition> tilesVisited = new List<GridPosition>();
    private GridPosition lastRecordedPosition;

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        // Let MoveAction build positionList first
        base.TakeAction(gridPosition, onActionComplete);

        // Pull the planned path directly from positionList — no Update() needed
        tilesVisited.Clear();
        foreach (Vector3 worldPos in positionList)
        {
            GridPosition gridPos = LevelGrid.Instance.GetGridPosition(worldPos);
            if (!tilesVisited.Contains(gridPos))
                tilesVisited.Add(gridPos);
        }

        if (trailVFX != null)
            trailVFX.Play();

        OnStopMoving += HandleMovementComplete;
    }

    // public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    // {
    //     tilesVisited.Clear();
    //     lastRecordedPosition = unit.GetGridPosition();
    //     tilesVisited.Add(lastRecordedPosition);

    //     if (trailVFX != null)
    //         trailVFX.Play();

    //     OnStopMoving += HandleMovementComplete;

    //     Debug.Log("Kaiju Moved");
    //     base.TakeAction(gridPosition, onActionComplete);
    // }

    // private void Update()
    // {
    //     if (!isActive) {
    //         return;
    //     }

    //     // Record each new tile entered during movement
    //     GridPosition current = LevelGrid.Instance.GetGridPosition(transform.position);
    //     if (current != lastRecordedPosition)
    //     {
    //         if (!tilesVisited.Contains(current))
    //             tilesVisited.Add(current);

    //         lastRecordedPosition = current;
    //     }
    // }

    private void HandleMovementComplete(object sender, EventArgs e)
    {
        OnStopMoving -= HandleMovementComplete;

        if (trailVFX != null)
            trailVFX.Stop();

        SpawnPuddles();
    }

    private void SpawnPuddles()
    {
        foreach (GridPosition gridPos in tilesVisited)
        {
            // Don't stack puddles on the same tile
            if (LevelGrid.Instance.HasToxicPuddle(gridPos)) continue;

            Vector3 worldPos = LevelGrid.Instance.GetWorldPosition(gridPos);
            worldPos.y += particleHeightOffset;
            GameObject puddleGO = Instantiate(toxicPuddlePrefab, worldPos, Quaternion.Euler(0, UnityEngine.Random.Range(0f, 360f), 0f));
            ToxicPuddle puddle = puddleGO.GetComponent<ToxicPuddle>();
            puddle.Initialize(gridPos, puddleTurnsUntilExpiry, puddleDamagePerTurn);

            LevelGrid.Instance.RegisterToxicPuddle(gridPos, puddle); // see note below
        }
    }
}