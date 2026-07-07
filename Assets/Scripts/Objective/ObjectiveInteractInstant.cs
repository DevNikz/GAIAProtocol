using UnityEngine;

/// <summary>
/// Objective type 2: a single interaction completes it immediately -
/// no bar to fill, just "interact once, done". Call Interact() from
/// whatever triggers your interact input (e.g. an ObjectiveInteractable
/// component, a trigger collider, etc).
/// </summary>
public class ObjectiveInteractInstant : ObjectiveBase
{
    public override float GetProgress() => isComplete ? 1f : 0f;

    public void Interact()
    {
        if (isComplete)
            return;

        CompleteObjective();
    }
}
