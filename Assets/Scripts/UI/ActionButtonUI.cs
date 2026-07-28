using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionButtonUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI actionName;

    [SerializeField]
    private TextMeshProUGUI actionCounter;

    [SerializeField]
    private Image actionIcon;

    [SerializeField]
    private Button button;

    [SerializeField]
    private GameObject selectedGameObject;

    private BaseAction baseAction;

    public void SetBaseAction(BaseAction baseAction)
    {
        this.baseAction = baseAction;
        actionName.text = baseAction.GetActionName().ToUpper();
        actionCounter.text = baseAction.GetActionPointsCost().ToString();

        switch (baseAction.GetActionName())
        {
            case "Move":
                actionIcon.sprite = UnitActionSystem.Instance.actionIconList[0];
                break;
            case "Interact":
                actionIcon.sprite = UnitActionSystem.Instance.actionIconList[1];

                break;
            case "Sword":
                actionIcon.sprite = UnitActionSystem.Instance.actionIconList[2];
                break;
            case "Shoot":
                actionIcon.sprite = UnitActionSystem.Instance.actionIconList[3];
                break;
            case "Repair":
                actionIcon.sprite = UnitActionSystem.Instance.actionIconList[4];
                break;
            case "Heal":
                actionIcon.sprite = UnitActionSystem.Instance.actionIconList[5];
                break;
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            UnitActionSystem.Instance.SetSelectedAction(baseAction);
            SoundManager.Instance.PlaySFX("Select");
        });
    }

    public void ButtonClick()
    {
        button.onClick.Invoke();
    }

    public void UpdateSelectedVisual()
    {
        BaseAction selectedBaseAction = UnitActionSystem.Instance.GetSelectedAction();
        selectedGameObject.SetActive(selectedBaseAction == baseAction);
        actionName.gameObject.SetActive(selectedBaseAction == baseAction);
    }

    public void ChangeActionPointText()
    {
        //BaseAction selectedBaseAction = UnitActionSystem.Instance.GetSelectedAction();
        actionCounter.text = UnitActionSystem.Instance.GetSelectedUnit().actionPoints.ToString();
    }

    public string GetActionName()
    {
        return baseAction.GetActionName();
    }
}
