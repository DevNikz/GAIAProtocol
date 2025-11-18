using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField] private SoundType sound;
    [SerializeField, Range(0, 1)] private float volume = 1;
    [SerializeField] private AudioSource source;
    public void PlaySound(int index = 0)
    {
        SoundManager.PlaySound(sound, source, volume, index);
    }
}
