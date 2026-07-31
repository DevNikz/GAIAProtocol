using System;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectiveType
{
    Main,
    Side,
}

public abstract class ObjectiveBase : MonoBehaviour
{
    [SerializeField]
    private Transform mergedGridVisualPrefab; // a 1x1 unit quad, same material/style as your normal grid tiles
    private Transform mergedGridVisualInstance;
    protected List<GridPosition> occupiedGridPositions = new List<GridPosition>();

    // ObjectiveBase.cs (addition)
    [SerializeField]
    private string displayName = "Objective";

    public virtual string GetDisplayName() => displayName;

    [SerializeField]
    protected int objectiveIndex;

    [SerializeField]
    protected string objectiveDesc;

    [SerializeField]
    protected ObjectiveType objectiveType;

    [SerializeField]
    protected bool isComplete;
    protected GridPosition gridPosition;
    protected bool isBeingInteracted;

    public int GetObjectiveIndex() => objectiveIndex;

    public ObjectiveType GetObjectiveType() => objectiveType;

    public string GetObjectiveDesc() => objectiveDesc;

    public bool IsComplete() => isComplete;

    public abstract float GetProgress();

    protected virtual void OnEnable()
    {
        ObjectiveManager.Instance.Register(this);
    }

    protected virtual void OnDisable()
    {
        if (ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.Unregister(this);
        }
    }

    protected virtual void CompleteObjective()
    {
        if (isComplete)
        {
            Debug.Log($"{displayName} {objectiveIndex} is already Complete");
            return;
        }

        isComplete = true;
        if (ObjectiveManager.Instance.CheckIndex(objectiveIndex))
        {
            ObjectiveManager.Instance.SetComplete(objectiveIndex);
        }
    }

    protected virtual void RegisterOnGrid()
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

                // LevelGrid.Instance.SetInteractableAtGridPosition(gridPosition, this);
                // LevelGrid.Instance.SetIngameObjectAtGridPosition(gridPosition, this.gameObject);
                Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, false);

                occupiedGridPositions.Add(gridPosition);
            }
        }
    }

    protected virtual Bounds GetObjectiveBounds()
    {
        if (TryGetComponent<BoxCollider>(out BoxCollider boxCollider))
            return boxCollider.bounds;

        if (TryGetComponent<Collider>(out Collider anyCollider))
            return anyCollider.bounds;

        // Fallback for objects with no collider — treat as single cell
        return new Bounds(transform.position, Vector3.one * 0.1f);
    }

    protected virtual void UnregisterFromGrid()
    {
        foreach (GridPosition gridPosition in occupiedGridPositions)
        {
            LevelGrid.Instance.ClearInteractableAtGridPosition(gridPosition);
            LevelGrid.Instance.ClearIngameObjectAtGridPosition(gridPosition);
            Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, true);
        }
        if (occupiedGridPositions != null)
            occupiedGridPositions.Clear();
    }

    protected virtual void SetupMergedGridVisual()
    {
        Bounds bounds = GetObjectiveBounds();

        if (mergedGridVisualInstance == null)
            mergedGridVisualInstance = Instantiate(mergedGridVisualPrefab);

        Vector3 center = bounds.center;
        mergedGridVisualInstance.position = new Vector3(
            center.x,
            mergedGridVisualInstance.position.y,
            center.z
        );
        mergedGridVisualInstance.localScale = new Vector3(bounds.size.x, 1f, bounds.size.z);
        mergedGridVisualInstance.gameObject.SetActive(false);
    }

    protected virtual void TeardownMergedGridVisual()
    {
        if (mergedGridVisualInstance != null)
        {
            Destroy(mergedGridVisualInstance.gameObject);
            mergedGridVisualInstance = null;
        }
    }
}
