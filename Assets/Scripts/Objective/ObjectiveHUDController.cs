using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Drives the objective HUD: a large panel for Main-type objectives at
/// the top, and a compact stacked list for Side-type objectives below.
///
/// Wired against the real ObjectiveManager/ObjectiveBase API:
///   - ObjectiveManager.Instance.GetAllObjectives() to populate
///   - static ObjectiveManager.OnObjectiveCompleted(int index)
///   - static ObjectiveManager.OnObjectiveProgressChanged(int index, float progress)
///     (added alongside this script — see NotifyProgress/NotifyProgressChanged)
///
/// Requires ObjectiveBase.GetIcon() (added), GetDisplayName(), GetObjectiveDesc(),
/// GetObjectiveType(), GetObjectiveIndex(), IsComplete(), GetProgress().
///
/// NOTE: GetProgress() is assumed to return a normalized 0-1 value. If your
/// ObjectiveCounterTarget/ObjectiveInteractFill implementations return raw
/// counts instead, either normalize inside GetProgress() or adjust
/// ObjectiveEntryView.SetProgress() to divide by a target you expose.
/// </summary>
public class ObjectiveHUDController : MonoBehaviour
{
    [Header("Main Objective Panel (large, top)")]
    [SerializeField]
    private Transform mainContainer;

    [SerializeField]
    private ObjectiveEntryView mainEntryPrefab;

    [Header("Side Objectives List (small, below)")]
    [SerializeField]
    private Transform sideContainer;

    [SerializeField]
    private ObjectiveEntryView sideEntryPrefab;

    [Header("Colors")]
    [SerializeField]
    private Color mainAccentColor = new Color(1f, 0.78f, 0.2f); // amber

    [SerializeField]
    private Color sideDefaultColor = new Color(0.75f, 0.8f, 0.85f);

    [SerializeField]
    private Color completedColor = new Color(0.4f, 0.85f, 0.55f);

    private readonly Dictionary<int, ObjectiveEntryView> _entries = new();

    private void OnEnable()
    {
        ObjectiveManager.OnObjectiveCompleted += HandleCompleted;
        ObjectiveManager.OnObjectiveProgressChanged += HandleProgress;

        // Rebuild whenever this panel becomes active (e.g. on level load,
        // after ObjectiveManager has finished Register()-ing objectives).
        if (ObjectiveManager.Instance != null)
            StartCoroutine(RebuildNextFrame());
    }

    private void OnDisable()
    {
        ObjectiveManager.OnObjectiveCompleted -= HandleCompleted;
        ObjectiveManager.OnObjectiveProgressChanged -= HandleProgress;
    }

    // Objectives register themselves in their own OnEnable, which can race
    // with this panel's OnEnable depending on scene load order — wait one
    // frame so ObjectiveManager.GetAllObjectives() is populated.
    private IEnumerator RebuildNextFrame()
    {
        yield return null;
        Rebuild();
    }

    /// <summary>Call this manually if you add/remove objectives mid-level
    /// outside of scene load (e.g. a side objective unlocked by a trigger).</summary>
    public void Rebuild()
    {
        foreach (Transform child in mainContainer)
            Destroy(child.gameObject);
        foreach (Transform child in sideContainer)
            Destroy(child.gameObject);
        _entries.Clear();

        foreach (var objective in ObjectiveManager.Instance.GetAllObjectives())
        {
            bool isMain = objective.GetObjectiveType() == ObjectiveType.Main;
            var prefab = isMain ? mainEntryPrefab : sideEntryPrefab;
            var parent = isMain ? mainContainer : sideContainer;
            var accent = isMain ? mainAccentColor : sideDefaultColor;

            var entry = Instantiate(prefab, parent);
            entry.Bind(objective, accent, completedColor);
            _entries[objective.GetObjectiveIndex()] = entry;
        }
    }

    private void HandleProgress(int index, float progress)
    {
        if (_entries.TryGetValue(index, out var entry))
            entry.SetProgress(progress);
    }

    private void HandleCompleted(int index)
    {
        if (_entries.TryGetValue(index, out var entry))
            entry.PlayCompleteAnimation(completedColor);
    }
}
