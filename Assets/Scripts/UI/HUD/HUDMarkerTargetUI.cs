using UnityEngine;
using UnityEngine.UI;

public class HUDMarkerTargetUI : MonoBehaviour
{
    [SerializeField] private Image LinkedImage;

    private HUDMarkerInWorldTarget LinkedTarget;
    private RectTransform HUDMarkerRect;

    public void Bind(HUDMarkerInWorldTarget _Target, Sprite _Image)
    {
        LinkedTarget = _Target;
        LinkedImage.sprite = _Image;
    }

    void Start()
    {
        HUDMarkerRect = (RectTransform) transform.parent;
    }

    void Update()
    {
        
    }
}