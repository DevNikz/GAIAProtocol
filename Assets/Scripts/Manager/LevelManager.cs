using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    [SerializeField] private GameObject _loaderCanvas; //UI Loading
    //[SerializeField] private GameObject _screenTransition;
    [SerializeField] private Animator transitionAnim;
    [SerializeField] private Image _progressBar;
    private float _target;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void LoadLevel(string sceneName)
    {
        StartCoroutine(LoadAsync(sceneName));
    }

    public void LoadLevelIndex(int sceneIndex)
    {
        StartCoroutine(LoadAsyncIndex(sceneIndex));
    }

    IEnumerator LoadAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        _loaderCanvas.SetActive(true);

        while (!operation.isDone)
        {
            _target = Mathf.Clamp01(operation.progress / .9f);
            yield return null;
        }

        _loaderCanvas.SetActive(false);
    }

    IEnumerator LoadAsyncIndex(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        _loaderCanvas.SetActive(true);

        while (!operation.isDone)
        {
            _target = Mathf.Clamp01(operation.progress / .9f);
            yield return null;
        }

        _loaderCanvas.SetActive(false);
    }
    
    void LateUpdate()
    {
        _progressBar.fillAmount = Mathf.MoveTowards(_progressBar.fillAmount, _target, 10 * Time.deltaTime);
    }
}
 