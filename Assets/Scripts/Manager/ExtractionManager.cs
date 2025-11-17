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
                ClearArea();
                SetExtractionArea();
                break;
        }
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void ClearArea()
    {
        extractionArea = null;
    }

    void SetExtractionArea()
    {
        extractionArea = GameObject.FindGameObjectWithTag("Extract");
    }
}