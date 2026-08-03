using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Unit : MonoBehaviour
{
    [Header("Grid Footprint")]
    [SerializeField]
    private Collider footprintCollider; // leave null for normal 1x1 units; assign on Kaiju for its multi-tile footprint

    private const int ACTION_POINTS_MAX = 4;

    public static event EventHandler OnAnyActionPointsChanged;
    public static event EventHandler OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDead;

    [SerializeField]
    private bool isEnemy;

    private GridPosition gridPosition;
    private HealthSystem healthSystem;
    private BaseAction[] baseActionArray;
    public int actionPoints;

    int customHP;

    [SerializeField]
    int customAP;
    bool hasRegenHP;
    bool hasCorruptionResist;

    [SerializeField]
    bool hasCorruptionImmune;
    bool hasMeleeAction;

    [SerializeField]
    UnitWorldUI unitWorldUI;

    private List<GridPosition> occupiedGridPositions = new();
    private readonly Dictionary<GridPosition, bool> footprintOriginalWalkable = new();

    private List<GridPosition> GetFootprintGridPositions()
    {
        List<GridPosition> positions = new List<GridPosition>();

        if (footprintCollider == null)
        {
            positions.Add(LevelGrid.Instance.GetGridPosition(transform.position));
            return positions;
        }

        Bounds bounds = footprintCollider.bounds;

        GridPosition minGridPosition = LevelGrid.Instance.GetGridPosition(bounds.min);
        GridPosition maxGridPosition = LevelGrid.Instance.GetGridPosition(bounds.max);

        int minX = Mathf.Min(minGridPosition.x, maxGridPosition.x);
        int maxX = Mathf.Max(minGridPosition.x, maxGridPosition.x);
        int minZ = Mathf.Min(minGridPosition.z, maxGridPosition.z);
        int maxZ = Mathf.Max(minGridPosition.z, maxGridPosition.z);

        for (int x = minX; x <= maxX; x++)
        for (int z = minZ; z <= maxZ; z++)
            positions.Add(new GridPosition(x, z, gridPosition.floor));

        return positions;
    }

    private void RegisterOnGrid()
    {
        List<GridPosition> footprint = GetFootprintGridPositions();
        footprintOriginalWalkable.Clear();

        foreach (GridPosition gp in footprint)
        {
            footprintOriginalWalkable[gp] = Pathfinding.Instance.IsWalkableGridPosition(gp);

            LevelGrid.Instance.AddUnitAtGridPosition(gp, this);
            Pathfinding.Instance.SetIsWalkableGridPosition(gp, false);
        }

        occupiedGridPositions = footprint;
    }

    private void UnregisterFromGrid()
    {
        foreach (GridPosition gp in occupiedGridPositions)
        {
            LevelGrid.Instance.RemoveUnitAtGridPosition(gp, this);

            bool restoreWalkable =
                !footprintOriginalWalkable.TryGetValue(gp, out bool wasWalkable) || wasWalkable;
            Pathfinding.Instance.SetIsWalkableGridPosition(gp, restoreWalkable);
        }

        occupiedGridPositions.Clear();
        footprintOriginalWalkable.Clear();
    }

    public bool HasCorruptionImmune()
    {
        return hasCorruptionImmune;
    }

    public void SetCorruptionImmune(bool value)
    {
        hasCorruptionImmune = value;
    }

    public bool HasMeleeAction()
    {
        return hasMeleeAction;
    }

    public void SetMeleeAction(bool value)
    {
        hasMeleeAction = value;
    }

    //Set Here
    public void SetHP(int hp)
    {
        customHP = hp;
    }

    public void SetAP(int ap)
    {
        customAP = ap;
    }

    public void SetRegenHealth(bool value)
    {
        hasRegenHP = value;
    }

    public void SetCorruptionResist(bool value)
    {
        hasCorruptionResist = value;
    }

    //Set to Components
    public void InitHP(int hp)
    {
        healthSystem.InitHP(hp);
    }

    public void InitAP(int ap)
    {
        actionPoints = ap;
    }

    public void SetRegenHealthToSys(bool value)
    {
        healthSystem.InitRegenHealth(value);
    }

    public void SetCorruptionResistToSys(bool value)
    {
        healthSystem.InitCorruptionResist(value);
    }

    public bool HasRegenHealth()
    {
        return hasRegenHP;
    }

    public bool HasCorruptionResist()
    {
        return hasCorruptionResist;
    }

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        unitWorldUI = transform.Find("UnitWorldUI").GetComponent<UnitWorldUI>();
    }

    public void SetValues()
    {
        // hp
        if (customHP != 0)
            InitHP(customHP);

        //Ap
        if (customAP != 0)
            InitAP(customAP);

        unitWorldUI.UpdateHealthBar();
        unitWorldUI.UpdateActionPointsText();
        //bools
        SetRegenHealthToSys(hasRegenHP);
        SetCorruptionResistToSys(hasCorruptionResist);

        if (HasRegenHealth())
        {
            //add component here
            gameObject.AddComponent<HealAction>();
        }

        if (HasMeleeAction())
        {
            gameObject.AddComponent<SwordAction>();
            GetComponent<SwordAction>().SetMinDmg(5);
            GetComponent<SwordAction>().SetMaxDmg(8);
        }
    }

    private void Start()
    {
        //Debug.Log($"Start called on: {gameObject.name} with ID: {GetInstanceID()}", this);
        baseActionArray = GetComponents<BaseAction>();

        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        RegisterOnGrid();
        //LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);

        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        healthSystem.OnDead += HealthSystem_OnDead;

        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);

        SetValues();
    }

    void OnDisable()
    {
        TurnSystem.Instance.OnTurnChanged -= TurnSystem_OnTurnChanged;
        healthSystem.OnDead -= HealthSystem_OnDead;
        UnregisterFromGrid();
    }

    private void Update()
    {
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if (newGridPosition != gridPosition)
        {
            // Unit changed Grid Position
            GridPosition oldGridPosition = gridPosition;
            gridPosition = newGridPosition;

            UnregisterFromGrid();
            RegisterOnGrid();

            LevelGrid.Instance.UnitMovedGridPosition(this, oldGridPosition, newGridPosition);
        }
    }

    public T GetAction<T>()
        where T : BaseAction
    {
        foreach (BaseAction baseAction in baseActionArray)
        {
            if (baseAction is T t)
            {
                return t;
            }
        }
        return null;
    }

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }

    public BaseAction[] GetBaseActionArray()
    {
        return baseActionArray;
    }

    public bool TrySpendActionPointsToTakeAction(BaseAction baseAction)
    {
        if (CanSpendActionPointsToTakeAction(baseAction))
        {
            SpendActionPoints(baseAction.GetActionPointsCost());
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CanSpendActionPointsToTakeAction(BaseAction baseAction)
    {
        if (actionPoints >= baseAction.GetActionPointsCost())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void SpendActionPoints(int amount)
    {
        actionPoints -= amount;

        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetActionPoints()
    {
        return actionPoints;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if (
            (IsEnemy() && !TurnSystem.Instance.IsPlayerTurn())
            || (!IsEnemy() && TurnSystem.Instance.IsPlayerTurn())
        )
        {
            if (customAP != 0)
                InitAP(customAP);

            // if (customAP == 0)
            //     actionPoints = ACTION_POINTS_MAX;
            // else
            //     InitAP(customAP);
            // if(setCustomAP == 0) actionPoints = ACTION_POINTS_MAX;
            // else actionPoints = setCustomAP;

            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool IsEnemy()
    {
        return isEnemy;
    }

    public void Damage(int damageAmount)
    {
        healthSystem.Damage(damageAmount);
    }

    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        Debug.Log($"{name} is Dead");
        //LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);
        UnregisterFromGrid();

        //Destroy Friendly Units
        if (GetComponent<KaijuUnit>() == null)
        {
            Destroy(gameObject);
        }
        else
        {
            //Do nothing or something here for the Kaiju
            GetComponent<KaijuUnit>().TurnUIFalse();
            GetComponent<KaijuUnit>().TurnMeshToDef();
            GetComponent<KaijuUnit>().InitAnimateHide();
        }
        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
    }

    public float GetHealthNormalized()
    {
        return healthSystem.GetHealthNormalized();
    }
}
