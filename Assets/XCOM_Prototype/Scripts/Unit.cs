using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    private const int ACTION_POINTS_DEFAULT = 2;

    public static event EventHandler OnUnitSpawned;
    public static event EventHandler OnUnitDead;


    public event EventHandler<IUnitAction> OnUnitActionCompleted; // Triggered when current action completes
    public event EventHandler<int> OnActionPointsChanged;
    public event EventHandler OnCoverTypeChanged;
    public event EventHandler OnUnitMovedGridPosition;


    [SerializeField] private Transform pfUnitRagdoll;
    [SerializeField] private Transform avatarRoot;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
    [SerializeField] private Material friendlyMaterial;
    [SerializeField] private Material enemyMaterial;
    [SerializeField] private bool isEnemy;


    private IUnitAction[] unitActionArray; // All Unit Actions attached to this Unit
    private IUnitAction activeUnitAction; // Currently active action
    private Vector2Int gridPosition; // Grid Position where this Unit is on
    private int actionPoints;
    private HealthSystem healthSystem;
    private CoverType coverType;

    private void Awake() {
        healthSystem = new HealthSystem(100);
        healthSystem.OnDead += HealthSystem_OnDead;

        unitActionArray = GetComponents<IUnitAction>();

        foreach (IUnitAction unitAction in unitActionArray) {
            unitAction.Setup(this);
        }

        skinnedMeshRenderer.material = isEnemy ? enemyMaterial : friendlyMaterial;
    }

    private void Start() {
        gridPosition = LevelGrid.Instance.GetGridPosition(GetPosition());
        LevelGrid.Instance.SetUnitAtGridPosition(gridPosition, this);
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;

        UpdateCoverType();

        actionPoints = ACTION_POINTS_DEFAULT;
        OnActionPointsChanged?.Invoke(this, actionPoints);

        OnUnitSpawned?.Invoke(this, EventArgs.Empty);
    }

    private void HealthSystem_OnDead(object sender, EventArgs e) {
        LevelGrid.Instance.ClearUnitAtGridPosition(gridPosition, this);
        TurnSystem.Instance.OnTurnChanged -= TurnSystem_OnTurnChanged;

        Transform unitRagdoll = Instantiate(pfUnitRagdoll, transform.position, transform.rotation);
        unitRagdoll.GetComponent<UnitRagdoll>().Setup(avatarRoot, skinnedMeshRenderer.material, transform.position);

        OnUnitDead?.Invoke(this, EventArgs.Empty);

        Destroy(gameObject);
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e) {
        if (TurnSystem.Instance.IsPlayerTurn()) {
            actionPoints = ACTION_POINTS_DEFAULT;
            OnActionPointsChanged?.Invoke(this, actionPoints);
        }
    }

    private void Update() {
        Vector2Int currentGridPosition = LevelGrid.Instance.GetGridPosition(GetPosition());
        if (currentGridPosition != gridPosition) {
            // Changed grid position
            LevelGrid.Instance.UnitMovedGridPosition(this, gridPosition, currentGridPosition);
            // Update this grid position
            gridPosition = currentGridPosition;

            OnUnitMovedGridPosition?.Invoke(this, EventArgs.Empty);

            UpdateCoverType();
        }
    }

    private void UpdateCoverType() {
        coverType = LevelGrid.Instance.GetUnitCoverType(GetPosition());
        OnCoverTypeChanged?.Invoke(this, EventArgs.Empty);
    }

    public CoverType GetCoverType() {
        return coverType;
    }

    public void SpendActionPoint() {
        actionPoints--;
        OnActionPointsChanged?.Invoke(this, actionPoints);
    }

    public void SetActiveUnitAction(IUnitAction activeUnitAction) {
        this.activeUnitAction = activeUnitAction;
    }

    public void UnitActionComplete(IUnitAction unitAction) {
        OnUnitActionCompleted?.Invoke(this, unitAction);
    }

    public Vector3 GetPosition() {
        return transform.position;
    }

    public Vector2Int GetGridPosition() {
        return gridPosition;
    }

    public T GetAction<T>() {
        foreach (IUnitAction unitAction in unitActionArray) {
            if (unitAction is T) {
                return (T)unitAction;
            }
        }

        return default;
    }

    public IUnitAction[] GetUnitActionArray() {
        return unitActionArray;
    }

    public bool TrySpendActionPointsToTakeAction(IUnitAction unitAction) {
        if (actionPoints > 0) {
            SpendActionPoint();
            return true;
        } else {
            return false;
        }
    }

    public bool IsEnemyAIActive() {
        foreach (Unit playerUnit in UnitManager.Instance.GetFriendlyUnitList()) {
            int activateAIRange = 20;
            if (GetGridDistance(playerUnit.GetGridPosition()) < activateAIRange) {
                // There's a Player Unit within the Active range, activate AI
                return true;
            }
        }
        return false;
    }

    private int GetGridDistance(Vector2Int testGridPosition) {
        Vector2Int currentGridPosition = GetGridPosition();
        Vector2Int gridVector = testGridPosition - currentGridPosition;
        int gridDistance = Mathf.Abs(gridVector.x) + Mathf.Abs(gridVector.y);
        return gridDistance;
    }

    public int GetActionPoints() {
        return actionPoints;
    }

    public HealthSystem GetHealthSystem() {
        return healthSystem;
    }

    public bool IsEnemy() {
        return isEnemy;
    }

    public bool IsVisible() {
        return transform.Find("Visual").gameObject.activeSelf;
    }

    public void HideVisual() {
        transform.Find("Visual").gameObject.SetActive(false);
        transform.Find("UnitWorldUI").gameObject.SetActive(false);
    }

    public void ShowVisual() {
        transform.Find("Visual").gameObject.SetActive(true);
        transform.Find("UnitWorldUI").gameObject.SetActive(true);
    }

}