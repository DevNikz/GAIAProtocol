using System;
using UnityEngine;

/// <summary>
/// Sits on each scattered world instance of a counter objective (e.g. one
/// specific crystal out of five). Not an ObjectiveBase itself - just points at
/// the shared ObjectiveCounter by index. On interact, reports one increment,
/// then disables itself so it can't be triggered twice.
/// </summary>
public class ObjectiveCounterTarget : ObjectiveBase, IInteractable
{
    // [SerializeField]
    // private int objectiveIndex;
    [SerializeField]
    CrystalHarvestManager crystalHarvestManager;

    [SerializeField, Min(1)]
    private int incrementAmount = 1;

    [SerializeField]
    private SoundController soundController;

    private bool hasBeenCollected;

    /// <summary>Used by ObjectiveWorldUI to find what progress to display for this instance.</summary>
    public ObjectiveBase GetSharedObjective() =>
        ObjectiveManager.Instance.GetObjective(objectiveIndex);

    void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.SetInteractableAtGridPosition(gridPosition, this);
        LevelGrid.Instance.SetIngameObjectAtGridPosition(gridPosition, this.gameObject);
        Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, false);

        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    protected override void OnEnable()
    {
        ObjectiveManager.Instance.Register(this);
    }

    protected override void OnDisable()
    {
        if (ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.Unregister(this);
        }
        TurnSystem.Instance.OnTurnChanged -= TurnSystem_OnTurnChanged;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            isBeingInteracted = false;
            if (!isComplete)
                LevelGrid.Instance.SetInteractableAtGridPosition(gridPosition, this);
        }
    }

    public void Interact(Action onInteractionComplete)
    {
        if (hasBeenCollected)
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

        if (soundController != null)
            SoundManager.Instance.PlaySFX("Harvest");

        hasBeenCollected = true;
        counter.Increment(incrementAmount);

        // Swap for Destroy(gameObject) if you want the instance to disappear
        // entirely rather than just go inactive.

        if (crystalHarvestManager != null)
        {
            Debug.Log("Harvested Crystal/Plant!");
            crystalHarvestManager.HarvestAll();
        }
        else
            gameObject.SetActive(false);

        LevelGrid.Instance.ClearInteractableAtGridPosition(gridPosition);
        LevelGrid.Instance.ClearIngameObjectAtGridPosition(gridPosition);
        Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, true);
        onInteractionComplete?.Invoke();
    }

    public void Interact(Action onInteractionComplete, float percentageAdd) { }

    public override float GetProgress() => GetSharedObjective()?.GetProgress() ?? 0f;
}
