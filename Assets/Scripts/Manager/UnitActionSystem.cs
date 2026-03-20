using System;
using System.Collections.Generic;
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
    [Header("UI")]
    [SerializeField] public List<Sprite> actionIconList; // 0 - Move | 1 - Interact
    [SerializeField] public bool isHovering;

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

    private void Start()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    void OnDisable()
    {
        TurnSystem.Instance.OnTurnChanged -= TurnSystem_OnTurnChanged;
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
            //Hovering
            if(TryHovering()) HoverGrid();

            //Click
            if(InputManager.Instance.IsMouseButtonDownThisFrame())
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

                SoundManager.Instance.PlaySFX("Select");
                SetBusy();
                selectedAction.TakeAction(mouseGridPosition, ClearBusy);

                OnActionStarted?.Invoke(this, EventArgs.Empty);

                selectedGrid.isSelect = false;
                GridSystemVisual.Instance.DeselectGridMaterial(selectedGrid);
                selectedGrid = nullGrid;

                if(selectedUnit.actionPoints == 0) {
                    DeselectUnit();
                    SetSelectedAction(null);
                    isHovering = false;
                    selectedGrid.isSelect = false;
                    selectedGrid = nullGrid;
                }
            }
        }
    }

    bool TryHovering()
    {
        if(IsPointerOverUIObject() || selectedAction == null || 
            selectedAction.GetActionName() == "Interact" || selectedAction.GetActionName() == "Shoot" ||
            selectedAction.GetActionName() == "Sword")
        {
            isHovering = false;
            return false;
        }
        else
        {
            isHovering = true;
            return true;
        }
    }

    void HoverGrid()
    {
        GridPosition gridPos = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPositionOnlyHitVisible());

        if (!selectedAction.IsValidActionGridPosition(gridPos))
        {
            if(selectedGrid != nullGrid) {
                selectedGrid.isSelect = false;
                GridSystemVisual.Instance.DeselectGridMaterial(selectedGrid);
                selectedGrid = nullGrid;
            }
            return;
        }
        else {
            //Debug.Log($"Hovering over Grid: {gridPos}");
            if(gridPos.isSelect != true)
            {
                if(selectedGrid != nullGrid) {
                    if(selectedGrid != gridPos) {
                        //Debug.Log($"Previously Selected Grid: {selectedGrid}");
                        selectedGrid.isSelect = false;
                        GridSystemVisual.Instance.DeselectGridMaterial(selectedGrid);
                        selectedGrid = nullGrid;
                        //return;
                    }
                    else
                    {
                        //Debug.Log($"Currently Selected Grid: {selectedGrid}");
                        GridSystemVisual.Instance.HoverGridMaterial(selectedGrid);
                    }
                }

                selectedGrid = gridPos;
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
                        SoundManager.Instance.PlaySFX("Deselect");
                        //soundController.PlaySound(3);
                        DeselectUnit();
                        SetSelectedAction(null);
                        isHovering = false;
                        selectedGrid.isSelect = false;
                        selectedGrid = nullGrid;
                        return false;
                    }

                    if (unit.IsEnemy())
                    {
                        // Clicked on an Enemy
                        return false;
                    }

                    if (unit.actionPoints == 0)
                    {
                        SoundManager.Instance.PlaySFX("Deselect");
                        //soundController.PlaySound(3);
                        DeselectUnit();
                        SetSelectedAction(null);
                        isHovering = false;
                        selectedGrid.isSelect = false;
                        selectedGrid = nullGrid;
                        return false;
                    }

                    SoundManager.Instance.PlaySFX("Select");
                    //soundController.PlaySound(2);
                    SetSelectedUnit(unit);
                    return true;
                }
            }
        }

        return false;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            DeselectUnit();
            SetSelectedAction(null);
            selectedGrid.isSelect = false;
            GridSystemVisual.Instance.DeselectGridMaterial(selectedGrid);
            selectedGrid = nullGrid;
        }
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