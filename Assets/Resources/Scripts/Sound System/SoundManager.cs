using UnityEngine.Audio;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

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
            sound.source.clip = sound.clip[0];
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
        }
    }

    private void Start() {
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    public void Play(string name){
        SoundClass sound = Array.Find(sounds, sound => sound.name == name);
        if(sound.source != null){
            sound.source.PlayOneShot(sound.source.clip, sound.source.volume);
        }
    }

    public void Play(string name, int index){
        SoundClass sound = Array.Find(sounds, sound => sound.name == name);
        if(sound.clip[index] != null){
            sound.source.clip = sound.clip[index];
            sound.source.PlayOneShot(sound.source.clip, sound.source.volume);
        }
    }

    public void UpdateVolume(float volume){
        foreach(SoundClass sound in sounds){
            sound.source.volume = volume;
        }
    }

    public void OnSceneUnloaded(Scene scene){
        foreach (SoundClass sound in sounds)
        {   
            if(!sound.playBetweenScenes){
                sound.source.Stop();
            }
        }
    }
}
