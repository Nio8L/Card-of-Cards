using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SettingsData
{
    public int audioLevel;
    public int musicLevel;
    public bool autoSave;

    public SettingsData(){
        audioLevel = 100;
        musicLevel = 100;
        autoSave = true;
    }
}
