using TMPro;
using UnityEngine;

// Drop this on the card root (e.g. the object holding your "Header" with
// the two TMP Text children) and drag the two Text (TMP) objects into
// the fields below in the Inspector.
public class UnitCardUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI titleText; // Header/Text (TMP)
    public TextMeshProUGUI descriptionText; // Header/Text (TMP) (1)

    public void Display(string title, IUnitStats stats)
    {
        if (titleText != null)
            titleText.text = title;
        if (descriptionText != null)
            descriptionText.text = stats.GetStatDisplay();
    }
}
