using UnityEngine;

public class ChangeLevelButton : MonoBehaviour
{
    public void ChangeLevel(string sceneName)
    {
        LevelManager.Instance.LoadScene(sceneName);
    }
}