using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UnitActionSystemUI : MonoBehaviour
{
    [SerializeField]
    private Transform actionButtonPrefab;

    [SerializeField]
    private Transform actionButtonContainerTransform;

    [SerializeField]
    private TextMeshProUGUI actionPointsText;

    [SerializeField]
    private int numChild;
    private List<ActionButtonUI> actionButtonUIList;

    private void Awake()
    {
        actionButtonUIList = new List<ActionButtonUI>();
        numChild = actionButtonContainerTransform.childCount;
        SetupUAS_UI();
    }

    void SetupUAS_UI()
    {
        //Debug.Log($"{transform.GetChild(0).name}");
        //actionButtonContainerTransform = transform.GetChild(1);

        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        UnitActionSystem.Instance.OnSelectedActionChanged +=
            UnitActionSystem_OnSelectedActionChanged;
        UnitActionSystem.Instance.OnActionStarted += UnitActionSystem_OnActionStarted;
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;

        //UpdateActionPoints();
        CreateUnitActionButtons();
        UpdateSelectedVisual();
    }

    void OnDisable()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged -= UnitActionSystem_OnSelectedUnitChanged;
        UnitActionSystem.Instance.OnSelectedActionChanged -=
            UnitActionSystem_OnSelectedActionChanged;
        UnitActionSystem.Instance.OnActionStarted -= UnitActionSystem_OnActionStarted;
        TurnSystem.Instance.OnTurnChanged -= TurnSystem_OnTurnChanged;
        Unit.OnAnyActionPointsChanged -= Unit_OnAnyActionPointsChanged;
    }

    private void CreateUnitActionButtons()
    {
        //Debug.Log($"Is Null? {actionButtonContainerTransform == null}");
        //Debug.Log($"has children? {actionButtonContainerTransform.childCount}");

        if (numChild > 0)
        {
            foreach (Transform buttonTransform in actionButtonContainerTransform)
            {
                Destroy(buttonTransform.gameObject);
            }
        }
        actionButtonUIList.Clear();

        if (UnitActionSystem.Instance.GetSelectedUnit() != null)
        {
            Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
            //Debug.Log(selectedUnit.name);

            foreach (BaseAction baseAction in selectedUnit.GetBaseActionArray())
            {
                Transform actionButtonTransform = Instantiate(
                    actionButtonPrefab,
                    actionButtonContainerTransform
                );
                ActionButtonUI actionButtonUI =
                    actionButtonTransform.GetComponent<ActionButtonUI>();
                actionButtonUI.SetBaseAction(baseAction);

                actionButtonUIList.Add(actionButtonUI);
            }
        }
    }

    private static readonly KeyCode[] numberKeys = new KeyCode[]
    {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6,
        KeyCode.Alpha7,
        KeyCode.Alpha8,
        KeyCode.Alpha9,
    };

    void Update()
    {
        if (UnitActionSystem.Instance.GetSelectedUnit() == null)
        {
            return;
        }

        for (int i = 0; i < actionButtonUIList.Count && i < numberKeys.Length; i++)
        {
            if (Input.GetKeyDown(numberKeys[i]))
            {
                actionButtonUIList[i].ButtonClick();
                break; // only one key press per frame matters
            }
        }
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e)
    {
        numChild = actionButtonContainerTransform.childCount;
        if (UnitActionSystem.Instance.GetSelectedUnit() != null)
        {
            CreateUnitActionButtons();
            UpdateSelectedVisual();
            //UpdateActionPoints();
        }
        else
        {
            DestroyUnitActionButtons();
            UpdateSelectedVisual();
            //UpdateActionPoints();
        }
    }

    private void DestroyUnitActionButtons()
    {
        foreach (Transform buttonTransform in actionButtonContainerTransform)
        {
            Destroy(buttonTransform.gameObject);
        }
        actionButtonUIList.Clear();
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateSelectedVisual();
    }

    private void UnitActionSystem_OnActionStarted(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }

    private void UpdateSelectedVisual()
    {
        foreach (ActionButtonUI actionButtonUI in actionButtonUIList)
        {
            actionButtonUI.UpdateSelectedVisual();
        }
    }

    private void UpdateActionPoints()
    {
        if (UnitActionSystem.Instance.GetSelectedUnit() != null)
        {
            if (UnitActionSystem.Instance.GetSelectedUnit().actionPoints != 0)
            {
                foreach (ActionButtonUI actionButtonUI in actionButtonUIList)
                {
                    // //INTERACT
                    // if (actionButtonUI.GetActionName() == "Interact")
                    // {
                    //     actionButtonUI.ChangeActionPointText();
                    // }
                }
            }
            else
            {
                DestroyUnitActionButtons();
                UpdateSelectedVisual();
            }
        }
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        //UpdateActionPoints();
        numChild = actionButtonContainerTransform.childCount;
        if (UnitActionSystem.Instance.GetSelectedUnit() != null)
        {
            CreateUnitActionButtons();
            UpdateSelectedVisual();
            //UpdateActionPoints();
        }
        else
        {
            DestroyUnitActionButtons();
            UpdateSelectedVisual();
            //UpdateActionPoints();
        }
    }

    private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
    {
        //UpdateActionPoints();
    }
}
