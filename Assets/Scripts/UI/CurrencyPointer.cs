using UnityEngine;
using UnityEngine.EventSystems;

public class CurrencyPointer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    GameObject highlight;
    void Awake()
    {
        highlight = transform.parent.transform.Find("Highlight").gameObject;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        highlight.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        highlight.SetActive(false);
    }
}
