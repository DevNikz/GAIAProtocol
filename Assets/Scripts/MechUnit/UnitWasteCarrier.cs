using UnityEngine;

public class UnitWasteCarrier : MonoBehaviour
{
    [SerializeField]
    public bool IsCarryingWaste;

    public bool TryPickUp()
    {
        if (IsCarryingWaste)
            return false;
        IsCarryingWaste = true;
        return true;
    }

    public bool TryDrop()
    {
        if (!IsCarryingWaste)
            return false;
        IsCarryingWaste = false;
        return true;
    }
}
