using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MechManager : MonoBehaviour
{
    public static MechManager Instance { get; private set; }

    [SerializeField] int unitsToBeDeployed;
    [SerializeField] int workerUnits;
    [SerializeField] int rangerUnits;
    [SerializeField] List<FriendlyUnitType> friendlyUnits = new List<FriendlyUnitType>();

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

        ClearAll();
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
}
