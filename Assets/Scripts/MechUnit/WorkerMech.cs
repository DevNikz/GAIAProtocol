using UnityEngine;

public class WorkerMech : MonoBehaviour
{
    [SerializeField] WorkerScriptableObject workerTier;

    public void SetCurrentTier(WorkerScriptableObject tier) { workerTier = tier; } 

    public void SetCustomValues()
    {
        GetComponent<Unit>().SetHP(workerTier.hp);
        GetComponent<Unit>().SetAP(workerTier.actionPoints);
        GetComponent<MoveAction>().SetMoveDist(workerTier.moveRange);
        GetComponent<InteractAction>().SetInteractEfficiency(workerTier.interactEfficiency);
        GetComponent<Unit>().SetMeleeAction(workerTier.hasMeleeAttack);
        //GetComponent<SwordAction>().enabled = workerTier.hasMeleeAttack;
        GetComponent<Unit>().SetRegenHealth(workerTier.hasRegenHealth);
    }
}