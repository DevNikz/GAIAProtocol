using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExtractionManager : MonoBehaviour
{
    public static ExtractionManager Instance { get; private set; }
    [SerializeField] private GameObject extractionArea;
    [SerializeField] private int currentSceneIndex;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentSceneIndex = scene.buildIndex;
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

    void ClearArea()
    {
        extractionArea = null;
    }

    void AddExtractionArea()
    {
        if(extractionArea != null) ClearArea();
        extractionArea = GameObject.FindGameObjectWithTag("Extract");
        //extractionArea.GetComponent<MeshRenderer>().enabled = false;

        extractionArea.GetComponent<BoxCollider>().enabled = false;
        extractionArea.transform.Find("Mesh").GetComponent<MeshRenderer>().enabled = false;
        //extractionArea.GetComponent<ExtractionArea>().enabled = false;
        //extractionArea.SetActive(false);
        //Debug.Log($"{extractionArea.name}");
    }

    public void SetExtraction()
    {
        extractionArea.GetComponent<BoxCollider>().enabled = true;
        extractionArea.transform.Find("Mesh").GetComponent<MeshRenderer>().enabled = true;
    }
}