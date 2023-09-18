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
        audioLevel = 0;
        musicLevel = 0;
        autoSave = false;
    }
}
