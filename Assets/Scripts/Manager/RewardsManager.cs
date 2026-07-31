using System.Collections.Generic;
using System.Linq;
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

    [SerializeField]
    bool mainCompleted;

    public void SetMainCompleted(bool value) => mainCompleted = value;

    [SerializeField]
    bool sideCompleted;

    public void SetSideCompleted(bool value) => sideCompleted = value;

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

    //Objectives
    [SerializeField]
    Transform mainObjectiveList;

    [SerializeField]
    Transform sideObjectiveList;

    [SerializeField]
    GameObject objectiveEntryPrefab;
    private readonly List<GameObject> spawnedEntries = new List<GameObject>();

    [SerializeField]
    private GameObject sideDivider;

    [SerializeField]
    private GameObject sideHeader;

    [SerializeField]
    List<ObjectiveBase> objectiveList;

    [SerializeField]
    int currentLevel = 0;

    public void SetCurrentLevel(int value) => currentLevel = value;

    public void SetObjectiveList(List<ObjectiveBase> value) => objectiveList = value;

    void PopulateObjectiveSummary()
    {
        ClearObjectiveSummary();

        Debug.Log($"PopulateObjectiveSummary: found {objectiveList.Count} objectives");

        foreach (var objective in objectiveList)
        {
            Debug.Log(
                $"  - {objective.GetDisplayName()} ({objective.GetObjectiveType()}), complete={objective.IsComplete()}"
            );
            if (objective.GetObjectiveType() == ObjectiveType.Main)
            {
                GameObject entry = Instantiate(objectiveEntryPrefab, mainObjectiveList);
                entry
                    .GetComponent<ObjectiveSummaryEntryUI>()
                    .Setup(
                        objective.GetDisplayName(),
                        objective.IsComplete(),
                        objective.GetObjectiveType()
                    );
                spawnedEntries.Add(entry);
            }
            else
            {
                GameObject entry = Instantiate(objectiveEntryPrefab, sideObjectiveList);
                entry
                    .GetComponent<ObjectiveSummaryEntryUI>()
                    .Setup(
                        objective.GetDisplayName(),
                        objective.IsComplete(),
                        objective.GetObjectiveType()
                    );
                spawnedEntries.Add(entry);
            }
        }
    }

    void ClearObjectiveSummary()
    {
        foreach (var entry in spawnedEntries)
            Destroy(entry);
        spawnedEntries.Clear();
    }

    public void SetPoints(int value)
    {
        points = value; //max value. -2 if side objectives not completed instead
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
                if (currentLevel == 1)
                    CorruptionManager.Instance.SetCorruption(0.33f);
                else if (currentLevel == 2)
                    CorruptionManager.Instance.SetCorruption(0.52f);
                else if (currentLevel == 3)
                    CorruptionManager.Instance.SetCorruption(0.7f);
                else if (currentLevel == 4)
                    CorruptionManager.Instance.SetCorruption(1f);
                InitWin();
                break;
            case RewardsType.LOSE:
                Lose();
                break;
        }

        Tween.Alpha(canvasGroup, show);
    }

    public void AnimateHide()
    {
        InputManager.Instance.EnableMechRotate();
        InputManager.Instance.EnableDebug();
        InputManager.Instance.EnableLevelCamera();
        InputManager.Instance.EnableLegacyInputs();

        Tween.Alpha(canvasGroup, hide).OnComplete(ResetValues);
        ObjectiveManager.Instance.ResetValues();
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

    public void InitWin()
    {
        ClearStars();

        if (AreSideObjectivesComplete(objectiveList))
            SetVisiblity(true);
        else
            SetVisiblity(false);

        PopulateObjectiveSummary();

        if (mainCompleted == true && sideCompleted == false)
            TwoStarsWin();
        else if (mainCompleted == true && sideCompleted == true)
            ThreeStarsWin();
        else
            return;
    }

    void SetVisiblity(bool value)
    {
        sideDivider.SetActive(value);
        sideHeader.SetActive(value);
        sideObjectiveList.gameObject.SetActive(value);
    }

    public bool AreSideObjectivesComplete(List<ObjectiveBase> value)
    {
        return value.Any(o => o.GetObjectiveType() == ObjectiveType.Side);
    }

    void TwoStarsWin()
    {
        for (int i = 0; i < 2; i++)
        {
            Stars[i].SetActive(true);
        }
        for (int i = 0; i < 1; i++)
        {
            NoStars[i].SetActive(true);
        }

        status.text = "MISSION COMPLETED";
        shadow.text = "MISSION COMPLETED";
        pointsText.text = $"{points - 2}";

        CurrencyManager.Instance.SetResearchPoints(
            CurrencyManager.Instance.GetResearchPoints()
                + CurrencyManager.Instance.GetPromptedPoints()
                - 2
        );
    }

    void ThreeStarsWin()
    {
        for (int i = 0; i < 3; i++)
        {
            Stars[i].SetActive(true);
        }

        status.text = "MISSION COMPLETED";
        shadow.text = "MISSION COMPLETED";
        pointsText.text = $"{points}";
        CurrencyManager.Instance.SetResearchPoints(
            CurrencyManager.Instance.GetResearchPoints()
                + CurrencyManager.Instance.GetPromptedPoints()
        );
    }

    public void ResetValues()
    {
        HideCanvas();
        InputManager.Instance.EnableMechRotate();
        InputManager.Instance.EnableDebug();
        InputManager.Instance.EnableLevelCamera();
        InputManager.Instance.EnableLegacyInputs();
        SetRewardType(RewardsType.NONE);

        mainCompleted = false;
        sideCompleted = false;
        objectiveList.Clear();

        status.text = "";
        shadow.text = "";
        pointsText.text = "";
        ClearStars();
        ClearObjectiveSummary();
    }

    void ClearStars()
    {
        for (int i = 0; i < 3; i++)
        {
            NoStars[i].SetActive(false);
        }

        for (int i = 0; i < 3; i++)
        {
            Stars[i].SetActive(false);
        }
    }
}
