using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExtractionManager : MonoBehaviour
{
    public static ExtractionManager Instance { get; private set; }

    [SerializeField]
    private GameObject extractionArea;

    [SerializeField]
    private int currentSceneIndex;

    [SerializeField]
    private int currentLevelIndex;

    [Header("UI")]
    [SerializeField]
    private GameObject extractButton;

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

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentSceneIndex = scene.buildIndex;
        switch (scene.buildIndex)
        {
            case 0:
                SetButtonVisible(false);
                break;
            case 1:
                InitButton(1);
                break;
            case 2:
                InitButton(2);
                break;
            case 3:
                InitButton(3);
                break;
            case 4:
                InitButton(4);
                break;
        }
    }

    public void SetButtonVisible(bool value)
    {
        extractButton.SetActive(value);
    }

    //Win Condition
    public void InitButton(int index)
    {
        Button btn = extractButton.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() =>
        {
            StartCoroutine(InitExtraction(index));
        });
    }

    IEnumerator InitExtraction(int index)
    {
        RewardsManager.Instance.SetMainCompleted(
            ObjectiveManager.Instance.AreMainObjectivesComplete()
        );
        RewardsManager.Instance.SetSideCompleted(
            ObjectiveManager.Instance.AreSideObjectivesComplete()
        );

        List<ObjectiveBase> allObjectives = new List<ObjectiveBase>();
        allObjectives = ObjectiveManager
            .Instance.GetAllObjectives()
            .Where(o => !(o is ObjectiveCounterTarget || o is WastePileCollectible))
            .OrderBy(o => o.GetObjectiveType()) // Main (0) before Side (1)
            .ToList();

        Debug.Log(
            $"Extraction Manager: allObjectives count = {allObjectives.Count}, unique names = {allObjectives.Select(o => o.GetDisplayName()).Distinct().Count()}"
        );

        RewardsManager.Instance.ClearObjRefs();

        foreach (var objective in allObjectives)
        {
            // Debug.Log(
            //     $"Extraction Manager: {objective.GetDisplayName()} ({objective.GetObjectiveType()}), complete={objective.IsComplete()}"
            // );

            RewardsManager.Instance.AddObjRef(
                objective.GetDisplayName(),
                objective.IsComplete(),
                objective.GetObjectiveType()
            );
        }

        SoundManager.Instance.PlaySFX("Extract");

        yield return new WaitForSeconds(0.5f);
        switch (index)
        {
            case 1: //Forest 1
                HUBTransitioner.Instance.ExtractForest1();
                break;
            case 2: //Forest 1
                HUBTransitioner.Instance.ExtractForest2();
                break;
            case 3: //Forest 1
                HUBTransitioner.Instance.ExtractForest3();
                break;
            case 4: //Forest 1
                HUBTransitioner.Instance.ExtractForest4();
                break;
        }

        SetButtonVisible(false);
    }
}
