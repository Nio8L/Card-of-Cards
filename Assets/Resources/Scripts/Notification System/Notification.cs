using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Notification")]
public class Notification : ScriptableObject
{
    [Multiline]
    public List<string> lines;
}
