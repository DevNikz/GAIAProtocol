using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExtractionManager : MonoBehaviour
{
    public static ExtractionManager Instance { get; private set; }
    [SerializeField] private GameObject extractionArea;
    [SerializeField] private int currentSceneIndex;
    [SerializeField] private int currentLevelIndex;

    [Header("UI")]
    [SerializeField] private GameObject extractButton;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        extractButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            SoundManager.Instance.PlaySFX("Extract");
            SetButtonVisible(false);
            HUBTransitioner.Instance.ExtractForest1();
            // CurrencyManager.Instance.SetResearchPoints(CurrencyManager.Instance.GetPromptedPoints());
            // WorldManager.Instance.SetWorldComplete(true, LevelManager.Instance.GetCurrentLevel() - 1);
            // LevelManager.Instance.LoadLevelIndex(extractButton.GetComponent<ChangeLevelButton>().sceneIndex);
            // SoundManager.Instance.PlaySFX("Extract");
            // //soundController.PlaySound(6);
        });
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // void OnDisable()
    // {
    //     SceneManager.sceneLoaded -= OnSceneLoaded;
    // }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentSceneIndex = scene.buildIndex;
        switch(scene.buildIndex)
        {
            case 0:
                ClearArea();
                SetButtonVisible(false);
                break;
            case 1:
            case 2:
            case 3:
                AddExtractionArea();
                break;
        }
    }

    void ClearArea()
    {
        extractionArea = null;
    }

    void AddExtractionArea()
    {
        if(extractionArea != null) ClearArea();
        extractionArea = GameObject.FindGameObjectWithTag("Extract");

        extractionArea.GetComponent<BoxCollider>().enabled = false;
        extractionArea.transform.Find("Mesh").GetComponent<MeshRenderer>().enabled = false;
    }

    public void SetExtraction()
    {
        extractionArea.GetComponent<BoxCollider>().enabled = true;
        extractionArea.transform.Find("Mesh").GetComponent<MeshRenderer>().enabled = true;
    }

    public void SetButtonVisible(bool value)
    {
        extractButton.SetActive(value);
    }
}