using System;
using System.Threading;
using UnityEngine;

public class ObjectiveInteract : MonoBehaviour, IInteractable
{
    [SerializeField, Range(0.1f, 1f)] private float interactPercentageAdd;

    private GridPosition gridPosition;
    private Action onInteractionComplete;
    private bool hasInteracted;
    public float percentage = 0.0f;
    private float timer;
    private bool objectiveComplete;
    [SerializeField] private bool disableInteract;

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

    // private void ObjectiveInteract_OnPuzzleComplete(object sender, EventArgs e)
    // {
    //     onInteractionComplete();
    //     TerminalPuzzleUI.Instance.HidePuzzleUI();
    // }
    
    private void TurnSystem_OnTurnChanged(object sender, EventArgs e) 
    {
        if(!TurnSystem.Instance.IsPlayerTurn())
        {
            hasBeenInteracted = false;
            if(!objectiveComplete) LevelGrid.Instance.SetInteractableAtGridPosition(gridPosition, this);
            //Debug.Log("Set Interactable at Grid Pos");
        }
    }

    private void Update()
    {
        if(!objectiveComplete) UpdateObjective();
        else
        {
            if(!disableInteract) {
                LevelGrid.Instance.ClearInteractableAtGridPosition(gridPosition);
                LevelGrid.Instance.ClearIngameObjectAtGridPosition(gridPosition);
                disableInteract = true;
            }
        }
    }

    void UpdateObjective()
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
                hasInteracted = false;
                LevelGrid.Instance.ClearInteractableAtGridPosition(gridPosition);
                LevelGrid.Instance.ClearIngameObjectAtGridPosition(gridPosition);
                objectiveComplete = true;
                onInteractionComplete();
            }
        }
    }

    public void Interact(Action onInteractionComplete)
    {
        this.onInteractionComplete = onInteractionComplete;
        hasInteracted = true;
        percentage += interactPercentageAdd;
        timer = 0.5f;
    }
}