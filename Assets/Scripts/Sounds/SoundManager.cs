using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public struct SoundList
{
    [HideInInspector] public string name;
    [Range(0, 1)] public float volume;
    public AudioMixerGroup mixer;
    public AudioClip[] sounds;
    
}

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    [SerializeField] private SoundsSO SO;
    private static SoundManager Instance;
    private AudioSource audioSource;
    public List<AudioSource> audioSourcesTemp;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            audioSource = GetComponent<AudioSource>();
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public static void PlaySound(SoundType sound, AudioSource source = null, float volume = 1, int index = 0)
    {
        SoundList soundList = Instance.SO.sounds[(int)sound];
        AudioClip[] clips = soundList.sounds;
        //AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];
        AudioClip clip = clips[index];

        if(source)
        {
            source.outputAudioMixerGroup = soundList.mixer;
            source.clip = clip;
            source.volume = volume * soundList.volume;

            Instance.audioSourcesTemp.Add(source);

            source.Play();
        }
        else
        {
            Instance.audioSource.outputAudioMixerGroup = soundList.mixer;
            Instance.audioSource.PlayOneShot(clip, volume * soundList.volume);
        }
    }

    public static void StopAllSounds()
    {
        for(int i = 0; i < Instance.audioSourcesTemp.Count; i++)
        {
            Instance.audioSourcesTemp[i].Stop();
        }
        Instance.audioSourcesTemp.Clear();

    }
}