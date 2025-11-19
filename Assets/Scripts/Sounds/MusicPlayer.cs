using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private SoundController soundController;
    [SerializeField] private int index;
    void Start()
    {
        soundController.PlaySound(index);
    }
}