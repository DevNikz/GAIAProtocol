#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Linq;

public static class ObjectiveIndexValidator
{
    [MenuItem("Tools/Validate Objective Indices")]
    static void Validate()
    {
        var targets = Object.FindObjectsByType<ObjectiveCounterTarget>(FindObjectsSortMode.None);
        var groups = targets.GroupBy(t => t.GetObjectiveIndex());

        foreach (var group in groups.Where(g => g.Count() > 1))
        {
            Debug.LogError($"Duplicate index {group.Key} found on:");
            foreach (var t in group)
                Debug.LogError($"  - {t.name} (path: {GetPath(t.transform)})", t);
        }
    }

    static string GetPath(Transform t)
    {
        string path = t.name;
        while (t.parent != null)
        {
            t = t.parent;
            path = t.name + "/" + path;
        }
        return path;
    }
}
#endif
