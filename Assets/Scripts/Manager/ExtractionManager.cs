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
                ClearArea();
                SetButtonVisible(false);
                break;
            case 1:
                InitButton(1);
                AddExtractionArea();
                break;
            case 2:
                InitButton(2);
                AddExtractionArea();
                break;
            case 3:
                InitButton(3);
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
        if (extractionArea != null)
            ClearArea();
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

    //Win Condition
    public void InitButton(int index)
    {
        extractButton
            .GetComponent<Button>()
            .onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySFX("Extract");
                SetButtonVisible(false);
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
                }
            });
    }
}
