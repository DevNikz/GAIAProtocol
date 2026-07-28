using UnityEngine;

public class RangerMech : MonoBehaviour
{
    [SerializeField]
    RangerScriptableObject rangerTier;

    public void SetCurrentTier(RangerScriptableObject tier)
    {
        rangerTier = tier;
    }

    public void SetCustomValues()
    {
        GetComponent<Unit>().SetHP(rangerTier.hp);
        GetComponent<Unit>().SetAP(rangerTier.actionPoints);
        GetComponent<MoveAction>().SetMoveDist(rangerTier.moveRange);
        GetComponent<ShootAction>().SetAttackRange(rangerTier.maxAttackDamage);
        GetComponent<ShootAction>().SetMinDmg(rangerTier.minAttackDamage);
        GetComponent<ShootAction>().SetMaxDmg(rangerTier.maxAttackDamage);
        GetComponent<ShootAction>().HasPlasmaRifle(rangerTier.hasPlasmaRifle);
        GetComponent<Unit>().SetCorruptionResist(rangerTier.hasCorruptionResist);
        //GetComponent<Unit>().SetValues();
    }
}
