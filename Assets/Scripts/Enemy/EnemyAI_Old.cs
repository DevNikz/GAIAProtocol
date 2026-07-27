using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyAI_Old : MonoBehaviour
{
    private enum State
    {
        WaitingForEnemyTurn,
        TakingTurn,
        Busy,
    }

    [SerializeField]
    private State state;

    [SerializeField]
    private float timer;
    private Coroutine wakeCoroutine;

    [SerializeField]
    bool playedWakeAnim;

    private void Awake()
    {
        state = State.WaitingForEnemyTurn;
    }

    void OnEnable()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    void OnDisable()
    {
        if (TurnSystem.Instance != null)
        {
            TurnSystem.Instance.OnTurnChanged -= TurnSystem_OnTurnChanged;
            playedWakeAnim = false;
        }
    }

    private void Update()
    {
        if (TurnSystem.Instance.IsPlayerTurn())
        {
            return;
        }

        switch (state)
        {
            case State.WaitingForEnemyTurn:
                break;
            case State.TakingTurn:
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    if (CheckKaijuState() && TryTakeKaijuAction(SetStateTakingTurn))
                    {
                        Debug.Log("Kaiju Action");
                        state = State.Busy;
                    }
                    else if (CheckOtherState() && TryTakeOtherEnemyAction(SetStateTakingTurn))
                    {
                        state = State.Busy;
                    }
                    else
                    {
                        // No more enemies have actions they can take, end enemy turn
                        //Debug.Log("Next Turn");
                        TurnSystem.Instance.NextTurn();
                    }
                }
                break;
            case State.Busy:
                break;
        }
    }

    private void SetStateTakingTurn()
    {
        timer = 0.5f;
        state = State.TakingTurn;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            if (CheckKaijuActive())
            {
                Debug.Log("Wake up Kaiju.");
                if (wakeCoroutine != null)
                    StopCoroutine(wakeCoroutine);
                wakeCoroutine = StartCoroutine(InitWake());
            }
            else
            {
                Debug.Log("Kaiju Disabled?");
                TurnSystem.Instance.NextTurn();
            }
        }
    }

    IEnumerator InitWake()
    {
        playedWakeAnim = DoKaijuWake();

        if (playedWakeAnim)
        {
            Debug.Log("animate kaiju");
            yield return new WaitForSeconds(6f);
        }

        // Debug.Log($"animate kaiju");
        // DoKaijuWake();

        Debug.Log($"Set Turn");
        state = State.TakingTurn;
        timer = 2f;
    }

    // void DoKaijuWake()
    // {
    //     foreach (Unit enemyUnit in UnitManager.Instance.GetKaijuList())
    //     {
    //         if (!enemyUnit.GetComponent<KaijuUnit>().HasAnimatedWake())
    //         {
    //             enemyUnit.GetComponent<KaijuUnit>().InitAnimateAwake();
    //         }
    //     }
    // }

    bool DoKaijuWake()
    {
        bool anyWoke = false;
        foreach (Unit enemyUnit in UnitManager.Instance.GetKaijuList())
        {
            var kaiju = enemyUnit.GetComponent<KaijuUnit>();
            if (!kaiju.HasAnimatedWake())
            {
                kaiju.InitAnimateAwake();
                anyWoke = true;
            }
        }
        return anyWoke;
    }

    bool CheckKaijuActive()
    {
        foreach (Unit enemyUnit in UnitManager.Instance.GetKaijuList())
        {
            var kaiju = enemyUnit.gameObject;
            if (kaiju.activeInHierarchy)
            {
                return true;
            }
        }
        return false;
    }

    bool TryTakeKaijuAction(Action onEnemyAIActionComplete)
    {
        //Do actions
        foreach (Unit enemyUnit in UnitManager.Instance.GetKaijuList())
        {
            if (TryTakeEnemyAIAction(enemyUnit, onEnemyAIActionComplete))
                return true;
        }
        return false;
    }

    bool TryTakeOtherEnemyAction(Action onEnemyAIActionComplete)
    {
        foreach (Unit enemyUnit in UnitManager.Instance.GetSmallEnemyList())
        {
            if (TryTakeEnemyAIAction(enemyUnit, onEnemyAIActionComplete))
                return true;
        }
        return false;
    }

    bool CheckKaijuState()
    {
        for (int i = 0; i < UnitManager.Instance.GetKaijuList().Count; i++)
        {
            //at least one is awake
            if (UnitManager.Instance.GetKaijuList()[i].GetComponent<KaijuUnit>().IsAwake())
            {
                return true;
            }
        }
        return false;
    }

    bool CheckOtherState()
    {
        if (
            UnitManager.Instance.GetSmallEnemyList() == null
            || UnitManager.Instance.GetSmallEnemyList().Count == 0
        )
            return false;
        return true;
    }

    private bool TryTakeEnemyAIAction(Unit enemyUnit, Action onEnemyAIActionComplete)
    {
        EnemyAIAction bestEnemyAIAction = null;
        BaseAction bestBaseAction = null;

        foreach (BaseAction baseAction in enemyUnit.GetBaseActionArray())
        {
            if (!enemyUnit.CanSpendActionPointsToTakeAction(baseAction))
            {
                // Enemy cannot afford this action
                continue;
            }

            if (bestEnemyAIAction == null)
            {
                bestEnemyAIAction = baseAction.GetBestEnemyAIAction();
                bestBaseAction = baseAction;
            }
            else
            {
                EnemyAIAction testEnemyAIAction = baseAction.GetBestEnemyAIAction();
                if (
                    testEnemyAIAction != null
                    && testEnemyAIAction.actionValue > bestEnemyAIAction.actionValue
                )
                {
                    bestEnemyAIAction = testEnemyAIAction;
                    bestBaseAction = baseAction;
                }
            }
        }

        if (bestEnemyAIAction != null && enemyUnit.TrySpendActionPointsToTakeAction(bestBaseAction))
        {
            bestBaseAction.TakeAction(bestEnemyAIAction.gridPosition, onEnemyAIActionComplete);
            return true;
        }
        else
        {
            return false;
        }
    }
}
