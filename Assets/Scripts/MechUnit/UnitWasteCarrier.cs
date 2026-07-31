using UnityEngine;

public class UnitWasteCarrier : MonoBehaviour
{
    [SerializeField]
    public bool IsCarryingWaste;

    [SerializeField]
    GameObject wastePile;

    public bool TryPickUp()
    {
        if (IsCarryingWaste)
            return false;
        IsCarryingWaste = true;
        wastePile.SetActive(true);
        return true;
    }

    public bool TryDrop()
    {
        if (!IsCarryingWaste)
            return false;
        IsCarryingWaste = false;
        wastePile.SetActive(false);
        return true;
    }
}
