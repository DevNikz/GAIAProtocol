using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitActionSystemUI : MonoBehaviour {

    private GameObject busyVisualGameObject;
    private GameObject enemyTurnVisualGameObject;
    private Dictionary<ActionType, Transform> actionButtonDic;
    private TextMeshProUGUI actionPointsText;

    private IUnitAction moveAction;
    private IUnitAction spinAction;
    private IUnitAction shootAction;
    private IUnitAction grenadeAction;
    private IUnitAction overwatchAction;

    private void Awake() {
        busyVisualGameObject = transform.Find("BusyVisual").gameObject;
        enemyTurnVisualGameObject = transform.Find("EnemyTurnVisual").gameObject;
        actionPointsText = transform.Find("ActionPointsText").GetComponent<TextMeshProUGUI>();

        actionButtonDic = new Dictionary<ActionType, Transform> {
            {  ActionType.Move, transform.Find("MoveActionBtn") },
            {  ActionType.Spin, transform.Find("SpinActionBtn") },
            {  ActionType.Shoot, transform.Find("ShootActionBtn") },
            {  ActionType.Grenade, transform.Find("GrenadeActionBtn") },
            {  ActionType.Overwatch, transform.Find("OverwatchActionBtn") },
        };

        transform.Find("MoveActionBtn").GetComponent<Button>().onClick.AddListener(() => {
            UnitActionSystem.Instance.SetSelectedAction(moveAction);
        });
        transform.Find("SpinActionBtn").GetComponent<Button>().onClick.AddListener(() => {
            UnitActionSystem.Instance.SetSelectedAction(spinAction);
        });
        transform.Find("ShootActionBtn").GetComponent<Button>().onClick.AddListener(() => {
            UnitActionSystem.Instance.SetSelectedAction(shootAction);
        });
        transform.Find("GrenadeActionBtn").GetComponent<Button>().onClick.AddListener(() => {
            UnitActionSystem.Instance.SetSelectedAction(grenadeAction);
        });
        transform.Find("OverwatchActionBtn").GetComponent<Button>().onClick.AddListener(() => {
            UnitActionSystem.Instance.SetSelectedAction(overwatchAction);
        });

    }

    private void Start() {
        UnitActionSystem.Instance.OnSelectedChanged += UnitActionSystem_OnSelectedChanged;
        UnitActionSystem.Instance.OnBusyChanged += UnitActionSystem_OnBusyChanged;
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;

        UpdateBusyVisual();
        UpdateSelectedAction();
        UpdateActionPointsText();
        UpdateEnemyTurnVisual();
    }

    private void TurnSystem_OnTurnChanged(object sender, System.EventArgs e) {
        UpdateEnemyTurnVisual();
    }

    private void UnitActionSystem_OnBusyChanged(object sender, bool isBusy) {
        UpdateBusyVisual();
        UpdateActionPointsText();
    }

    private void UpdateBusyVisual() {
        busyVisualGameObject.SetActive(UnitActionSystem.Instance.IsBusy());
    }

    private void UnitActionSystem_OnSelectedChanged(object sender, UnitActionSystem.OnSelectedChangedEventArgs e) {
        UpdateSelectedAction();
        IUnitAction[] unitActionArray = e.unit.GetUnitActionArray();
    }

    private void UpdateSelectedAction() {
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        IUnitAction selectedUnitAction = UnitActionSystem.Instance.GetSelectedUnitAction();

        moveAction = selectedUnit.GetAction<MoveAction>();
        spinAction = selectedUnit.GetAction<SpinAction>();
        shootAction = selectedUnit.GetAction<ShootAction>();
        grenadeAction = selectedUnit.GetAction<GrenadeAction>();
        overwatchAction = selectedUnit.GetAction<OverwatchAction>();

        // Deselect all
        foreach (ActionType actionType in actionButtonDic.Keys) {
            actionButtonDic[actionType].Find("Selected").gameObject.SetActive(false);
        }

        if (selectedUnit != null) {
            actionButtonDic[selectedUnitAction.GetActionType()].Find("Selected").gameObject.SetActive(true);
        }

        UpdateActionPointsText();
    }

    private void UpdateActionPointsText() {
        actionPointsText.text = "Action Points: " + UnitActionSystem.Instance.GetSelectedUnit().GetActionPoints().ToString();
    }

    private void UpdateEnemyTurnVisual() {
        enemyTurnVisualGameObject.SetActive(!TurnSystem.Instance.IsPlayerTurn());
    }

}