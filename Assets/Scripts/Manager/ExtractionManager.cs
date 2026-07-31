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
        extractButton
            .GetComponent<Button>()
            .onClick.AddListener(() =>
            {
                RewardsManager.Instance.SetMainCompleted(
                    ObjectiveManager.Instance.AreMainObjectivesComplete()
                );
                RewardsManager.Instance.SetSideCompleted(
                    ObjectiveManager.Instance.AreSideObjectivesComplete()
                );

                var allObjectives = ObjectiveManager
                    .Instance.GetAllObjectives()
                    .Where(o => !(o is ObjectiveCounterTarget) || !(o is WastePileCollectible))
                    .OrderBy(o => o.GetObjectiveType()) // Main (0) before Side (1)
                    .ToList();

                RewardsManager.Instance.SetObjectiveList(allObjectives);

                SoundManager.Instance.PlaySFX("Extract");
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
            });
    }
}
