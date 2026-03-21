using UnityEngine;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    [SerializeField] AudioMixer master;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
        
    }

    public void OpenSFX()
    {
        SoundManager.Instance.PlaySFX("Select1");
    }
    public void CloseSFX()
    {
        SoundManager.Instance.PlaySFX("Deselect");
    }

    public void AdjustMasterVolume(float volume)
    {
        master.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
    }

    public void AdjustMusicVolume(float volume)
    {
        master.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
    }

    public void AdjustSFXVolume(float volume)
    {
        master.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
    }

    public void AdjustUIVolume(float volume)
    {
        master.SetFloat("UIVolume", Mathf.Log10(volume) * 20);
    }

}
