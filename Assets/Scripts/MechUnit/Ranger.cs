using UnityEngine;

[CreateAssetMenu(fileName = "Ranger", menuName = "CUSTOM/RANGER UNIT", order = 2)]
public class RangerScriptableObject : ScriptableObject, IUnitStats
{
    public int hp = 100; //default = 100
    public int actionPoints = 4; //default = 4
    public int moveRange = 4; //default = 4
    public int attackRange = 7; //default = 0.25f
    public int minAttackDamage = 1;
    public int maxAttackDamage = 5;
    public bool hasPlasmaRifle;
    public bool hasCorruptionResist;

    public string GetStatDisplay()
    {
        string stats =
            $"HP: {hp}\n"
            + $"Action Points: {actionPoints}\n"
            + $"Move Range: {moveRange}\n"
            + $"Attack Range: {attackRange}\n"
            + $"Damage: {minAttackDamage}-{maxAttackDamage}";

        if (hasPlasmaRifle)
            stats += "\nPlasma Rifle";
        if (hasCorruptionResist)
            stats += "\nCorruption Resist";

        stats += "\n";
        stats += "\nThe Standard Armed Unit of the GAIA PROTOCOL.";
        stats += "\n";
        stats += "\nCapable of <color=#b3fbff>Dealing Damage</color> against corrupted lifeforms.";
        stats += "\n";
        stats +=
            "\nHowever, this is <color=red>incapable</color> of <color=red>slaying Kaijus</color>, but they can <color=yellow>stagger</color> them when dealt enough damage.";

        return stats;
    }
}
