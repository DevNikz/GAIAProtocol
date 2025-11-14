using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
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
    private Camera cam;
    [SerializeField] private GridPosition selectedGrid, nullGrid;
    private GridObject grid;

    public BaseAction selectedAction;
    private bool isBusy;

    //UI Stuffs
    [SerializeField] public List<Sprite> actionIconList; // 0 - Move | 1 - Interact


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        cam = Camera.main;

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

        
        //Debug.DrawRay(cam.transform.position, mousePos - cam.transform.position, Color.blue);

        HandleSelectedAction();
    }

    private void HandleSelectedAction()
    {
        if (selectedUnit != null)
        {
            if(!IsPointerOverUIObject()) HoverGrid();
            //Click
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

    void HoverGrid()
    {
        GridPosition gridPos = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPositionOnlyHitVisible());

        if (!selectedAction.IsValidActionGridPosition(gridPos))
        {
            return;
        }
        else {
            if(gridPos.isSelect != true)
            {
                //Debug.Log($"Grid: {gridPos}");

                if(selectedGrid != nullGrid) {
                    if(selectedGrid == gridPos) return;
                    //Debug.Log("Not Null Grid");
                    selectedGrid.isSelect = false;
                    GridSystemVisual.Instance.DeselectGridMaterial(selectedGrid);
                    selectedGrid = nullGrid;
                    return;
                }

                selectedGrid = gridPos;
                //Debug.Log($"Selected Grid: {selectedGrid}");
                
                selectedGrid.isSelect = true;
                GridSystemVisual.Instance.HoverGridMaterial(selectedGrid);
            }
        }
    }

    private bool IsPointerOverUIObject() {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
    
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
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