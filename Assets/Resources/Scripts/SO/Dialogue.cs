using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[CreateAssetMenu(menuName = "Dialogue")]
public class Dialogue : ScriptableObject
{
    [HideInInspector]
    public TextMeshProUGUI textBox;

    public List<string> dialogueLines = new List<string>();
    [HideInInspector]
    public int line = -1;

    public void Initialize()
    {
        textBox = GameObject.Find("DialogueTextBox").GetComponent<TextMeshProUGUI>();
        line = -1;
    }

    public void StartDialogue()
    {   
        textBox.transform.parent.gameObject.SetActive(true);
        NextLine();
    }

    public void EndDialogue()
    {
        textBox.transform.parent.gameObject.SetActive(false);
    }

    public void NextLine()
    {
        line++;
        if (line >= dialogueLines.Count || dialogueLines[line] == "")
        {
            textBox.text = "";
            EndDialogue();
        }
        else textBox.text = dialogueLines[line];
    }
}
