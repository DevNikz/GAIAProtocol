using System;
using System.Collections.Generic;
using ForestBiome;
using UnityEngine;

[RequireComponent(typeof(PumpjackAnimator))]
public class ObjectivePumpFill : ObjectiveBase, IInteractable
{
    [SerializeField]
    OilFlowManager oilManager;

    [SerializeField]
    string pipeID;

    [SerializeField]
    private float percentage;
    private float timer;

    [SerializeField]
    private bool disableInteract;

    private Action onInteractionComplete;

    [SerializeField, Range(1f, 10f)]
    float modifier = 1.0f;

    [SerializeField]
    PumpjackAnimator pump;

    [SerializeField]
    List<ToxicPuddle> toxicPuddle;

    void Start()
    {
        RegisterOnGrid();
        pump = GetComponent<PumpjackAnimator>();
        if (pipeID != "")
            oilManager.BeginOverflow(pipeID);
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    protected override void OnEnable()
    {
        // ObjectiveManager.Instance.Register(this);
    }

    protected override void OnDisable()
    {
        // if (ObjectiveManager.Instance != null)
        // {
        //     Debug.Log($"Unregistering this objective: {GetDisplayName()} | {objectiveIndex}");
        //     ObjectiveManager.Instance.Unregister(this);
        // }
        TurnSystem.Instance.OnTurnChanged -= TurnSystem_OnTurnChanged;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            isBeingInteracted = false;
        }
    }

    private void Update()
    {
        if (!isComplete) //isComplete false
            UpdateObjective();
        else
        {
            if (!disableInteract)
            {
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    pump.SetEnabled(false);
                    CleanPipeline();

                    UnregisterFromGrid();

                    disableInteract = true;
                    onInteractionComplete?.Invoke();
                }
            }
        }
    }

    void CleanPipeline()
    {
        for (int i = 0; i < toxicPuddle.Count; i++)
            toxicPuddle[i].GetComponent<ToxicPuddle>().Expire();

        if (pipeID != "")
            oilManager.StopOverflow(pipeID);
    }

    void UpdateObjective()
    {
        if (percentage < 1.0f)
        {
            if (!isBeingInteracted)
                return;
            else
            {
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    isBeingInteracted = false;
                    onInteractionComplete?.Invoke();
                }
            }
        }
        else
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                isBeingInteracted = false;
                UnregisterFromGrid();
                isComplete = true;
                onInteractionComplete?.Invoke();
            }
        }
    }

    public void Interact(Action onInteractionComplete) { }

    public void Interact(Action onInteractionComplete, float percentageAdd)
    {
        if (isComplete)
            return;

        var counter = GetSharedObjective() as ObjectiveCounter;
        if (counter == null)
        {
            Debug.LogWarning(
                $"ObjectiveCounterTarget on '{name}' couldn't find an ObjectiveCounter registered with index {objectiveIndex}. Make sure one exists in the scene.",
                this
            );
            return;
        }

        SoundManager.Instance.PlaySFX("ObjectiveInteract");

        this.onInteractionComplete = onInteractionComplete;
        isBeingInteracted = true;
        percentage += percentageAdd * modifier;
        timer = 0.5f;

        if (percentage >= 1.0f)
        {
            SoundManager.Instance.PlaySFX("ObjectiveComplete");
            counter.Increment(1);
        }
    }

    public void Interact(Action onInteractionComplete, Unit unit) { }

    public void SetInteracting(bool value)
    {
        isBeingInteracted = value;
    }

    // public override float GetProgress() => percentage;
    public ObjectiveBase GetSharedObjective() =>
        ObjectiveManager.Instance.GetObjective(objectiveIndex);

    public override float GetProgress() => percentage;

    protected override void RegisterOnGrid()
    {
        Bounds bounds = GetObjectiveBounds();

        GridPosition minGridPosition = LevelGrid.Instance.GetGridPosition(bounds.min);
        GridPosition maxGridPosition = LevelGrid.Instance.GetGridPosition(bounds.max);

        int minX = Mathf.Min(minGridPosition.x, maxGridPosition.x);
        int maxX = Mathf.Max(minGridPosition.x, maxGridPosition.x);
        int minZ = Mathf.Min(minGridPosition.z, maxGridPosition.z);
        int maxZ = Mathf.Max(minGridPosition.z, maxGridPosition.z);

        for (int x = minX; x <= maxX; x++)
        {
            for (int z = minZ; z <= maxZ; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z, 0);

                LevelGrid.Instance.SetInteractableAtGridPosition(gridPosition, this);
                LevelGrid.Instance.SetIngameObjectAtGridPosition(gridPosition, this.gameObject);
                Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, false);

                occupiedGridPositions.Add(gridPosition);
            }
        }
    }

    protected override void UnregisterFromGrid()
    {
        foreach (GridPosition gridPosition in occupiedGridPositions)
        {
            LevelGrid.Instance.ClearInteractableAtGridPosition(gridPosition);
            LevelGrid.Instance.ClearIngameObjectAtGridPosition(gridPosition);
            // Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, true);
        }
        occupiedGridPositions?.Clear();
    }
}
