using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnitManager : MonoBehaviour
{

    public static UnitManager Instance { get; private set; }


    public List<Unit> unitList = new List<Unit>();
    public List<Unit> friendlyUnitList;
    public List<Unit> kaijuEnemyList;
    public List<Unit> smallEnemyUnitList;

    public List<Unit> GetKaijuList() { return kaijuEnemyList; }
    public List<Unit> GetSmallEnemyList() { return smallEnemyUnitList; }

    public List<Transform> ReferenceUnitList; // workers = 0-3 | ranger = 4-6
    public List<Transform> ReferenceEnemyUnitList;

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
        kaijuEnemyList = new List<Unit>();
        smallEnemyUnitList = new List<Unit>();

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
        kaijuEnemyList.Clear();
        smallEnemyUnitList.Clear();

        switch(scene.buildIndex)
        {
            case 1:
                SpawnFriendlyUnits();
            break;
        }
    }

    void SpawnFriendlyUnits()
    {
        //SpawnFriendlyUnits
        int count = MechManager.Instance.GetUnitsToBeDeployed();
        List<FriendlyUnitType> types = MechManager.Instance.GetFriendlyUnits();
        for(int i = 0; i < count; i++)
        {
            if(types[i] == FriendlyUnitType.WORKER)
            {
                ReferenceUnitList[i].GetComponent<WorkerMech>().SetCurrentTier(MechManager.Instance.GetCurrentTierWorkerObject());
                ReferenceUnitList[i].GetComponent<WorkerMech>().SetCustomValues();
                ReferenceUnitList[i].gameObject.SetActive(true);
            }
            else
            {
                ReferenceUnitList[i+3].GetComponent<RangerMech>().SetCurrentTier(MechManager.Instance.GetCurrentTierRangerObject());
                ReferenceUnitList[i+3].GetComponent<RangerMech>().SetCustomValues();
                ReferenceUnitList[i+3].gameObject.SetActive(true);
            }
        }
    }

    private void Unit_OnAnyUnitSpawned(object sender, EventArgs e)
    {
        Unit unit = sender as Unit;

        unitList.Add(unit);
        //ObjectTransManager.Instance.AddUnit(unit.transform);

        if (unit.IsEnemy() && unit.CompareTag("Kaiju"))
        {
            kaijuEnemyList.Add(unit);
        }
        else if(unit.IsEnemy() && !unit.CompareTag("Kaiju"))
        {
            smallEnemyUnitList.Add(unit);
        }
        else
        {
            friendlyUnitList.Add(unit);
        }
    }

    private void Unit_OnAnyUnitDead(object sender, EventArgs e)
    {
        Unit unit = sender as Unit;

        unitList.Remove(unit);

        if (unit.IsEnemy() && unit.CompareTag("Kaiju"))
        {
            kaijuEnemyList.Remove(unit);
        }
        else if(unit.IsEnemy() && !unit.CompareTag("Kaiju"))
        {
            smallEnemyUnitList.Remove(unit);
        }
        else
        {
            friendlyUnitList.Remove(unit);
            //ObjectTransManager.Instance.RemoveUnit(unit.transform);
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

    public void ClearRefList() { ReferenceUnitList.Clear(); }
}
