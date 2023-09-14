using UnityEngine.Audio;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SoundClass
{
    public string name;
    public List<AudioClip> clip;

    [Range(0f, 1f)]
    public float volume;
    [Range(0.1f, 3f)]
    public float pitch;

    public bool playBetweenScenes = false;

    [HideInInspector]
    public AudioSource source;
}
