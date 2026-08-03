using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A single objective row/panel. Used both as the large main-objective
/// entry and the compact side-objective entry — just use two prefab
/// variants (different sizes/fonts) with this same script attached.
/// </summary>
public class ObjectiveEntryView : MonoBehaviour
{
    [Header("Core")]
    [SerializeField]
    private Image icon;

    [SerializeField]
    private TextMeshProUGUI titleText;

    [SerializeField]
    private TextMeshProUGUI shadowTitleText;

    [SerializeField]
    private TextMeshProUGUI descText; // optional, can be left null on side prefab

    [SerializeField]
    private TextMeshProUGUI shadowDescText;

    [Header("Progress (optional — hide bar for binary objectives)")]
    [SerializeField]
    private Image progressBar;

    [SerializeField]
    private TextMeshProUGUI progressLabel;

    [SerializeField]
    private TextMeshProUGUI shadowProgressLabel;

    [Header("Completion")]
    [SerializeField]
    private Image checkboxFill; // empty ring -> filled on complete

    [SerializeField]
    private RectTransform strikeThroughLine; // scaled 0->1 on X on complete

    [Header("Animation")]
    [SerializeField]
    private float punchScale = 1.15f;

    [SerializeField]
    private float punchDuration = 0.25f;

    [SerializeField]
    private float progressTweenDuration = 0.35f;

    [SerializeField]
    private float completeTweenDuration = 0.3f;

    public RectTransform IconTransform => icon.rectTransform;

    private Color _activeColor;
    private Color _completedColor;
    private Coroutine _progressRoutine;

    public void Bind(ObjectiveBase objective, Color activeColor, Color completedColor)
    {
        _activeColor = activeColor;
        _completedColor = completedColor;

        icon.sprite = objective.GetIcon();
        titleText.text = objective.GetDisplayName();
        shadowTitleText.text = objective.GetDisplayName();
        if (descText != null)
        {
            descText.text = objective.GetObjectiveDesc();
            shadowDescText.text = objective.GetObjectiveDesc();
        }

        bool complete = objective.IsComplete();
        SetVisualState(complete ? completedColor : activeColor);

        if (progressBar != null)
        {
            progressBar.fillAmount = objective.GetProgress();
            UpdateProgressLabel(objective.GetProgress());
        }

        if (checkboxFill != null)
            checkboxFill.fillAmount = complete ? 1f : 0f;
        if (strikeThroughLine != null)
            strikeThroughLine.localScale = new Vector3(complete ? 1f : 0f, 1f, 1f);
    }

    /// <summary>Called from ObjectiveHUDController on OnObjectiveProgressChanged.</summary>
    public void SetProgress(float normalizedProgress)
    {
        if (progressBar == null)
            return;

        if (_progressRoutine != null)
            StopCoroutine(_progressRoutine);
        _progressRoutine = StartCoroutine(TweenProgress(normalizedProgress));
        PunchIcon();
    }

    private IEnumerator TweenProgress(float target)
    {
        float start = progressBar.fillAmount;
        float t = 0f;
        while (t < progressTweenDuration)
        {
            t += Time.deltaTime;
            float v = Mathf.Lerp(start, target, t / progressTweenDuration);
            progressBar.fillAmount = v;
            UpdateProgressLabel(v);
            yield return null;
        }
        progressBar.fillAmount = target;
        UpdateProgressLabel(target);
    }

    private void UpdateProgressLabel(float normalizedProgress)
    {
        if (progressLabel == null)
            return;
        // Percentage by default since raw counts aren't exposed on ObjectiveBase.
        // Swap for "{current}/{target}" if you add those getters to your
        // ObjectiveCounterTarget/ObjectiveInteractFill subclasses.
        progressLabel.text = $"{Mathf.RoundToInt(normalizedProgress * 100f)}%";
        shadowProgressLabel.text = $"{Mathf.RoundToInt(normalizedProgress * 100f)}%";
    }

    /// <summary>Called from ObjectiveHUDController on OnObjectiveCompleted.</summary>
    public void PlayCompleteAnimation(Color completedColor)
    {
        StopAllCoroutines();
        StartCoroutine(CompleteRoutine(completedColor));
    }

    private IEnumerator CompleteRoutine(Color completedColor)
    {
        if (progressBar != null)
            progressBar.fillAmount = 1f;
        if (progressLabel != null)
        {
            progressLabel.text = "100%";
            progressLabel.text = "100%";
        }

        yield return PunchIconRoutine();

        float t = 0f;
        while (t < completeTweenDuration)
        {
            t += Time.deltaTime;
            float p = t / completeTweenDuration;
            if (checkboxFill != null)
                checkboxFill.fillAmount = p;
            if (strikeThroughLine != null)
                strikeThroughLine.localScale = new Vector3(p, 1f, 1f);
            yield return null;
        }
        if (checkboxFill != null)
            checkboxFill.fillAmount = 1f;
        if (strikeThroughLine != null)
            strikeThroughLine.localScale = Vector3.one;

        SetVisualState(completedColor);
    }

    private void PunchIcon() => StartCoroutine(PunchIconRoutine());

    private IEnumerator PunchIconRoutine()
    {
        RectTransform rt = icon.rectTransform;
        float t = 0f;
        while (t < punchDuration)
        {
            t += Time.deltaTime;
            float p = t / punchDuration;
            float scale = 1f + (punchScale - 1f) * Mathf.Sin(p * Mathf.PI);
            rt.localScale = Vector3.one * scale;
            yield return null;
        }
        rt.localScale = Vector3.one;
    }

    private void SetVisualState(Color color)
    {
        icon.color = color;
        titleText.color = color;
        //shadowTitleText.color = color;
        if (checkboxFill != null)
            checkboxFill.color = color;
    }
}
