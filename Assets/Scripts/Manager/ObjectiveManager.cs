using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance { get; private set; }
    [SerializeField] public List<ObjectiveObject> objectivesList;
    [SerializeField] public int numObjectives;
    private ObjectiveScreenUI objectiveScreenUI;
    [SerializeField] private bool areObjectivesDone;
    bool currentlyInCutscene;
    public bool IsInCutscene() { return currentlyInCutscene; }
    public void SetInCutscene(bool value) { currentlyInCutscene = value; } 
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        objectiveScreenUI = GetComponent<ObjectiveScreenUI>();

        //GameObject obj = GameObject.FindGameObjectWithTag("Default");
    }

    public void ResetSys()
    {
        areObjectivesDone = false;
    }

    public void ClearObjectives() 
    {
        if(objectivesList != null) objectivesList.Clear();
        objectivesList = new List<ObjectiveObject>();
        if(objectivesList != null) objectivesList.Clear();
    }

    public void CheckComplete()
    {
        //bool hasFalse = myList.Any(item => item.IsComplete == false);
        areObjectivesDone = objectivesList.All(item => item.isDone);


        // for(int i = 0; i < objectivesList.Count; i++)
        // {
        //     if(objectivesList[i].isDone != true)
        //     {
        //         areObjectivesDone = false;
        //         //return;
        //     }
        //     else
        //     {
        //         areObjectivesDone = true;
        //         ExtractionManager.Instance.SetExtraction();
        //         return;
        //     }
        // }
    }

    public bool CheckCompleteBool()
    {
        for(int i = 0; i < objectivesList.Count; i++)
        {
            if(objectivesList[i].isDone != true)
            {
                return false;
            }
        }
        return true;
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
        CheckComplete();
    }

    public bool GetComplete(int index)
    {
        return objectivesList[index].isDone;
    }

    public bool GetObjectiveDone()
    {
        return areObjectivesDone;
    }
}
