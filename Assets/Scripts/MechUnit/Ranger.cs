using UnityEngine;

[CreateAssetMenu(fileName = "Ranger", menuName = "CUSTOM/RANGER UNIT", order = 2)]
public class RangerScriptableObject : ScriptableObject
{
    public int hp = 100; //default = 100
    public int actionPoints = 4; //default = 4
    public int moveRange = 4; //default = 4
    public int attackRange = 7; //default = 0.25f
    public int minAttackDamage = 1;
    public int maxAttackDamage = 5;
    public bool hasPlasmaRifle;
    public bool hasCorruptionResist;
}