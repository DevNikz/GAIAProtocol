using System;
using UnityEngine;

public class ObjectiveInteractFill : ObjectiveBase, IInteractable
{
    [SerializeField, Range(0.1f, 5f)]
    private float fillRatePerInteract = 1f;
    private float percentage;
    private float timer;

    [SerializeField]
    private bool disableInteract;

    private Action onInteractionComplete;

    [SerializeField]
    private SoundController soundController;
    RadarScanEffect radarScan;

    [SerializeField]
    bool HasRadarScan = false;

    void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.SetInteractableAtGridPosition(gridPosition, this);
        LevelGrid.Instance.SetIngameObjectAtGridPosition(gridPosition, this.gameObject);
        Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, false);

        if (HasRadarScan)
            radarScan = GetComponent<RadarScanEffect>();

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

    private void Update()
    {
        /*
        if (isComplete)
            return;

        // percentage = isBeingInteracted
        //     ? Mathf.Clamp01(percentage + fillRatePerInteract * Time.deltaTime)
        //     : Mathf.Clamp01(percentage - 0f * Time.deltaTime);

        if (percentage >= 1f)
            CompleteObjective();
        */
        if (!isComplete)
            UpdateObjective();
        else
        {
            if (!disableInteract)
            {
                SoundManager.Instance.PlaySFX("ObjectiveInteract");
                //soundController.PlaySound(5);
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
            if (!isBeingInteracted)
                return;
            else
            {
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    isBeingInteracted = false;
                    //hasBeenInteracted = true;

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
                isBeingInteracted = false;
                LevelGrid.Instance.ClearInteractableAtGridPosition(gridPosition);
                LevelGrid.Instance.ClearIngameObjectAtGridPosition(gridPosition);
                isComplete = true;
                onInteractionComplete();
            }
        }
    }

    public void Interact(Action onInteractionComplete) { }

    public void Interact(Action onInteractionComplete, float percentageAdd)
    {
        if (soundController != null)
        {
            SoundManager.Instance.PlaySFX("ObjectiveComplete");
            //soundController.PlaySound(4);
        }

        this.onInteractionComplete = onInteractionComplete;
        isBeingInteracted = true;
        percentage += percentageAdd;
        timer = 0.5f;

        if (HasRadarScan)
            radarScan.TriggerScan(transform.position);
    }

    public void SetInteracting(bool value)
    {
        isBeingInteracted = value;
    }

    public override float GetProgress() => percentage;
}
