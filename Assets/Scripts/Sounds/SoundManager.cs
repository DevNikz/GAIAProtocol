using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;   

    [Header("References")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource voiceSource;
    [SerializeField] private AudioSource uiSource;
    [SerializeField] private List<AudioSource> customMusicList;
    [SerializeField] private AudioMixer masterMixer;

    [Header("Sounds")]
    [SerializeField] public Sound[] sounds;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        sfxSource = GetComponentsInChildren<AudioSource>()[0]; //first in hierarchy
        musicSource = GetComponentsInChildren<AudioSource>()[1];
        voiceSource = GetComponentsInChildren<AudioSource>()[2];
        uiSource = GetComponentsInChildren<AudioSource>()[3];
        //SwitchAudio();
    }
    
    public void StopCustomMusic()
    {
        for(int i = 0; i < customMusicList.Count; i++)
        {
            customMusicList[i].Stop();
        }
    }

    public void PlayCustomMusic(int index)
    {
        
        customMusicList[index].Play();
    }

    public void PlayMusic(string name)
    {
        StopMusic();
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null || s.clip == null || musicSource == null) return;

        musicSource.clip = s.clip;
        musicSource.loop = s.loop;
        musicSource.volume = s.volume;
        musicSource.Play();
    }

    public void PauseMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
            musicSource.Pause();
    }

    public void ResumeMusic()
    {
        if (musicSource != null)
            musicSource.UnPause();
    }

    public void StopMusic() //Call this first before play music
    {
        if (musicSource != null && musicSource.isPlaying)
            musicSource.Stop();
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null || s.clip == null || sfxSource == null) return;    

        sfxSource.PlayOneShot(s.clip, s.volume);
    }

    public void StopSFX()
    {
        if(sfxSource != null && sfxSource.isPlaying) sfxSource.Stop();
    }

    public void PlayUI(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null || s.clip == null || uiSource == null) return;    

        uiSource.PlayOneShot(s.clip, s.volume);
    }

    public void StopUI()
    {
        if(uiSource != null && uiSource.isPlaying) uiSource.Stop();
    }

    public void PlaySFXAtPosition(string name, Vector3 position)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null || s.clip == null) return;

        GameObject tempGO = new GameObject("TempSFX3D_" + s.name);
        tempGO.transform.position = position;

        AudioSource tempSource = tempGO.AddComponent<AudioSource>();
        tempSource.clip = s.clip;
        tempSource.volume = s.volume;
        tempSource.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
        tempSource.outputAudioMixerGroup = sfxSource.outputAudioMixerGroup;
        tempSource.spatialBlend = 1.0f;
        tempSource.minDistance = 1.0f;  
        tempSource.maxDistance = 90.0f;
        tempSource.rolloffMode = AudioRolloffMode.Linear;

        tempSource.Play();

        Destroy(tempGO, s.clip.length / tempSource.pitch);
    }

    public void PlayVoice(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null || s.clip == null || voiceSource == null) return;  

        //voiceSource.PlayOneShot(s.clip, s.volume);
        voiceSource.clip = s.clip;
        voiceSource.loop = s.loop;
        voiceSource.volume = s.volume;  
        voiceSource.Play();
    }

    public void StopVoice()
    {
        if(voiceSource != null && voiceSource.isPlaying) voiceSource.Stop();
    }

    public void PauseVoice()
    {
        if(voiceSource != null && voiceSource.isPlaying) voiceSource.Pause();
    }

    public void ResumeVoice()
    {
        if(voiceSource != null && !voiceSource.isPlaying) voiceSource.UnPause();
    }
}