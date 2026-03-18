using UnityEngine;

public class ChangeLevelButton : MonoBehaviour
{
    public SoundController soundController;
    public int sceneIndex;

    public void ChangeLevelIndex()
    {
        //soundController.PlaySound(1);
        LevelManager.Instance.LoadLevelIndex(sceneIndex);
    }

    public void PlaySound()
    {
        //soundController.PlaySound(1);
    }
}