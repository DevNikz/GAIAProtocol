using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveSummaryEntryUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI label;

    [SerializeField]
    Image checkIcon;

    [SerializeField]
    Sprite completeSprite;

    [SerializeField]
    Sprite incompleteSprite;

    [SerializeField]
    GameObject sideTag; // small "SIDE" badge, active only for side objectives

    public void Setup(string displayName, bool isComplete, ObjectiveType type)
    {
        Debug.Log($"Name: {displayName} | Complete: {isComplete} | Type: {type}");
        label.text = displayName;
        checkIcon.sprite = isComplete ? completeSprite : incompleteSprite;
        if (sideTag != null)
            sideTag.SetActive(type == ObjectiveType.Side);
    }
}
