using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A single row in the quest tracker, bound to one ObjectiveBase.
/// Doesn't care which subtype it is - just reads GetProgress().
/// </summary>
public class QuestEntryUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI label;

    [SerializeField]
    private Image progressFill;

    private ObjectiveBase objective;

    public void Bind(ObjectiveBase target)
    {
        objective = target;
        //label.text = $"Objective {objective.GetObjectiveIndex()}";

        switch (objective.GetObjectiveType())
        {
            case ObjectiveType.Main:
                label.text = $"Main: {objective.GetObjectiveDesc()}";
                break;
            case ObjectiveType.Side:
                label.text = $"Side: {objective.GetObjectiveDesc()}";
                break;
        }
        //label.text = $"{objective.GetObjectiveIndex()}";
    }

    public void RefreshProgress()
    {
        if (objective == null)
            return;

        //progressFill.fillAmount = objective.GetProgress();
        progressFill.fillAmount = Mathf.Clamp01(objective.GetProgress() + 1f * Time.deltaTime);

        // Counters read nicer as "3/5" than a bare progress bar.
        if (objective is ObjectiveCounter counter)
        {
            switch (objective.GetObjectiveType())
            {
                case ObjectiveType.Main:
                    label.text =
                        $"Main: {objective.GetObjectiveDesc()} ({counter.GetCurrentCount()}/{counter.GetTargetCount()})";
                    break;
                case ObjectiveType.Side:
                    label.text =
                        $"Side: {objective.GetObjectiveDesc()} ({counter.GetCurrentCount()}/{counter.GetTargetCount()})";
                    break;
            }
        }
    }

    public void RefreshProgressComplete()
    {
        if (objective == null)
            return;

        //progressFill.fillAmount = objective.GetProgress();
        progressFill.fillAmount = Mathf.Clamp01(objective.GetProgress() + 1f * Time.deltaTime);

        // Counters read nicer as "3/5" than a bare progress bar.
        if (objective is ObjectiveCounter counter)
        {
            switch (objective.GetObjectiveType())
            {
                case ObjectiveType.Main:
                    label.text =
                        $"Main: {objective.GetObjectiveDesc()} ({counter.GetCurrentCount()}/{counter.GetTargetCount()}) (Complete)";
                    break;
                case ObjectiveType.Side:
                    label.text =
                        $"Side: {objective.GetObjectiveDesc()} ({counter.GetCurrentCount()}/{counter.GetTargetCount()}) (Complete)";
                    break;
            }
        }
    }

    public void MarkComplete()
    {
        label.text += " (Complete)";
        //Destroy(gameObject, 1.5f);
    }
}
