using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UnitActionSystem : MonoBehaviour
{

    public static UnitActionSystem Instance { get; private set; }


    public event EventHandler OnSelectedUnitChanged;
    public event EventHandler OnSelectedActionChanged;
    public event EventHandler<bool>  OnBusyChanged;
    public event EventHandler OnActionStarted;
    public event EventHandler OnDeselectedUnitChanged;

    [SerializeField] private Unit selectedUnit;
    [SerializeField] private LayerMask unitLayerMask;

    private BaseAction selectedAction;
    private bool isBusy;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetSelectedUnit(selectedUnit);
    }
    
    private void Update()
    {
        if (isBusy)
        {
            return;
        }

        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            return;
        }

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (TryHandleUnitSelection())
        {
            return;
        }

        HandleSelectedAction();
    }

    private void HandleSelectedAction()
    {
        if (selectedUnit != null)
        {
            if (InputManager.Instance.IsMouseButtonDownThisFrame())
            {
                GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPositionOnlyHitVisible());

                if (!selectedAction.IsValidActionGridPosition(mouseGridPosition))
                {
                    return;
                }

                if (!selectedUnit.TrySpendActionPointsToTakeAction(selectedAction))
                {
                    return;
                }

                SetBusy();
                selectedAction.TakeAction(mouseGridPosition, ClearBusy);

                OnActionStarted?.Invoke(this, EventArgs.Empty);
            }
        }
    }


    private void SetBusy()
    {
        isBusy = true;

        OnBusyChanged?.Invoke(this, isBusy);
    }

    private void ClearBusy()
    {
        isBusy = false;

        OnBusyChanged?.Invoke(this, isBusy);
    }

    private bool TryHandleUnitSelection()
    {
        if (InputManager.Instance.IsMouseButtonDownThisFrame())
        {
            Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unitLayerMask))
            {
                if (raycastHit.transform.TryGetComponent<Unit>(out Unit unit))
                {
                    //Debug.Log(unit.name);
                    if (unit == selectedUnit)
                    {
                        // Unit is already selected

                        //Deselect
                        DeselectUnit();
                        return false;
                    }

                    if (unit.IsEnemy())
                    {
                        // Clicked on an Enemy
                        return false;
                    }

                    SetSelectedUnit(unit);
                    return true;
                }
            }
        }

        return false;
    }

    private void SetSelectedUnit(Unit unit)
    {
        if (unit != null)
        {
            selectedUnit = unit;
            SetSelectedAction(unit.GetAction<MoveAction>());
            OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void DeselectUnit()
    {
        if (selectedUnit != null)
        {
            selectedUnit = null;
            OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
            OnDeselectedUnitChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public void SetSelectedAction(BaseAction baseAction)
    {
        //Debug.Log(baseAction);
        selectedAction = baseAction;

        OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
    }

    public Unit GetSelectedUnit()
    {
        return selectedUnit;
    }

    public BaseAction GetSelectedAction()
    {
        return selectedAction;
    }

}