using UnityEngine;

public class ChangeLevelButton : MonoBehaviour
{
    public void ChangeLevel(string sceneName)
    {
        LevelManager.Instance.LoadLevel(sceneName);
    }
    public void ChangeScene(string sceneName)
    {
        LevelManager.Instance.LoadScene(sceneName);
    }
}