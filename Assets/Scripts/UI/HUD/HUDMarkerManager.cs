using UnityEngine;
using UnityEngine.SceneManagement;

public class HUDMarkerManager : MonoBehaviour
{
    public static HUDMarkerManager Instance { get; private set; }

    [SerializeField]
    private GameObject MarkerPrefab;

    [SerializeField]
    private Transform MarkerRoot;

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
        switch (scene.buildIndex)
        {
            case 0:
                if (MarkerRoot.childCount > 0)
                    ClearMarkers();
                break;
        }
    }

    public void ClearMarkers()
    {
        for (int i = MarkerRoot.childCount - 1; i >= 0; i--)
        {
            if (MarkerRoot.name != MarkerRoot.GetChild(i).name)
                Destroy(MarkerRoot.GetChild(i).gameObject);
        }
    }

    public void AddMarker(
        HUDMarkerInWorldTarget target,
        Sprite image,
        bool isObjective,
        int index = 0
    )
    {
        var newMarker = Instantiate(MarkerPrefab, Vector3.zero, Quaternion.identity, MarkerRoot);
        newMarker.GetComponent<HUDMarkerTargetUI>().Bind(target, image, isObjective, index);
    }
}
