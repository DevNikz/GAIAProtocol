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

    [SerializeReference] private bool hasBeenInteracted;

    private void Start()
    {
        //TerminalPuzzleUI.Instance.OnPuzzleComplete += ObjectiveInteract_OnPuzzleComplete;

        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.SetInteractableAtGridPosition(gridPosition, this);
        LevelGrid.Instance.SetIngameObjectAtGridPosition(gridPosition, this.gameObject);
        Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, false);

        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    void OnDisable()
    {
        TurnSystem.Instance.OnTurnChanged -= TurnSystem_OnTurnChanged;
    }

    private void ObjectiveInteract_OnPuzzleComplete(object sender, EventArgs e)
    {
        onInteractionComplete();
        TerminalPuzzleUI.Instance.HidePuzzleUI();
    }
    
    private void TurnSystem_OnTurnChanged(object sender, EventArgs e) 
    {
        hasBeenInteracted = false;
        LevelGrid.Instance.SetInteractableAtGridPosition(gridPosition, this);
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
                    hasBeenInteracted = true;

                    //Do terminal thingy here
                    //TerminalPuzzleUI.Instance.ShowPuzzleUI();
                    LevelGrid.Instance.ClearInteractableAtGridPosition(gridPosition);
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
        if (!hasBeenInteracted)
        {
            this.onInteractionComplete = onInteractionComplete;
            isActive = true;
            hasInteracted = true;
            percentage += interactPercentageAdd;
            timer = 0.5f;
        }
        else
        {
            onInteractionComplete();
            return;
        }
    }
}