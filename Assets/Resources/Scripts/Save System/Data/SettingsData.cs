using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SettingsData
{
    public int audioLevel;
    public int musicLevel;
    public bool autoSave;

    public bool disableAudio;
    public bool disableMusic;

    public SettingsData(){
        audioLevel = 100;
        musicLevel = 100;
        
        disableAudio = false;
        disableMusic = false;
        
        autoSave = true;
    }
}
