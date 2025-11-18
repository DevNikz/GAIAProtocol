using UnityEngine;

public class ChangeLevelButton : MonoBehaviour
{

    public string sceneName;
    public int sceneIndex;
    public void ChangeLevel()
    {
        LevelManager.Instance.LoadLevel(sceneName);
    }

    public void ChangeLevelIndex()
    {
        LevelManager.Instance.LoadLevelIndex(sceneIndex);
    }
}