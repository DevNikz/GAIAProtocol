using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class EnemyAIManager : MonoBehaviour
{
    public static EnemyAIManager Instance { get; private set; }

    private enum State { Idle, ProcessingEnemy, Done }
    private State state = State.Idle;

    [SerializeField] private List<EnemyAI> registeredEnemies = new List<EnemyAI>();

    // Index into registeredEnemies for the current pass.
    [SerializeField] private int currentEnemyIndex = 0;
    [SerializeField] bool wasPlayerTurn = false;

    // Small delay between each enemy action so it doesn't feel instant.
    [SerializeField] private float actionDelayTimer;
    [SerializeField] private float delayBetweenActions = 0.5f;
    [SerializeField] private float delayAtTurnStart   = 2f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        wasPlayerTurn = TurnSystem.Instance.IsPlayerTurn();
        //TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    private void OnDestroy()
    {
        //TurnSystem.Instance.OnTurnChanged -= TurnSystem_OnTurnChanged;
    }

    private void Update()
    {
        if(SceneManager.GetActiveScene().buildIndex != 0)
        {
            ProcessAI();
        }
    }

    void ProcessAI()
    {
        bool isPlayerTurn = TurnSystem.Instance.IsPlayerTurn();
 
        // Detect the moment the turn flips from player to enemy.
        // Using Update instead of OnTurnChanged avoids event subscription
        // ordering issues between EnemyAIManager and individual EnemyAI components.
        if (wasPlayerTurn && !isPlayerTurn && state == State.Idle)
        {
            currentEnemyIndex = 0;
            state = State.ProcessingEnemy;
            actionDelayTimer = delayAtTurnStart;
        }
 
        wasPlayerTurn = isPlayerTurn;
 
        if (isPlayerTurn) return;
        if (state == State.Idle) return;
 
        Debug.Log($"Wait..");
        actionDelayTimer -= Time.deltaTime;
        if (actionDelayTimer > 0f) return;
        
        Debug.Log($"Processing Enemy");
        //StartCoroutine(ProcessEnemy());
        if(!isPlayerTurn) ProcessNextEnemy();
        // if (TurnSystem.Instance.IsPlayerTurn()) return;
        // if (state == State.Idle) return;

        // actionDelayTimer -= Time.deltaTime;
        // if (actionDelayTimer > 0f) return;

        // ProcessNextEnemy();
    }

    // ── Registration (called by each EnemyAI) ───────────────────────────────

    public void Register(EnemyAI enemy)
    {
        if (!registeredEnemies.Contains(enemy))
            registeredEnemies.Add(enemy);
    }

    public void Unregister(EnemyAI enemy)
    {
        registeredEnemies.Remove(enemy);
    }

    // ── Turn flow ────────────────────────────────────────────────────────────

    // private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    // {
    //     if (TurnSystem.Instance.IsPlayerTurn()) return;
    //     else{
    //         // Enemy turn just started — begin sequencing.
    //         Debug.Log($"Processing Enemy");
    //         currentEnemyIndex = 0;
    //         state = State.ProcessingEnemy;
    //         actionDelayTimer = delayAtTurnStart;
    //     }
    // }

    private void ProcessNextEnemy()
    {
        if (TurnSystem.Instance.IsPlayerTurn()) return;
        else
        {
            Debug.Log($"Enemy's Turn | Is Player Turn: {TurnSystem.Instance.IsPlayerTurn()}");

            // Skip dead / destroyed entries.
            //while (currentEnemyIndex < registeredEnemies.Count && registeredEnemies[currentEnemyIndex] == null) currentEnemyIndex++;

            // if (currentEnemyIndex >= registeredEnemies.Count)
            // {
            //     Debug.Log($"Process Next Enemy: Next Turn | {TurnSystem.Instance.IsPlayerTurn()}");
            //     // All enemies processed — end the enemy turn.
            //     state = State.Idle;
            //     TurnSystem.Instance.NextTurn();
            //     return;
            // }

            EnemyAI currentEnemy = registeredEnemies[currentEnemyIndex];
            Debug.Log($"Current Enemy: {currentEnemy.name}");

            bool actionTaken = currentEnemy.TryTakeAction(OnEnemyActionComplete);
            if (!actionTaken)
            {
                Debug.Log($"{currentEnemy.name} can't take action");
                // This enemy had nothing to do (sleeping, no AP, etc.) — move on immediately.
                currentEnemyIndex++;
                actionDelayTimer = 0f;
                if(currentEnemyIndex >= registeredEnemies.Count)
                {
                    Debug.Log($"All Enemies have been iterated. Time for Player's turn.");
                    state = State.Idle;
                    TurnSystem.Instance.NextTurn();
                    return;
                }
                return;
            }
            Debug.Log($"Trying Action: {actionTaken}");
            // If actionTaken == true, we wait for the OnEnemyActionComplete callback.
        }
        return;
    }

    private void OnEnemyActionComplete()
    {
        // The current enemy finished its action; try to take another action with
        // the same enemy before advancing (enemies can have multiple AP).
        Debug.Log($"Enemy Action Completed");
        actionDelayTimer = delayBetweenActions;

        // if (currentEnemyIndex >= registeredEnemies.Count)
        // {
        //     Debug.Log($"Enemy Action Completed: Next Turn | {TurnSystem.Instance.IsPlayerTurn()}");
        //     // List shrank mid-turn (enemy died during its own action) — just end the turn.
        //     state = State.Idle;
        //     TurnSystem.Instance.NextTurn();
        //     return;
        // }

        EnemyAI currentEnemy = registeredEnemies[currentEnemyIndex];
        currentEnemy.SetBusy(false);
        bool canActAgain = currentEnemy != null && currentEnemy.HasActionsRemaining();

        if (!canActAgain)
        {
            currentEnemyIndex++;
            if(currentEnemyIndex >= registeredEnemies.Count)
            {
                Debug.Log($"All Enemies have been iterated. Time for Player's turn.");
                state = State.Idle;
                TurnSystem.Instance.NextTurn();
                return;
            }
        }
        // Update runs next frame and calls ProcessNextEnemy after the delay.
    }
}
