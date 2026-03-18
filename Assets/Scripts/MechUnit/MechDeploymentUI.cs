using System.Collections.Generic;
using UnityEngine;

public class MechDeploymentUI : MonoBehaviour
{
    [Header("Units")]
    [SerializeField] int unitsDeployed = 1;
    [SerializeField] int workerUnitsDeployed = 1;
    [SerializeField] int rangerUnitsDeployed = 0;

    [Header("References")]
    [SerializeField] GameObject worker;
    [SerializeField] GameObject ranger;
    [SerializeField] GameObject addButton;
    [SerializeField] Transform content;

    [SerializeField] GameObject spawnedAddButton;

    [SerializeField] List<GameObject> mechSelectUIs;
    public void ClearList()
    {
        if(mechSelectUIs != null) mechSelectUIs.RemoveRange(1, mechSelectUIs.Count);
    }
    public void AddList(GameObject m)
    {
        mechSelectUIs.Add(m);
    }

    public void RemoveAtIndex(GameObject obj) 
    {
        int i = GetIndex(obj);
        RemoveGameObjectAtIndex(i);
        RemoveUnitAtIndex(i);
    }

    int GetIndex(GameObject m)
    {
        int temp = -1;
        for(int i = 0; i < mechSelectUIs.Count; i++)
        {
            if(mechSelectUIs[i] == m) temp = i; 
        }
        return temp;
    }

    void Awake()
    {
        content = transform.Find("MainUI/MechDeployment/Content/Base/Horizontal");
    }

    void Start()
    {
        spawnedAddButton = Instantiate(addButton, content);
    }

    public void AddWorkerUnit()
    {
        GameObject temp = Instantiate(worker, content);
        AddList(temp);
        //temp.GetComponent<MechSelectUI>().SetIndex(unitsDeployed);

        spawnedAddButton.transform.SetAsLastSibling();
        workerUnitsDeployed++;
        unitsDeployed++;
        MechManager.Instance.AddFriendlyUnit(FriendlyUnitType.WORKER);

        if(unitsDeployed >= 4)
        {
            Destroy(spawnedAddButton);
        }
    }

    public void AddRangerUnit()
    {
        GameObject temp = Instantiate(ranger, content);
        AddList(temp);
        //temp.GetComponent<MechSelectUI>().SetIndex(unitsDeployed);

        spawnedAddButton.transform.SetAsLastSibling();
        rangerUnitsDeployed++;
        unitsDeployed++;
        MechManager.Instance.AddFriendlyUnit(FriendlyUnitType.RANGER);

        if(unitsDeployed >= 4)
        {
            Destroy(spawnedAddButton);
        }
    }

    public void RemoveWorkerUnit()
    {
        unitsDeployed--;
        workerUnitsDeployed--;
        if(unitsDeployed == 3)
        {
            if(spawnedAddButton == null) spawnedAddButton = Instantiate(addButton, content);
        }
    }

    public void RemoveRangerUnit()
    {
        unitsDeployed--;
        rangerUnitsDeployed--;
        if(unitsDeployed == 3)
        {
            if(spawnedAddButton == null) spawnedAddButton = Instantiate(addButton, content);
        }
    }

    public void RemoveGameObjectAtIndex(int value)
    {
        mechSelectUIs.RemoveAt(value);
    } 
    public void RemoveUnitAtIndex(int value)
    {
        MechManager.Instance.RemoveFriendlyUnitAtIndex(value);
    }
    

    public void Deploy()
    {
        MechManager.Instance.SetUnitsToBeDeployed(unitsDeployed);
        MechManager.Instance.SetWorkerUnits(workerUnitsDeployed);
        MechManager.Instance.SetRangerUnits(rangerUnitsDeployed);
    }
}
