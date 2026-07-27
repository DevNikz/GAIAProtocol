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
    private float corruptionFloat = 0.0f;

    [Header("UI")]
    [SerializeField]
    private GameObject canvas;

    [SerializeField]
    private Material bar;

    public void SetCorruption(float value)
    {
        corruptionFloat = value;
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
        //bar.SetFloat("_Fill", corruptionList[selectedAreaIndex]);
        bar.SetFloat("_Fill", corruptionFloat);
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
