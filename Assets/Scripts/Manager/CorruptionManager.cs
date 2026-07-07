using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CorruptionManager : MonoBehaviour
{
    public static CorruptionManager Instance { get; private set; }

    /*
    
Corruption
1 - 0.33
2 - .52
3 - .7

    */

    [Header("Stats")]
    [SerializeField]
    private List<float> corruptionList = new List<float> { 0.33f, 0.33f }; //Planet Corruption

    [SerializeField]
    private int promptedCorruptionIndex;

    public int GetPromptedCorruptionIndex()
    {
        return promptedCorruptionIndex;
    }

    public void SetPromptedCorruptionIndex(int value)
    {
        promptedCorruptionIndex = value;
    }

    [SerializeField]
    private float promptedCorruption;

    public float GetPromptedCorruption()
    {
        return promptedCorruption;
    }

    public void SetPromptedCorruption(float value)
    {
        promptedCorruption = value;
    }

    [Header("UI")]
    [SerializeField]
    private GameObject canvas;

    [SerializeField]
    private Material bar;

    [SerializeField]
    private int selectedAreaIndex;

    public int GetAreaIndex()
    {
        return selectedAreaIndex;
    }

    public void SetAreaIndex(int value)
    {
        selectedAreaIndex = value;
    }

    public float GetCorruption()
    {
        return corruptionList[selectedAreaIndex];
    }

    public void SetCorruption(float value)
    {
        corruptionList[selectedAreaIndex] = value;
    }

    public float GetCorruptionByIndex(int index)
    {
        return corruptionList[index];
    }

    public void SetCorruptionByIndex(int index, float value)
    {
        corruptionList[index] = value;
    }

    private void Awake()
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
                canvas.SetActive(true);
                break;
            case 1:
            case 2:
            case 3:
                canvas.SetActive(false);
                break;
        }
    }

    void LateUpdate()
    {
        bar.SetFloat("_Fill", corruptionList[selectedAreaIndex]);
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
