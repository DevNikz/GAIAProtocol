using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    [Header("Stats")]
    [SerializeField, Min(0)]
    private int researchPoints;

    [SerializeField, Min(0)]
    private int promptedPoints;

    [Header("UI")]
    [SerializeField]
    private GameObject canvas;

    [SerializeField]
    private TextMeshProUGUI pointsUI;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        researchPoints = 50;
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
        switch (scene.buildIndex)
        {
            case 0:
                canvas.SetActive(true);
                break;
            case 1:
            case 2:
            case 3:
            case 4:
                canvas.SetActive(false);
                break;
        }
    }

    void LateUpdate()
    {
        pointsUI.text = $"{researchPoints}";
    }

    public int GetResearchPoints()
    {
        return researchPoints;
    }

    public void SetResearchPoints(int value)
    {
        researchPoints = value;
    }

    public int GetPromptedPoints()
    {
        return promptedPoints;
    }

    public void SetPromptedPoints(int value)
    {
        promptedPoints = value;
    }

    public void DisableCanvas()
    {
        canvas.SetActive(false);
    }

    public void EnableCanvas()
    {
        canvas.SetActive(true);
    }
}
