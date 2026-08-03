using UnityEngine;

[CreateAssetMenu(fileName = "Worker", menuName = "CUSTOM/WORKER UNIT", order = 1)]
public class WorkerScriptableObject : ScriptableObject, IUnitStats
{
    public int hp = 100; //default = 100
    public int actionPoints = 4; //default = 4
    public int moveRange = 4; //default = 4
    public float interactEfficiency = 0.25f; //default = 0.25f
    public bool hasMeleeAttack;
    public bool hasRegenHealth;

    public string GetStatDisplay()
    {
        string stats =
            $"HP: {hp}\n"
            + $"Action Points: {actionPoints}\n"
            + $"Move Range: {moveRange}\n"
            + $"Interact Efficiency: {interactEfficiency:P0}";

        if (hasMeleeAttack)
            stats += "\nMelee Attack";
        if (hasRegenHealth)
            stats += "\nHealth Regen";

        stats += "\n";
        stats += "\nThe Standard Unarmed Unit of the GAIA PROTOCOL.";
        stats += "\n";
        stats +=
            "\nCapable of <color=#b3fbff>interaction</color> with points of interests and objectives.";
        stats += "\n";
        stats +=
            "\nThis is <color=red>incapable</color> of dealing damage against <color=red>Kaijus</color>. Be warned as they are quite vulnerable.";

        return stats;
    }
}
