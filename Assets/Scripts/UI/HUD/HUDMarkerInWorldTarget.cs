using UnityEngine;

public class HUDMarkerInWorldTarget : MonoBehaviour
{
    [SerializeField]
    private Sprite HUDSprite;

    [SerializeField]
    private ObjectiveBase obj;

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
                obj.GetObjectiveIndex()
            );
        }
        else
        {
            HUDMarkerManager.Instance.AddMarker(this, HUDSprite, isObjective);
        }
        //HUDMarkerManager.Instance.AddMarker(this, HUDSprite, isObjective, worldUI.GetObjectiveIndex());
    }
}
