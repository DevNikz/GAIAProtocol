using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Example quest tracker: spawns one QuestEntryUI per active objective
/// (regardless of type - fill, instant, health, whatever else you add later)
/// and removes it from the list when that objective completes.
/// </summary>
public class QuestTrackerUI : MonoBehaviour
{
    [SerializeField]
    private Transform listContainer;

    [SerializeField]
    private QuestEntryUI questEntryPrefab;

    private readonly Dictionary<int, QuestEntryUI> entries = new Dictionary<int, QuestEntryUI>();

    private void OnEnable()
    {
        ObjectiveManager.OnObjectiveCompleted += HandleObjectiveCompleted;
    }

    private void OnDisable()
    {
        ObjectiveManager.OnObjectiveCompleted -= HandleObjectiveCompleted;
    }

    private void Start()
    {
        BuildList();
    }

    private void BuildList()
    {
        foreach (var objective in ObjectiveManager.Instance.GetActiveObjectives())
        {
            var entry = Instantiate(questEntryPrefab, listContainer);
            entry.Bind(objective);
            entries[objective.GetObjectiveIndex()] = entry;
        }
    }

    private void Update()
    {
        foreach (var entry in entries.Values)
            entry.RefreshProgress();
    }

    private void HandleObjectiveCompleted(int index)
    {
        if (entries.TryGetValue(index, out var entry))
        {
            entry.MarkComplete();
            entry.RefreshProgress();
            entries.Remove(index);
        }
    }
}
