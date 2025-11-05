using System;
using System.Threading;
using UnityEngine;

public class ObjectiveInteract : MonoBehaviour, IInteractable
{
    [SerializeField, Range(0.1f, 1f)] private float interactPercentageAdd;

    private GridPosition gridPosition;
    private Action onInteractionComplete;
    private bool isActive;
    public float percentage = 0.0f;
    private float timer;

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.SetInteractableAtGridPosition(gridPosition, this);
        LevelGrid.Instance.SetIngameObjectAtGridPosition(gridPosition, this.gameObject);
        Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, false);
    }

    private void Update()
    {
        if (percentage < 1.0f)
        {
            if (!isActive) return;

            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                isActive = false;

                //LevelGrid.Instance.ClearInteractableAtGridPosition(gridPosition);
                onInteractionComplete();
            }
        }
        else
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                isActive = false;

                LevelGrid.Instance.ClearInteractableAtGridPosition(gridPosition);
                LevelGrid.Instance.ClearIngameObjectAtGridPosition(gridPosition);
                onInteractionComplete();
            }
        }

    }

    public void Interact(Action onInteractionComplete)
    {
        this.onInteractionComplete = onInteractionComplete;
        isActive = true;
        percentage += interactPercentageAdd;
        timer = 0.5f;
    }
}