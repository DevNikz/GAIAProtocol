using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CorruptionManager : MonoBehaviour
{
    public static CorruptionManager Instance { get; private set; }


    [Header("Stats")]
    [SerializeField] private List<float> corruptionList = new List<float> {0.33f, 0.33f, 0.33f};

    [Header("UI")]
    [SerializeField] private GameObject canvas;
    [SerializeField] private Material bar;

    [SerializeField] private int selectedAreaIndex;
    public int GetAreaIndex() { return selectedAreaIndex; }
    public void SetAreaIndex(int value) { selectedAreaIndex = value; }
    public float GetCorruption() { return corruptionList[selectedAreaIndex]; }
    public void SetCorruption(float value) { corruptionList[selectedAreaIndex] = value; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch(scene.buildIndex)
        {
            case 0:
                canvas.SetActive(true);
                break;
            case 1:
                canvas.SetActive(false);
                break;
        }
    }

    void LateUpdate()
    {
        bar.SetFloat("_Fill", corruptionList[selectedAreaIndex]);
    }


}
