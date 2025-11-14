using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionButtonUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI actionName;
    [SerializeField] private TextMeshProUGUI actionCounter;
    [SerializeField] private Image actionIcon;
    [SerializeField] private Button button;
    [SerializeField] private GameObject selectedGameObject;


    private BaseAction baseAction;

    public void SetBaseAction(BaseAction baseAction)
    {
        this.baseAction = baseAction;
        actionName.text = baseAction.GetActionName().ToUpper();
        actionCounter.text =  baseAction.GetActionPointsCost().ToString();

        switch(baseAction.GetActionName())
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
        }
        
        button.onClick.AddListener(() => {
            UnitActionSystem.Instance.SetSelectedAction(baseAction);
        });
    }

    public void UpdateSelectedVisual()
    {
        BaseAction selectedBaseAction = UnitActionSystem.Instance.GetSelectedAction();
        selectedGameObject.SetActive(selectedBaseAction == baseAction);
        actionName.gameObject.SetActive(selectedBaseAction == baseAction);
    }
}
