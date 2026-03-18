using UnityEngine;

[CreateAssetMenu(fileName = "Worker", menuName = "CUSTOM/WORKER UNIT", order = 1)]
public class WorkerScriptableObject : ScriptableObject
{
    public int hp = 100; //default = 100
    public int actionPoints = 4; //default = 4
    public int moveRange = 4; //default = 4
    public float interactEfficiency = 0.25f; //default = 0.25f
    public bool hasMeleeAttack;
    public bool hasRegenHealth;
}