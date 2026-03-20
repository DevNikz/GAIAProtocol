using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    [SerializeField] private GameObject _loaderCanvas; //UI Loading
    [SerializeField] private Image _progressBar;
    private float _target;

    [SerializeField] private int currentLevel;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }
    public void LoadLevelIndex(int sceneIndex)
    {
        //SoundManager.StopAllSounds();
        StartCoroutine(LoadAsyncIndex(sceneIndex));
    }

    IEnumerator LoadAsyncIndex(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        //FadeScreenManager.Instance.HideCanvas();
        //_loaderCanvas.SetActive(true);

        while (!operation.isDone)
        {
            _target = Mathf.Clamp01(operation.progress / .9f);
            yield return null;
        }

        //FadeScreenManager.Instance.ShowCanvas();
        //_loaderCanvas.SetActive(false);
        
    }
    
    void LateUpdate()
    {
        _progressBar.fillAmount = Mathf.MoveTowards(_progressBar.fillAmount, _target, 10 * Time.deltaTime);
    }

    public void SetCurrentLevel(int value) 
    {
        currentLevel = value;
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }
}
 