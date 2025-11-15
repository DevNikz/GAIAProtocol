using UnityEngine;

public class ChangeLevelButton : MonoBehaviour
{

    public string sceneName;
    public void ChangeLevel()
    {
        LevelManager.Instance.LoadLevel(sceneName);
    }
}