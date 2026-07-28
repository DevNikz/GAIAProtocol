using System;
using UnityEngine;

public class ObjectiveInteractFill : ObjectiveBase, IInteractable
{
    [SerializeField, Range(0.1f, 5f)]
    private float fillRatePerInteract = 1f;

    [SerializeField]
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
        if (!isComplete) //isComplete false
            UpdateObjective();
        else
        {
            if (!disableInteract)
            {
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    if (HasRadarScan)
                        radarScan.TriggerScan(transform.position);

                    LevelGrid.Instance.ClearInteractableAtGridPosition(gridPosition);
                    LevelGrid.Instance.ClearIngameObjectAtGridPosition(gridPosition);
                    disableInteract = true;
                    onInteractionComplete?.Invoke();
                }
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
                    // LevelGrid.Instance.ClearInteractableAtGridPosition(gridPosition);
                    onInteractionComplete?.Invoke();
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
                onInteractionComplete?.Invoke();
            }
        }
    }

    public void Interact(Action onInteractionComplete) { }

    public void Interact(Action onInteractionComplete, float percentageAdd)
    {
        if (isComplete)
            return;

        if (soundController != null)
            SoundManager.Instance.PlaySFX("ObjectiveInteract");

        this.onInteractionComplete = onInteractionComplete;
        isBeingInteracted = true;
        percentage += percentageAdd;
        timer = 0.5f;

        if (percentage == 1.0f)
        {
            if (soundController != null)
                SoundManager.Instance.PlaySFX("ObjectiveComplete");
            CompleteObjective();
        }
    }

    public void SetInteracting(bool value)
    {
        isBeingInteracted = value;
    }

    public override float GetProgress() => percentage;
}
