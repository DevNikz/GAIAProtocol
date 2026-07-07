using System;
using System.Collections.Generic;
using UnityEngine;

public class KaijuMoveAction : MoveAction
{
    [Header("Toxic Trail")]
    [SerializeField]
    private GameObject toxicPuddlePrefab;

    [SerializeField]
    private ParticleSystem trailVFX;

    [SerializeField]
    private int puddleDamagePerTurn = 10;

    [SerializeField]
    private int puddleTurnsUntilExpiry = 3;

    [SerializeField]
    private float particleHeightOffset = 0.05f; // sits just above the floor
    private List<GridPosition> tilesVisited = new List<GridPosition>();

    //Terrain
    private Terrain terrain;

    [SerializeField]
    private int grassDetailLayerIndex = 1;

    [SerializeField]
    private int grassDensityValue = 8; // how "thick" the grass is per cell, terrain-dependent

    [SerializeField]
    private int brushRadiusInDetailCells = 1; // how many detail cells around the center to paint

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        // Let MoveAction build positionList first
        //Debug.Log($"Heading To: {gridPosition}");
        base.TakeAction(gridPosition, onActionComplete);

        RecordVisitedTiles();

        if (trailVFX != null)
            trailVFX.Play();

        OnStopMoving += HandleMovementComplete;
    }

    private void HandleMovementComplete(object sender, EventArgs e)
    {
        OnStopMoving -= HandleMovementComplete;

        if (trailVFX != null)
            trailVFX.Stop();

        SpawnPuddles();
    }

    private void RecordVisitedTiles()
    {
        tilesVisited.Clear();
        foreach (Vector3 worldPos in positionList)
        {
            GridPosition gridPos = LevelGrid.Instance.GetGridPosition(worldPos);
            if (!tilesVisited.Contains(gridPos))
                tilesVisited.Add(gridPos);
        }
    }

    private void SpawnPuddles()
    {
        foreach (GridPosition gridPos in tilesVisited)
        {
            // Don't stack puddles on the same tile
            if (LevelGrid.Instance.HasToxicPuddle(gridPos))
                continue;

            Vector3 worldPos = LevelGrid.Instance.GetWorldPosition(gridPos);
            worldPos.y += particleHeightOffset;
            GameObject puddleGO = Instantiate(
                toxicPuddlePrefab,
                worldPos,
                Quaternion.Euler(0, UnityEngine.Random.Range(0f, 360f), 0f)
            );
            ToxicPuddle puddle = puddleGO.GetComponent<ToxicPuddle>();
            puddle.Initialize(gridPos, puddleTurnsUntilExpiry, puddleDamagePerTurn);

            LevelGrid.Instance.RegisterToxicPuddle(gridPos, puddle); // see note below
        }
    }
}
