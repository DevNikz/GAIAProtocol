using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjUI : MonoBehaviour
{
    [SerializeField] private Toggle toggle;
    [SerializeField] private TextMeshProUGUI desc;

    public void SetUI(string value)
    {
        toggle.isOn = false;
        desc.text = value;
    }

    public void SetBool(bool value)
    {
        toggle.isOn = value;
    }
}