using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnitManager : MonoBehaviour
{

    public static UnitManager Instance { get; private set; }


    public List<Unit> unitList = new List<Unit>();
    public List<Unit> friendlyUnitList;
    public List<Unit> enemyUnitList;

    public List<Transform> ReferenceUnitList; // workers = 0-3 | ranger = 4-6

    public void SetReferenceList(List<Transform> units) { ReferenceUnitList = units; }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        unitList = new List<Unit>();
        friendlyUnitList = new List<Unit>();
        enemyUnitList = new List<Unit>();

        Unit.OnAnyUnitSpawned += Unit_OnAnyUnitSpawned;
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        Unit.OnAnyUnitSpawned -= Unit_OnAnyUnitSpawned;
        Unit.OnAnyUnitDead -= Unit_OnAnyUnitDead;
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        unitList.Clear();
        friendlyUnitList.Clear();
        enemyUnitList.Clear();

        switch(scene.buildIndex)
        {
            case 1:
                SpawnUnit();
            break;
        }
    }

    void SpawnUnit()
    {
        int count = MechManager.Instance.GetUnitsToBeDeployed();
        List<FriendlyUnitType> types = MechManager.Instance.GetFriendlyUnits();
        for(int i = 0; i < count; i++)
        {
            if(types[i] == FriendlyUnitType.WORKER)
            {
                ReferenceUnitList[i].gameObject.SetActive(true);
            }
            else
            {
                ReferenceUnitList[i+3].gameObject.SetActive(true);
            }
            Debug.Log(i);
        }
        // int numWorker = MechManager.Instance.GetWorkerUnits();
        // int numRanger = MechManager.Instance.GetRangerUnits();

        // for(int i = 0; i < numWorker; i++)
        // {
        //     ReferenceUnitList[i].gameObject.SetActive(true);
        // }

        // for(int i = 0; i < numRanger; i++)
        // {
        //     ReferenceUnitList[i+4].gameObject.SetActive(true);
        // }
    }

    private void Unit_OnAnyUnitSpawned(object sender, EventArgs e)
    {
        Unit unit = sender as Unit;

        unitList.Add(unit);
        ObjectTransManager.Instance.AddUnit(unit.transform);

        if (unit.IsEnemy())
        {
            enemyUnitList.Add(unit);
        } else
        {
            friendlyUnitList.Add(unit);
        }
    }

    private void Unit_OnAnyUnitDead(object sender, EventArgs e)
    {
        Unit unit = sender as Unit;

        unitList.Remove(unit);

        if (unit.IsEnemy())
        {
            enemyUnitList.Remove(unit);
        }
        else
        {
            friendlyUnitList.Remove(unit);
            ObjectTransManager.Instance.RemoveUnit(unit.transform);
        }
    }

    public List<Unit> GetUnitList()
    {
        return unitList;
    }

    public List<Unit> GetFriendlyUnitList()
    {
        return friendlyUnitList;
    }

    public List<Unit> GetEnemyUnitList()
    {
        return enemyUnitList;
    }

    public void ClearRefList() { ReferenceUnitList.Clear(); }
}
