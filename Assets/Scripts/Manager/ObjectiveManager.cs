using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance { get; private set; }
    [SerializeField] public List<ObjectiveObject> objectivesList;
    [SerializeField] public int numObjectives;
    private ObjectiveScreenUI objectiveScreenUI;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        objectiveScreenUI = GetComponent<ObjectiveScreenUI>();
    }

    public void ClearObjectives() 
    {
        objectivesList = new List<ObjectiveObject>();
        if(objectivesList != null) objectivesList.Clear();
    }

    public void AddObjective(ObjectiveObject obj)
    {
        objectivesList.Add(obj);
        numObjectives = objectivesList.Count;
    }

    public int GetObjectiveCount()
    {
        return numObjectives;
    }

    public bool CheckIndex(int index)
    {
        return objectivesList[index] != null;
    }

    public void SetComplete(int index)
    {
        objectivesList[index].isDone = true;
        objectiveScreenUI.SetToggleUI(index);
    }

    public bool GetComplete(int index)
    {
        return objectivesList[index].isDone;
    }
}
