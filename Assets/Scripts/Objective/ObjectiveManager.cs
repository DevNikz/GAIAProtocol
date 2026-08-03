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
    public static event Action<int, float> OnObjectiveProgressChanged;

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

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.buildIndex)
        {
            default:
                break;
            case 1:
            case 2:
            case 3:
            case 4:
                Debug.Log($"Objectives Count: {objectives.Count}");
                Debug.Log($"Main Objectives: {GetMainObjectivesCount()}");
                Debug.Log($"Side Objectives: {GetSideObjectivesCount()}");
                break;
        }
    }

    public void ResetValues()
    {
        mainCompleted = false;
        sideCompleted = false;
        objectives.Clear();
        Debug.Log($"Objectives | Count: {objectives.Count}");
        completedIndices.Clear();
    }

    public void Register(ObjectiveBase objective)
    {
        Debug.Log($"{objective.GetDisplayName()} | {objective.GetObjectiveIndex()}");
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

    public void NotifyProgress(int index)
    {
        var objective = GetObjective(index);
        if (objective != null)
            OnObjectiveProgressChanged?.Invoke(index, objective.GetProgress());
    }

    public bool IsInCutscene() => isInCutscene;

    public void SetInCutscene(bool value) => isInCutscene = value;

    public ObjectiveBase GetObjective(int index) =>
        objectives.TryGetValue(index, out var objective) ? objective : null;

    public float GetProgress(int index)
    {
        ObjectiveBase objective = GetObjective(index);
        return objective != null ? objective.GetProgress() : 0f;
    }

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
        if (GetSideObjectivesCount() > 0)
        {
            return objectives
                .Values.Where(o => o != null && o.GetObjectiveType() == ObjectiveType.Side)
                .All(o => o.IsComplete());
        }
        else
            return false;
    }

    public bool AreSideObjectivesComplete(List<ObjectiveBase> value)
    {
        if (GetSideObjectivesCount(value) > 0)
        {
            return value
                .Where(o => o != null && o.GetObjectiveType() == ObjectiveType.Side)
                .All(o => o.IsComplete());
        }
        else
            return false;
    }

    public int GetSideObjectivesCount()
    {
        return objectives.Values.Count(o =>
            o != null && o.GetObjectiveType() == ObjectiveType.Side
        );
    }

    public int GetSideObjectivesCount(List<ObjectiveBase> objList)
    {
        return objList.Count(o => o != null && o.GetObjectiveType() != ObjectiveType.Main);
    }

    public int GetMainObjectivesCount()
    {
        return objectives.Values.Count(o => o.GetObjectiveType() == ObjectiveType.Main);
    }

    public void CheckAndTriggerExtraction()
    {
        if (AreSideObjectivesComplete())
            sideCompleted = true;

        if (AreMainObjectivesComplete())
            mainCompleted = true;
        else
            return;

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
