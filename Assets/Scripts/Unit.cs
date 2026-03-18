using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Unit : MonoBehaviour
{
    private const int ACTION_POINTS_MAX = 4;


    public static event EventHandler OnAnyActionPointsChanged;
    public static event EventHandler OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDead;
    [SerializeField] private bool isEnemy;


    private GridPosition gridPosition;
    private HealthSystem healthSystem;
    private BaseAction[] baseActionArray;
    public int actionPoints = ACTION_POINTS_MAX;

    int customHP;
    [SerializeField] int customAP;
    bool hasRegenHP;
    bool hasCorruptionResist;

    //Set Here
    public void SetHP(int hp) { customHP = hp; }
    public void SetAP(int ap) { customAP = ap; }
    public void HasRegenHealth(bool value) { hasRegenHP = value; } 
    public void HasCorruptionResist(bool value) { hasCorruptionResist = value; }

    //Set to Components
    public void InitHP(int hp) { healthSystem.InitHP(hp); }
    public void InitAP(int ap) { actionPoints = ap; }
    public void HasRegenHealthToSys(bool value) { healthSystem.InitRegenHealth(value); } 
    public void HasCorruptionResistToSys(bool value) { healthSystem.InitCorruptionResist(value); }

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        baseActionArray = GetComponents<BaseAction>();

        //hp
        if(customHP != 0) InitHP(customHP);
        
        //Ap
        if(customAP == 0) actionPoints = ACTION_POINTS_MAX; 
        else InitAP(customAP);

        //bools
        HasRegenHealthToSys(hasRegenHP);
        HasCorruptionResistToSys(hasCorruptionResist);

        // if(setCustomAP == 0) actionPoints = ACTION_POINTS_MAX;
        // else actionPoints = setCustomAP;

        //SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);

        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        healthSystem.OnDead += HealthSystem_OnDead;

        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);
    }

    void OnDisable()
    {
        TurnSystem.Instance.OnTurnChanged -= TurnSystem_OnTurnChanged;
        healthSystem.OnDead -= HealthSystem_OnDead;
    }

    private void Update()
    {
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if (newGridPosition != gridPosition)
        {
            // Unit changed Grid Position
            GridPosition oldGridPosition = gridPosition;
            gridPosition = newGridPosition;

            LevelGrid.Instance.UnitMovedGridPosition(this, oldGridPosition, newGridPosition);
        }
    }

    public T GetAction<T>() where T : BaseAction
    {
        foreach (BaseAction baseAction in baseActionArray)
        {
            if (baseAction is T)
            {
                return (T)baseAction;
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
        } else
        {
            return false;
        }
    }

    public bool CanSpendActionPointsToTakeAction(BaseAction baseAction)
    {
        if (actionPoints >= baseAction.GetActionPointsCost())
        {
            return true;
        } else
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
        if ((IsEnemy() && !TurnSystem.Instance.IsPlayerTurn()) ||
            (!IsEnemy() && TurnSystem.Instance.IsPlayerTurn()))
        {
            if(customAP == 0) actionPoints = ACTION_POINTS_MAX; 
            else InitAP(customAP);
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
        LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);

        Destroy(gameObject);

        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
    }

    public float GetHealthNormalized()
    {
        return healthSystem.GetHealthNormalized();
    }

}