using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISettingsPersistence
{
    void LoadData(SettingsData data);
    void SaveData(ref SettingsData data);
}
