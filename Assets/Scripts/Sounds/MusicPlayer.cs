using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private SoundController soundController;

    void Start()
    {
        soundController.PlaySound();
    }
}