using UnityEngine;

public class HUDMarkerInWorldTarget : MonoBehaviour
{
    [SerializeField]
    private Sprite HUDSprite;

    [SerializeField]
    private ObjectiveBase obj;

    [SerializeField]
    private Color color = default;

    [SerializeField]
    private Vector3 scale = Vector3.one;

    void Start()
    {
        HUDMarkerManager.Instance.AddMarker(this, HUDSprite, obj, scale, color);
    }
}
