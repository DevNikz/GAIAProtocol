using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance { get; private set; }
    private readonly Dictionary<int, ObjectiveBase> objectives =
        new Dictionary<int, ObjectiveBase>();
    private readonly HashSet<int> completedIndices = new HashSet<int>();

    private bool isInCutscene;

    public static event Action<int> OnObjectiveCompleted;

    [SerializeField]
    bool mainCompleted;

    public bool IsMainComplete() => mainCompleted;

    [SerializeField]
    bool sideCompleted;

    public bool IsSideCompleted() => sideCompleted;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public void ResetValues()
    {
        mainCompleted = false;
        sideCompleted = false;
        objectives.Clear();
        completedIndices.Clear();
    }

    public void Register(ObjectiveBase objective)
    {
        int index = objective.GetObjectiveIndex();
        if (objectives.ContainsKey(index) && objectives[index] != objective)
            Debug.LogWarning(
                $"ObjectiveManager: duplicate objective index {index} on '{objective.name}', overwriting previous entry."
            );

        objectives[index] = objective;
    }

    public void Unregister(ObjectiveBase objective)
    {
        int index = objective.GetObjectiveIndex();
        if (objectives.TryGetValue(index, out var current) && current == objective)
            objectives.Remove(index);
    }

    public bool CheckIndex(int index)
    {
        return !completedIndices.Contains(index);
    }

    public void SetComplete(int index)
    {
        if (completedIndices.Add(index))
        {
            OnObjectiveCompleted?.Invoke(index);
            CheckAndTriggerExtraction();
        }
    }

    public bool GetComplete(int index) => completedIndices.Contains(index);

    public bool IsInCutscene() => isInCutscene;

    public void SetInCutscene(bool value) => isInCutscene = value;

    public ObjectiveBase GetObjective(int index) =>
        objectives.TryGetValue(index, out var objective) ? objective : null;

    public float GetProgress(int index) => GetObjective(index)?.GetProgress() ?? 0f;

    public IEnumerable<ObjectiveBase> GetActiveObjectives() =>
        objectives.Values.Where(o => !o.IsComplete());

    public IEnumerable<ObjectiveBase> GetAllObjectives() => objectives.Values;

    public bool AreMainObjectivesComplete()
    {
        return objectives
            .Values.Where(o => o.GetObjectiveType() == ObjectiveType.Main)
            .All(o => o.IsComplete());
    }

    public bool AreSideObjectivesComplete()
    {
        return objectives
            .Values.Where(o => o.GetObjectiveType() == ObjectiveType.Side)
            .All(o => o.IsComplete());
    }

    public bool HasSideObjectives()
    {
        return objectives.Values.Any(o => o.GetObjectiveType() == ObjectiveType.Side);
    }

    public void CheckAndTriggerExtraction()
    {
        if (AreMainObjectivesComplete())
            mainCompleted = true;
        else
            return;

        if (AreSideObjectivesComplete())
            sideCompleted = true;

        if (ExtractionManager.Instance != null)
        {
            ExtractionManager.Instance.SetButtonVisible(true);
        }
        else
            Debug.LogWarning(
                "ObjectiveManager: ExtractionManager.Instance is null — cannot trigger extraction."
            );
    }
}

/*
public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance { get; private set; }

    [SerializeField]
    public List<ObjectiveObject> objectivesList;

    [SerializeField]
    public int numObjectives;
    private ObjectiveScreenUI objectiveScreenUI;

    [SerializeField]
    private bool areObjectivesDone;
    bool currentlyInCutscene;

    public bool IsInCutscene()
    {
        return currentlyInCutscene;
    }

    public void SetInCutscene(bool value)
    {
        currentlyInCutscene = value;
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        objectiveScreenUI = GetComponent<ObjectiveScreenUI>();

        //GameObject obj = GameObject.FindGameObjectWithTag("Default");
    }

    public void ResetSys()
    {
        areObjectivesDone = false;
    }

    public void ClearObjectives()
    {
        if (objectivesList != null)
            objectivesList.Clear();
        objectivesList = new List<ObjectiveObject>();
        if (objectivesList != null)
            objectivesList.Clear();
    }

    public void CheckComplete()
    {
        //bool hasFalse = myList.Any(item => item.IsComplete == false);
        areObjectivesDone = objectivesList.All(item => item.isDone);
        if (areObjectivesDone)
        {
            //ExtractionManager.Instance.SetExtraction();
            switch (LevelManager.Instance.GetCurrentLevel())
            {
                case 1:
                    HUBTransitioner.Instance.ExtractForest1();
                    break;
                case 2:
                    HUBTransitioner.Instance.ExtractForest2();
                    break;
            }
        }

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
        for (int i = 0; i < objectivesList.Count; i++)
        {
            if (objectivesList[i].isDone != true)
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
*/
