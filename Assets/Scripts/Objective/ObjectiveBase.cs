using System;
using UnityEngine;

public enum ObjectiveType
{
    Main,
    Side,
}

public abstract class ObjectiveBase : MonoBehaviour
{
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
            return;

        isComplete = true;
        if (ObjectiveManager.Instance.CheckIndex(objectiveIndex))
        {
            ObjectiveManager.Instance.SetComplete(objectiveIndex);
        }
    }
}
