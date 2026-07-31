using System;
using UnityEngine;

public class KaijuObjective : ObjectiveBase, IInteractable
{
    [SerializeField]
    private HealthSystem healthSystem;

    [SerializeField]
    private KaijuUnit kaijuUnit;

    [SerializeField]
    private SoundController soundController;

    private bool isDowned;

    [SerializeField]
    HUDMarkerInWorldTarget markerUI;

    void Awake()
    {
        if (healthSystem == null)
            healthSystem = GetComponent<HealthSystem>();
        if (kaijuUnit == null)
            kaijuUnit = GetComponent<KaijuUnit>();
    }

    void Start()
    {
        healthSystem.OnDead += HealthSystem_OnDead;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (healthSystem != null)
            healthSystem.OnDead -= HealthSystem_OnDead;
    }

    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        if (isDowned)
            return;
        isDowned = true;

        kaijuUnit.enabled = false; // stops Update() -> detection + AI. GameObject/mesh stay as-is.
        //mesh_dead.SetActive(true);
        markerUI.enabled = true;
        RegisterOnGrid(); // only becomes a valid interact target now that it's downed

        //SoundManager.Instance.PlaySFX("KaijuDowned");
    }

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
        for (int z = minZ; z <= maxZ; z++)
        {
            GridPosition gridPosition = new GridPosition(x, z, 0);
            LevelGrid.Instance.SetInteractableAtGridPosition(gridPosition, this);
            LevelGrid.Instance.SetIngameObjectAtGridPosition(gridPosition, this.gameObject);
            occupiedGridPositions.Add(gridPosition);
        }
    }

    protected override void UnregisterFromGrid()
    {
        foreach (GridPosition gridPosition in occupiedGridPositions)
        {
            LevelGrid.Instance.ClearInteractableAtGridPosition(gridPosition);
            LevelGrid.Instance.ClearIngameObjectAtGridPosition(gridPosition);
        }
        occupiedGridPositions.Clear();
    }

    public void Interact(Action onInteractionComplete)
    {
        if (!isDowned || isComplete)
        {
            onInteractionComplete?.Invoke();
            return;
        }

        if (soundController != null)
            SoundManager.Instance.PlaySFX("ObjectiveComplete");

        UnregisterFromGrid();
        CompleteObjective();
        onInteractionComplete?.Invoke();
    }

    public void Interact(Action onInteractionComplete, float percentageAdd) { }

    public void Interact(Action onInteractionComplete, Unit interactingUnit) { }

    public override float GetProgress()
    {
        if (isComplete)
            return 1f;
        if (isDowned)
            return 0.99f; // downed, waiting on the finishing interact
        return healthSystem != null ? 1f - healthSystem.GetHealthNormalized() : 0f;
    }
}
