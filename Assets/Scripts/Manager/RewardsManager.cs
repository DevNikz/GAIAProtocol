using System.Collections.Generic;
using PrimeTween;
using TMPro;
using UnityEngine;

public enum RewardsType
{
    NONE,
    WIN,
    LOSE,
}

public class RewardsManager : MonoBehaviour
{
    public static RewardsManager Instance;

    [SerializeField]
    GameObject canvas;

    [SerializeField]
    CanvasGroup canvasGroup;

    public void ShowCanvas()
    {
        canvas.SetActive(true);
    }

    public void HideCanvas()
    {
        canvas.SetActive(false);
    }

    [SerializeField]
    TweenSettings<float> show;

    [SerializeField]
    TweenSettings<float> hide;

    //Get if win or lose
    [SerializeField]
    TextMeshProUGUI status,
        shadow;

    [SerializeField]
    List<GameObject> NoStars,
        Stars;

    [SerializeField]
    RewardsType rewards;

    public RewardsType GetRewards()
    {
        return rewards;
    }

    public void SetRewardType(RewardsType type)
    {
        rewards = type;
    }

    //Get number of points won
    [SerializeField]
    TextMeshProUGUI pointsText;

    [SerializeField]
    int points;

    public void SetPoints(int value)
    {
        points = value;
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
    }

    public void AnimateShow()
    {
        InputManager.Instance.DisableMechRotate();
        InputManager.Instance.DisableDebug();
        InputManager.Instance.DisableLevelCamera();
        InputManager.Instance.DisableLegacyInputs();

        switch (rewards)
        {
            case RewardsType.WIN:
                Win();
                break;
            case RewardsType.LOSE:
                Lose();
                break;
        }

        Tween.Alpha(canvasGroup, show);
    }

    public void AnimateHide()
    {
        Tween.Alpha(canvasGroup, hide).OnComplete(ResetValues);
    }

    public void Lose()
    {
        ClearStars();
        Stars[0].SetActive(true);
        NoStars[0].SetActive(true);
        NoStars[1].SetActive(true);

        status.text = "MISSION LOST";
        shadow.text = "MISSION LOST";
        pointsText.text = "0";
    }

    public void Win()
    {
        ClearStars();
        for (int i = 0; i < Stars.Count; i++)
        {
            Stars[i].SetActive(true);
        }

        status.text = "MISSION COMPLETED";
        shadow.text = "MISSION COMPLETED";
        pointsText.text = $"{points}";
    }

    public void ResetValues()
    {
        HideCanvas();
        InputManager.Instance.EnableMechRotate();
        InputManager.Instance.EnableDebug();
        InputManager.Instance.EnableLevelCamera();
        InputManager.Instance.EnableLegacyInputs();
        SetRewardType(RewardsType.NONE);

        status.text = "";
        shadow.text = "";
        pointsText.text = "";
        ClearStars();
    }

    void ClearStars()
    {
        for (int i = 0; i < NoStars.Count; i++)
        {
            NoStars[i].SetActive(false);
        }

        for (int i = 0; i < Stars.Count; i++)
        {
            Stars[i].SetActive(false);
        }
    }
}
