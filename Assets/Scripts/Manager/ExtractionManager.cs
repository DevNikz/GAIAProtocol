using UnityEngine;
using UnityEngine.SceneManagement;

public class ExtractionManager : MonoBehaviour
{
    public static ExtractionManager Instance { get; private set; }
    [SerializeField] private GameObject extractionArea;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch(scene.buildIndex)
        {
            case 0:
                ClearArea();
                break;
            case 1:
                AddExtractionArea();
                break;
        }
    }

    void Update()
    {
        CheckObjective();
    }

    void CheckObjective()
    {
        if(ObjectiveManager.Instance.GetObjectiveDone())
        {
            SetExtraction();
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
        extractionArea.GetComponent<MeshRenderer>().enabled = false;
        extractionArea.GetComponent<BoxCollider>().enabled = false;
        //extractionArea.GetComponent<ExtractionArea>().enabled = false;
        //extractionArea.SetActive(false);
        Debug.Log($"{extractionArea.name}");
    }

    void SetExtraction()
    {
        extractionArea.GetComponent<MeshRenderer>().enabled = true;
        extractionArea.GetComponent<BoxCollider>().enabled = true;
        //extractionArea.GetComponent<ExtractionArea>().enabled = true;
    }
}