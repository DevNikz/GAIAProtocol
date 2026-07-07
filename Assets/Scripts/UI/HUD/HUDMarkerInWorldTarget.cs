using UnityEngine;

public class HUDMarkerInWorldTarget : MonoBehaviour
{
    [SerializeField]
    private Sprite HUDSprite;

    [SerializeField]
    private ObjectiveWorldUI worldUI;

    [SerializeField]
    private bool isObjective;

    void Start()
    {
        if (isObjective)
        {
            HUDMarkerManager.Instance.AddMarker(
                this,
                HUDSprite,
                isObjective,
                worldUI.GetObjectiveIndex()
            );
        }
        else
        {
            HUDMarkerManager.Instance.AddMarker(this, HUDSprite, isObjective);
        }
        //HUDMarkerManager.Instance.AddMarker(this, HUDSprite, isObjective, worldUI.GetObjectiveIndex());
    }
}
