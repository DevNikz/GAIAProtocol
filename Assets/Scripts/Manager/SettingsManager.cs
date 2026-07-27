using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    [SerializeField]
    AudioMixer master;

    [SerializeField]
    Slider masterSlider;

    [SerializeField]
    Slider musicSlider;

    [SerializeField]
    Slider sfxSlider;

    [SerializeField]
    Slider uiSlider;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    void OnEnable()
    {
        masterSlider.SetValueWithoutNotify(GetMasterVolume());
        musicSlider.SetValueWithoutNotify(GetMusicVolume());
        sfxSlider.SetValueWithoutNotify(GetSFXVolume());
        uiSlider.SetValueWithoutNotify(GetUIVolume());
    }

    public float GetMasterVolume()
    {
        if (master.GetFloat("MasterVolume", out float dB))
            return Mathf.Pow(10f, dB / 20f);
        return 1f; // fallback default
    }

    public float GetMusicVolume()
    {
        if (master.GetFloat("MusicVolume", out float dB))
            return Mathf.Pow(10f, dB / 20f);
        return 1f;
    }

    public float GetSFXVolume()
    {
        if (master.GetFloat("SFXVolume", out float dB))
            return Mathf.Pow(10f, dB / 20f);
        return 1f;
    }

    public float GetUIVolume()
    {
        if (master.GetFloat("UIVolume", out float dB))
            return Mathf.Pow(10f, dB / 20f);
        return 1f;
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
