using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitWorldUI : MonoBehaviour {

    [SerializeField] private Sprite coverHalf;
    [SerializeField] private Sprite coverFull;


    private Unit unit;
    private TextMeshProUGUI actionPointsText;
    private TextMeshProUGUI hitPercentText;
    private Image healthBarImage;
    private Image coverImage;

    private void Awake() {
        actionPointsText = transform.Find("ActionPointsText").GetComponent<TextMeshProUGUI>();
        hitPercentText = transform.Find("HitPercentText").GetComponent<TextMeshProUGUI>();
        healthBarImage = transform.Find("HealthBar").Find("Image").GetComponent<Image>();
        coverImage = transform.Find("CoverImage").GetComponent<Image>();

        HideHitPercent();
    }

    private void Start() {
        unit = transform.parent.GetComponent<Unit>();

        unit.OnActionPointsChanged += Unit_OnActionPointsChanged;
        unit.OnCoverTypeChanged += Unit_OnCoverTypeChanged;
        unit.GetHealthSystem().OnHealthChanged += Unit_OnHealthChanged;
        unit.GetHealthSystem().OnDead += Unit_OnDead;

        UnitActionSystem.Instance.OnSelectedChanged += UnitActionSystem_OnSelectedChanged;

        UpdateActionPointsText();
        UpdateHealthBar();
        UpdateCoverImage();
    }

    private void Unit_OnDead(object sender, System.EventArgs e) {
        UnitActionSystem.Instance.OnSelectedChanged -= UnitActionSystem_OnSelectedChanged;
    }

    private void UnitActionSystem_OnSelectedChanged(object sender, UnitActionSystem.OnSelectedChangedEventArgs e) {
        // See if action needs to show Hit Percent
        HideHitPercent();

        switch (e.unitAction.GetActionType()) {
            case ActionType.Shoot:
                // Shoot Action active
                if (unit.IsEnemy() != e.unit.IsEnemy()) {
                    // This unit and the selected are enemies
                    ShootAction shootAction = e.unit.GetAction<ShootAction>();
                    if (shootAction.IsValidShootPosition(unit.GetGridPosition())) {
                        // This unit us on a valid shoot position
                        ShowHitPercent(shootAction.GetHitPercent(unit));
                    }
                }
                break;
        }
    }

    private void Unit_OnCoverTypeChanged(object sender, System.EventArgs e) {
        UpdateCoverImage();
    }

    private void Unit_OnHealthChanged(object sender, System.EventArgs e) {
        UpdateHealthBar();
    }

    private void Unit_OnActionPointsChanged(object sender, int e) {
        UpdateActionPointsText();
    }

    private void UpdateActionPointsText() {
        actionPointsText.text = unit.GetActionPoints().ToString();
    }

    private void UpdateHealthBar() {
        healthBarImage.fillAmount = unit.GetHealthSystem().GetHealthNormalized();
    }

    private void UpdateCoverImage() {
        switch (unit.GetCoverType()) {
            default:
            case CoverType.None:
                coverImage.enabled = false;
                break;
            case CoverType.Half:
                coverImage.enabled = true;
                coverImage.sprite = coverHalf;
                break;
            case CoverType.Full:
                coverImage.enabled = true;
                coverImage.sprite = coverFull;
                break;
        }
    }

    private void ShowHitPercent(float hitChance) {
        hitPercentText.gameObject.SetActive(true);
        hitPercentText.text = "HIT: " + Mathf.Round(hitChance * 100f) + "%";
    }

    private void HideHitPercent() {
        hitPercentText.gameObject.SetActive(false);
    }
}