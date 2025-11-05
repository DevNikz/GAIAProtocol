using System;
using System.Threading;
using UnityEngine;

public class ObjectiveInteract : MonoBehaviour, IInteractable
{
    [SerializeField, Range(0.1f, 1f)] private float interactPercentageAdd;

    private GridPosition gridPosition;
    private Action onInteractionComplete;
    private bool isActive;
    private bool hasInteracted;
    public float percentage = 0.0f;
    private float timer;

    private void Start()
    {
        //TerminalPuzzleUI.Instance.OnPuzzleComplete += ObjectiveInteract_OnPuzzleComplete;

        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.SetInteractableAtGridPosition(gridPosition, this);
        LevelGrid.Instance.SetIngameObjectAtGridPosition(gridPosition, this.gameObject);
        Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, false);
    }
    
    private void ObjectiveInteract_OnPuzzleComplete(object sender, EventArgs e)
    {
        onInteractionComplete();
        TerminalPuzzleUI.Instance.HidePuzzleUI();
    }

    private void Update()
    {
        if (percentage < 1.0f)
        {
            if (!hasInteracted) return;
            else
            {
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    hasInteracted = false;

                    //LevelGrid.Instance.ClearInteractableAtGridPosition(gridPosition);

                    //Do terminal thingy here
                    //TerminalPuzzleUI.Instance.ShowPuzzleUI();
                    onInteractionComplete();
                }
            }
        }
        else
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                isActive = false;
                hasInteracted = false;
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
        hasInteracted = true;
        percentage += interactPercentageAdd;
        timer = 0.5f;
    }
}