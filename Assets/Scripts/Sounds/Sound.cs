using System;
using UnityEngine;

[Serializable]
public class Sound {
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 0.5f;
    [Range(-3f, 3f)] public float pitch = 1f;
    public bool loop;
}