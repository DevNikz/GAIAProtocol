using System;
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

[Serializable]
public struct ObjectiveReference
{
    public string name;
    public bool isComplete;
    public ObjectiveType objectiveType;

    public ObjectiveReference(string _n, bool _c, ObjectiveType _t)
    {
        name = _n;
        isComplete = _c;
        objectiveType = _t;
    }
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
    List<ObjectiveReference> objRef = new List<ObjectiveReference>();

    public void ClearObjRefs()
    {
        objRef.Clear();
    }

    public void AddObjRef(string _name, bool value, ObjectiveType type)
    {
        objRef.Add(new ObjectiveReference(_name, value, type));
    }

    [SerializeField]
    int currentLevel = 0;

    public void SetCurrentLevel(int value) => currentLevel = value;

    void PopulateObjectiveSummary()
    {
        ClearObjectiveSummary();
        foreach (var objective in objRef)
        {
            if (objective.objectiveType == ObjectiveType.Main)
            {
                GameObject entry = Instantiate(objectiveEntryPrefab, mainObjectiveList);
                entry
                    .GetComponent<ObjectiveSummaryEntryUI>()
                    .Setup(objective.name, objective.isComplete, objective.objectiveType);
                spawnedEntries.Add(entry);
            }
            else
            {
                GameObject entry = Instantiate(objectiveEntryPrefab, sideObjectiveList);
                entry
                    .GetComponent<ObjectiveSummaryEntryUI>()
                    .Setup(objective.name, objective.isComplete, objective.objectiveType);
                spawnedEntries.Add(entry);
            }
        }
    }

    void ClearObjectiveSummary()
    {
        if (spawnedEntries != null)
        {
            foreach (var entry in spawnedEntries)
                Destroy(entry);
            spawnedEntries.Clear();
        }
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
        SoundManager.Instance.PlaySFX("Return");
        Tween.Alpha(canvasGroup, hide);
        ResetValues();
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

    public int GetSideObjectivesCount(List<ObjectiveReference> objectiveReferences)
    {
        return objectiveReferences.Count(o => o.objectiveType != ObjectiveType.Main);
    }

    public void InitWin()
    {
        ClearStars();
        PopulateObjectiveSummary();

        if (GetSideObjectivesCount(objRef) > 0)
        {
            SetVisiblity(true);
            if (mainCompleted == true && sideCompleted == false)
                TwoStarsWin();
            else if (mainCompleted == true && sideCompleted == true)
                ThreeStarsWin();
            else
                return;
        }
        //No Side Objectives
        else
        {
            SetVisiblity(false);
            if (mainCompleted == true)
                ThreeStarsWin();
            else
                return;
        }
    }

    void SetVisiblity(bool value)
    {
        sideDivider.SetActive(value);
        sideHeader.SetActive(value);
        sideObjectiveList.gameObject.SetActive(value);
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
        pointsText.text = $"{points / 2}";

        CurrencyManager.Instance.SetResearchPoints(
            CurrencyManager.Instance.GetResearchPoints()
                + CurrencyManager.Instance.GetPromptedPoints() / 2
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
        Debug.Log("Rewards: Reset");
        HideCanvas();
        InputManager.Instance.EnableMechRotate();
        InputManager.Instance.EnableDebug();
        InputManager.Instance.EnableLevelCamera();
        InputManager.Instance.EnableLegacyInputs();
        SetRewardType(RewardsType.NONE);

        mainCompleted = false;
        sideCompleted = false;
        objRef.Clear();
        objRef = new List<ObjectiveReference>();

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
