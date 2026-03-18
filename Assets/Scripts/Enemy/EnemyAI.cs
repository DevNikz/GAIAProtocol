using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{

    private enum State
    {
        Sleeping,            // Inactive — not participating in turns.
        WaitingForEnemyTurn, // Awake but it is the player's turn.
        ReadyToAct,          // Awake, enemy turn active, has AP — will act when the manager calls.
        Patrolling,          // Player lost — moving toward last-seen position.
        // Sleeping,
        // WaitingForEnemyTurn,
        // TakingTurn,
        // Busy,
        // Patrolling
    }

    [SerializeField] private State state = State.Sleeping;
    [SerializeField] Unit unit;
    [SerializeField] private float timer;
    [SerializeField] private EnemyAIDetection detection;
    [SerializeField] private GridPosition lastSeenGridPos;
    [SerializeField] bool hasLastSeenPos = false;
    [SerializeField] bool patrolDestinationReached = false;
    [SerializeField] bool isBusy;
    public void SetBusy(bool value) { isBusy = value; }

    private void Awake()
    {
        detection = GetComponent<EnemyAIDetection>();
        unit = GetComponent<Unit>();
    }

    private void Start()
    {
        EnemyAIManager.Instance.Register(this);
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        detection.OnPlayerDetected += Detection_OnPlayerDetected;
        detection.OnPlayerLost += Detection_OnPlayerLost;
    }

    void OnDestroy()
    {
        EnemyAIManager.Instance.Unregister(this);
        TurnSystem.Instance.OnTurnChanged -= TurnSystem_OnTurnChanged;
        detection.OnPlayerDetected -= Detection_OnPlayerDetected;
        detection.OnPlayerLost -= Detection_OnPlayerLost;
    }

    public bool TryTakeAction(Action onActionComplete)
    {
        Debug.Log($"{name} | {state}");
        switch (state)
        {
            case State.Sleeping:
                return false;

            case State.WaitingForEnemyTurn:
                return false;

            case State.ReadyToAct:
                if(!isBusy) return TryTakeAttackOrMoveAction(onActionComplete);
                else return false;

            case State.Patrolling:
                return TryPatrolStep(onActionComplete);

            default:
                return false;
        }
    }

    public bool HasActionsRemaining()
    {
        if (state == State.Sleeping || state == State.WaitingForEnemyTurn)
            return false;

        foreach (BaseAction baseAction in unit.GetBaseActionArray())
        {
            if (unit.CanSpendActionPointsToTakeAction(baseAction))
                return true;
        }

        return false;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if (TurnSystem.Instance.IsPlayerTurn())
        {
            //Debug.Log($"Player's Turn");
            if (state != State.Sleeping)
                state = State.WaitingForEnemyTurn;
        }
        else
        {
            //Debug.Log($"{name} Turn | {state}");
            switch (state)
            {
                case State.WaitingForEnemyTurn:
                    state = State.ReadyToAct;
                    Debug.Log($"Ready To Act");
                    break;
                case State.Patrolling:
                    break;
                case State.Sleeping:
                    break;
            }
        }
        Debug.Log($"End");
    }

    private void Detection_OnPlayerDetected(object sender, EnemyAIDetection.OnPlayerDetectedEventArgs e)
    {
        lastSeenGridPos = e.detectedUnit.GetGridPosition();
        hasLastSeenPos = true;

        if (state == State.Sleeping || state == State.Patrolling)
        {
            state = TurnSystem.Instance.IsPlayerTurn()
                ? State.WaitingForEnemyTurn
                : State.ReadyToAct;
        }
    }

    private void Detection_OnPlayerLost(object sender, EnemyAIDetection.OnPlayerLostEventArgs e)
    {
        lastSeenGridPos = e.lastSeenGridPosition;
        hasLastSeenPos = true;
        patrolDestinationReached = false;

        if (state == State.ReadyToAct || state == State.WaitingForEnemyTurn)
        {
            state = TurnSystem.Instance.IsPlayerTurn()
                ? State.WaitingForEnemyTurn
                : State.Patrolling;
        }
    }

    // ── Action selection ─────────────────────────────────────────────────────

    private bool TryTakeAttackOrMoveAction(Action onActionComplete)
    {
        EnemyAIAction bestAction = null;
        BaseAction bestBaseAction = null;

        foreach (BaseAction baseAction in unit.GetBaseActionArray())
        {
            if (!unit.CanSpendActionPointsToTakeAction(baseAction))
                continue;

            EnemyAIAction candidate = baseAction.GetBestEnemyAIAction();
            if (candidate == null) continue;

            if (bestAction == null || candidate.actionValue > bestAction.actionValue)
            {
                bestAction = candidate;
                bestBaseAction = baseAction;
            }
        }

        if (bestAction != null && unit.TrySpendActionPointsToTakeAction(bestBaseAction))
        {
            bestBaseAction.TakeAction(bestAction.gridPosition, onActionComplete);
            Debug.Log($"Doing Action..");
            isBusy = true;
            return true;
        }

        Debug.Log($"No Action Taken");
        return false;
    }

    // ── Patrol ───────────────────────────────────────────────────────────────

    private bool TryPatrolStep(Action onActionComplete)
    {
        if (!hasLastSeenPos || patrolDestinationReached)
        {
            ConsiderSleeping();
            return false;
        }

        MoveAction moveAction = unit.GetAction<MoveAction>();
        if (moveAction == null || !unit.CanSpendActionPointsToTakeAction(moveAction))
        {
            ConsiderSleeping();
            return false;
        }

        if (unit.GetGridPosition() == lastSeenGridPos)
        {
            patrolDestinationReached = true;
            ConsiderSleeping();
            return false;
        }

        GridPosition destination = lastSeenGridPos;

        if (!moveAction.IsValidActionGridPosition(destination))
        {
            destination = GetClosestValidPositionTo(moveAction, destination);
            if (destination == unit.GetGridPosition())
            {
                patrolDestinationReached = true;
                ConsiderSleeping();
                return false;
            }
        }

        if (!unit.TrySpendActionPointsToTakeAction(moveAction))
            return false;

        moveAction.TakeAction(destination, onActionComplete);
        return true;
    }

    private void ConsiderSleeping()
    {
        state = detection.IsPlayerDetected() ? State.ReadyToAct : State.Sleeping;
    }

    private GridPosition GetClosestValidPositionTo(MoveAction moveAction, GridPosition target)
    {
        var validPositions = moveAction.GetValidActionGridPositionList();
        GridPosition closest = unit.GetGridPosition();
        float closestDist = float.MaxValue;
        Vector3 targetWorld = LevelGrid.Instance.GetWorldPosition(target);

        foreach (GridPosition pos in validPositions)
        {
            float dist = Vector3.Distance(LevelGrid.Instance.GetWorldPosition(pos), targetWorld);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = pos;
            }
        }

        return closest;
    }


    // /*
    // private void Update()
    // {
    //     // if (TurnSystem.Instance.IsPlayerTurn()) return;
    //     switch (state)
    //     {
    //         case State.Sleeping:
    //             break;
    //         case State.WaitingForEnemyTurn:
    //             break;
    //         case State.TakingTurn:
    //             if (TurnSystem.Instance.IsPlayerTurn()) return;

    //             timer -= Time.deltaTime;
    //             if(timer > 0f) break;

    //             if (TryTakeEnemyAIAction(SetStateTakingTurn))
    //             {
    //                 state = State.Busy;
    //             }
    //             else
    //             {
    //                 // All awake enemies have exhausted their actions; end the turn.
    //                 TurnSystem.Instance.NextTurn();
    //             }
                
    //             /*
    //             if (timer <= 0f)
    //             {
    //                 if (TryTakeEnemyAIAction(SetStateTakingTurn))
    //                 {
    //                     state = State.Busy;
    //                 } else
    //                 {
    //                     // No more enemies have actions they can take, end enemy turn
    //                     TurnSystem.Instance.NextTurn();
    //                 }
    //             }
    //             break;

    //         case State.Busy:
    //             break;
            
    //         case State.Patrolling:
    //             if (TurnSystem.Instance.IsPlayerTurn()) break;

    //             timer -= Time.deltaTime;
    //             if (timer > 0f) break;

    //             if (hasLastSeenPos && !patrolDestinationReached)
    //             {
    //                 if (TryMoveToLastSeenPosition(SetStatePatrolling))
    //                 {
    //                     state = State.Busy;
    //                 }
    //                 else
    //                 {
    //                     // Already at the destination (or no path); patrol complete.
    //                     patrolDestinationReached = true;
    //                     ConsiderSleeping();
    //                 }
    //             }
    //             else
    //             {
    //                 ConsiderSleeping();
    //             }
    //             break;
    //     }
    // }
    // */

    // private void SetStateTakingTurn()
    // {
    //     timer = 0.5f;
    //     state = State.TakingTurn;
    // }

    // private void SetStatePatrolling()
    // {
    //     timer = 0.5f;
    //     state = State.Patrolling;
    // }

    // private bool TryTakeAttackOrMoveAction(Action onActionComplete)
    // {
    //     EnemyAIAction bestAction = null;
    //     BaseAction bestBaseAction = null;

    //     foreach (BaseAction baseAction in unit.GetBaseActionArray())
    //     {
    //         if (!unit.CanSpendActionPointsToTakeAction(baseAction))
    //             continue;

    //         EnemyAIAction candidate = baseAction.GetBestEnemyAIAction();
    //         if (candidate == null) continue;

    //         if (bestAction == null || candidate.actionValue > bestAction.actionValue)
    //         {
    //             bestAction = candidate;
    //             bestBaseAction = baseAction;
    //         }
    //     }

    //     if (bestAction != null && unit.TrySpendActionPointsToTakeAction(bestBaseAction))
    //     {
    //         bestBaseAction.TakeAction(bestAction.gridPosition, onActionComplete);
    //         return true;
    //     }

    //     return false;
    // }

    // private void ConsiderSleeping()
    // {
    //     if (!detection.IsPlayerDetected())
    //     {
    //         state = State.Sleeping;
    //     }
    //     else
    //     {
    //         // Player came back into view while we were patrolling; resume normal turn.
    //         state = State.TakingTurn;
    //         timer = 0.5f;
    //     }
    // }

    // /*
    // private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    // {
    //     if (TurnSystem.Instance.IsPlayerTurn())
    //     {
    //         // Player's turn just started — just wait regardless of current state.
    //         if (state != State.Sleeping)
    //         {
    //             state = State.WaitingForEnemyTurn;
    //         }
    //         return;
    //     }

    //     // Enemy turn just started.
    //     switch (state)
    //     {
    //         case State.Sleeping:
    //             // Still asleep — do nothing.
    //             break;

    //         case State.WaitingForEnemyTurn:
    //             state = State.TakingTurn;
    //             timer = 2f;   // brief dramatic pause before the enemy acts
    //             break;

    //         case State.Patrolling:
    //             // Resume patrolling toward last-seen position.
    //             timer = 2f;
    //             break;
    //     }
    // }
    // */

    // /*
    // private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    // {
    //     if (!TurnSystem.Instance.IsPlayerTurn())
    //     {
    //         state = State.TakingTurn;
    //         timer = 2f;
    //     }
    // }
    // */

    // private void Detection_OnPlayerDetected(object sender, EnemyAIDetection.OnPlayerDetectedEventArgs e)
    // {
    //     if (state == State.Sleeping)
    //     {
    //         // Wake up! But only act on our turn.
    //         state = TurnSystem.Instance.IsPlayerTurn()
    //             ? State.WaitingForEnemyTurn
    //             : State.TakingTurn;

    //         timer = 0.5f;
    //     }
    //     else if (state == State.Patrolling)
    //     {
    //         // Player came back while we were patrolling.
    //         state = TurnSystem.Instance.IsPlayerTurn()
    //             ? State.WaitingForEnemyTurn
    //             : State.TakingTurn;

    //         timer = 0.5f;
    //     }

    //     // Update last-seen in case we lose them again later.
    //     lastSeenGridPos = e.detectedUnit.GetGridPosition();
    //     hasLastSeenPos = true;
    // }

    // private void Detection_OnPlayerLost(object sender, EnemyAIDetection.OnPlayerLostEventArgs e)
    // {
    //     lastSeenGridPos = e.lastSeenGridPosition;
    //     hasLastSeenPos = true;
    //     patrolDestinationReached = false;

    //     if (state == State.TakingTurn || state == State.WaitingForEnemyTurn)
    //     {
    //         state = TurnSystem.Instance.IsPlayerTurn()
    //             ? State.WaitingForEnemyTurn  // will switch to Patrolling next enemy turn
    //             : State.Patrolling;

    //         // Override state to Patrolling when it's the enemy's turn so the
    //         // TurnSystem_OnTurnChanged handler resumes patrol correctly.
    //         if (!TurnSystem.Instance.IsPlayerTurn())
    //         {
    //             state = State.Patrolling;
    //             timer = 1f;
    //         }
    //     }
    // }

    // private bool TryTakeEnemyAIAction(Action onEnemyAIActionComplete)
    // {
    //     foreach (Unit enemyUnit in UnitManager.Instance.GetEnemyUnitList())
    //     {
    //         if (TryTakeEnemyAIAction(enemyUnit, onEnemyAIActionComplete))
    //         {
    //             return true;
    //         }
    //     }

    //     return false;
    // }

    // private bool TryTakeEnemyAIAction(Unit enemyUnit, Action onEnemyAIActionComplete)
    // {
    //     EnemyAIAction bestEnemyAIAction = null;
    //     BaseAction bestBaseAction = null;

    //     foreach (BaseAction baseAction in enemyUnit.GetBaseActionArray())
    //     {
    //         if (!enemyUnit.CanSpendActionPointsToTakeAction(baseAction))
    //         {
    //             // Enemy cannot afford this action
    //             continue;
    //         }

    //         if (bestEnemyAIAction == null)
    //         {
    //             bestEnemyAIAction = baseAction.GetBestEnemyAIAction();
    //             bestBaseAction = baseAction;
    //         }
    //         else
    //         {
    //             EnemyAIAction testEnemyAIAction = baseAction.GetBestEnemyAIAction();
    //             if (testEnemyAIAction != null && testEnemyAIAction.actionValue > bestEnemyAIAction.actionValue)
    //             {
    //                 bestEnemyAIAction = testEnemyAIAction;
    //                 bestBaseAction = baseAction;
    //             }
    //         }

    //     }

    //     if (bestEnemyAIAction != null && enemyUnit.TrySpendActionPointsToTakeAction(bestBaseAction))
    //     {
    //         bestBaseAction.TakeAction(bestEnemyAIAction.gridPosition, onEnemyAIActionComplete);
    //         return true;
    //     }
    //     else
    //     {
    //         return false;
    //     }
    // }

    // private bool TryMoveToLastSeenPosition(Action onMoveComplete)
    // {
    //     Unit ownerUnit = GetComponent<Unit>();
    //     if (ownerUnit == null) return false;

    //     MoveAction moveAction = ownerUnit.GetAction<MoveAction>();
    //     if (moveAction == null) return false;

    //     if (!ownerUnit.CanSpendActionPointsToTakeAction(moveAction)) return false;

    //     // Already at the destination?
    //     if (ownerUnit.GetGridPosition() == lastSeenGridPos)
    //     {
    //         patrolDestinationReached = true;
    //         return false;
    //     }

    //     // Is the destination reachable in one move?
    //     if (!moveAction.IsValidActionGridPosition(lastSeenGridPos))
    //     {
    //         // Try to get as close as possible by finding the valid position
    //         // nearest to the last-seen point.
    //         GridPosition closest = GetClosestValidPositionTo(moveAction, lastSeenGridPos);
    //         if (closest == ownerUnit.GetGridPosition())
    //         {
    //             // Can't get any closer this turn.
    //             patrolDestinationReached = true;
    //             return false;
    //         }
    //         lastSeenGridPos = closest;
    //     }

    //     if (!ownerUnit.TrySpendActionPointsToTakeAction(moveAction)) return false;

    //     moveAction.TakeAction(lastSeenGridPos, onMoveComplete);
    //     return true;
    // }

    // private GridPosition GetClosestValidPositionTo(MoveAction moveAction, GridPosition target)
    // {
    //     Unit ownerUnit = GetComponent<Unit>();
    //     var validPositions = moveAction.GetValidActionGridPositionList();

    //     GridPosition closest = ownerUnit.GetGridPosition();
    //     float closestDist = float.MaxValue;

    //     Vector3 targetWorld = LevelGrid.Instance.GetWorldPosition(target);

    //     foreach (GridPosition pos in validPositions)
    //     {
    //         float dist = Vector3.Distance(LevelGrid.Instance.GetWorldPosition(pos), targetWorld);
    //         if (dist < closestDist)
    //         {
    //             closestDist = dist;
    //             closest = pos;
    //         }
    //     }

    //     return closest;
    // }

}
