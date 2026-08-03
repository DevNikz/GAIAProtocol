using UnityEngine;
using UnityEngine.SceneManagement;

public class HUDMarkerManager : MonoBehaviour
{
    public static HUDMarkerManager Instance { get; private set; }

    [SerializeField]
    private GameObject mainMarkerPrefab;

    [SerializeField]
    private GameObject sideMarkerPrefab;

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
        ObjectiveBase objective,
        Vector3 scale,
        Color color
    )
    {
        if (objective != null)
        {
            if (objective.GetObjectiveType() == ObjectiveType.Main)
            {
                var newMarker = Instantiate(
                    mainMarkerPrefab,
                    Vector3.zero,
                    Quaternion.identity,
                    MarkerRoot
                );
                newMarker.GetComponent<RectTransform>().localScale = scale;
                newMarker
                    .GetComponent<HUDMarkerTargetUI>()
                    .Bind(target, image, objective, objective.GetObjectiveIndex(), color);
            }
            else
            {
                var newMarker = Instantiate(
                    sideMarkerPrefab,
                    Vector3.zero,
                    Quaternion.identity,
                    MarkerRoot
                );
                newMarker.GetComponent<RectTransform>().localScale = scale;
                newMarker
                    .GetComponent<HUDMarkerTargetUI>()
                    .Bind(target, image, objective, objective.GetObjectiveIndex(), color);
            }
        }
    }
}
