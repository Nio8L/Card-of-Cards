using UnityEngine.Audio;
using UnityEngine;
using System;

public class SoundManager : MonoBehaviour
{
    public SoundClass[] sounds;

    public static SoundManager soundManager;

    private void Awake() {
        if(soundManager == null){
            soundManager = this;
        }else{
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach(SoundClass sound in sounds){
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
        }
    }

    public void Play(string name){
        SoundClass sound = Array.Find(sounds, sound => sound.name == name);
        sound.source.Play();
    }
}
