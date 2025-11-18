using UnityEngine;

public class HUDMarkerInWorldTarget : MonoBehaviour
{
    [SerializeField] Sprite HUDSprite;

    void Start()
    {
        HUDMarkerManager.Instance.AddMarker(this, HUDSprite);
    }
}
