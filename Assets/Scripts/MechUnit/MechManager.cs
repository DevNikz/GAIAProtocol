using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum WorkerTier
{
    TIER1,
    TIER2,
    TIER3
}

public enum RangerTier
{
    TIER1,
    TIER2,
    TIER3
}


public class MechManager : MonoBehaviour
{
    public static MechManager Instance { get; private set; }

    [Header("Properties")]
    [SerializeField] int unitsToBeDeployed;
    [SerializeField] int workerUnits;
    [SerializeField] int rangerUnits;
    [SerializeField] List<FriendlyUnitType> friendlyUnits = new List<FriendlyUnitType>();

    [Header("Worker")]
    [SerializeField] List<WorkerScriptableObject> tierWorkers; //0 = tier1 | 1 = tier2 | 2 = tier3
    [SerializeField] WorkerScriptableObject currentTierWorkerObject;

    public WorkerScriptableObject GetCurrentTierWorkerObject() { return currentTierWorkerObject; }
    public void SetCurrentTierWorkerObject(int index) { currentTierWorkerObject = tierWorkers[index]; }

    [Header("Ranger")]
    [SerializeField] List<RangerScriptableObject> tierRangers; //0 = tier1 | 1 = tier2 | 2 = tier3
    [SerializeField] RangerScriptableObject currentTierRangerObject;

    public RangerScriptableObject GetCurrentTierRangerObject() { return currentTierRangerObject; }
    public void SetCurrentTierRangerObject(int index) { currentTierRangerObject = tierRangers[index]; }

    public int GetUnitsToBeDeployed() { return unitsToBeDeployed; }
    public int GetWorkerUnits() { return workerUnits; }
    public int GetRangerUnits() { return rangerUnits; }
    public void SetUnitsToBeDeployed(int value) { unitsToBeDeployed = value; }
    public void SetWorkerUnits(int value) { workerUnits = value; }
    public void SetRangerUnits(int value) { rangerUnits = value; } 
    public List<FriendlyUnitType> GetFriendlyUnits() { return friendlyUnits; }
    public void AddFriendlyUnit(FriendlyUnitType value) { friendlyUnits.Add(value); }
    public void RemoveFriendlyUnitAtIndex(int i) { friendlyUnits.RemoveAt(i); }

    public void ClearUnitsDeployed() { unitsToBeDeployed = 1; }
    public void ClearWorkerUnits() { workerUnits = 1; }
    public void ClearRangerUnits() { rangerUnits = 0; }
    public void ClearFriendlyUnits() 
    { 
        friendlyUnits.Clear();
        friendlyUnits.Add(FriendlyUnitType.WORKER);
    }

    public void ClearAll()
    {
        ClearFriendlyUnits();
        ClearWorkerUnits();
        ClearRangerUnits();
        ClearUnitsDeployed();
    }


    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        InitWorkerTiers();
        InitRangerTiers();

        ClearAll();
        SetCurrentTierRangerObject(0);
        SetCurrentTierWorkerObject(0);
    }

    void InitWorkerTiers()
    {
        tierWorkers.Add(Resources.Load("Scriptables/WorkerT1") as WorkerScriptableObject);
        tierWorkers.Add(Resources.Load("Scriptables/WorkerT2") as WorkerScriptableObject);
        tierWorkers.Add(Resources.Load("Scriptables/WorkerT3") as WorkerScriptableObject);
    }

    void InitRangerTiers()
    {
        tierRangers.Add(Resources.Load("Scriptables/RangerT1") as RangerScriptableObject);
        tierRangers.Add(Resources.Load("Scriptables/RangerT2") as RangerScriptableObject);
        tierRangers.Add(Resources.Load("Scriptables/RangerT3") as RangerScriptableObject);
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch(scene.buildIndex)
        {
            case 0:
                ClearAll();
                break;
        }
    }

    public void UpgradeWorker(WorkerTier tier)
    {
        switch(tier)
        {
            case WorkerTier.TIER2:
                SetCurrentTierWorkerObject(1);
                break;

            case WorkerTier.TIER3:
                SetCurrentTierWorkerObject(2);
                break;
        }
    }

    public void UpgradeRanger(RangerTier tier)
    {
        switch(tier)
        {
            case RangerTier.TIER2:
                SetCurrentTierRangerObject(1);
                break;

            case RangerTier.TIER3:
                SetCurrentTierRangerObject(2);
                break;
        }
    }
}
