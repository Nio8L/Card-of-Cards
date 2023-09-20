using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[CreateAssetMenu(menuName = "Dialogue")]
public class Dialogue : ScriptableObject
{
    [HideInInspector]
    public TextMeshProUGUI textBox;
    GameObject clickIcon;
    [TextArea()]
    public List<string> dialogueLines = new List<string>();
    [HideInInspector]
    public int line = -1;
    public bool NextLineAtStartOfTurn;
    public bool NextLineAtClick;
    public bool StartOnLoad;

    public void Initialize()
    {
        textBox = GameObject.Find("DialogueTextBox").GetComponent<TextMeshProUGUI>();
        clickIcon = textBox.transform.parent.GetChild(0).gameObject;
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
    public void NextLine(int lines)
    {
        for (int i = 0; i < lines; i++)
        {
            NextLine();
        }
    }

    public void GoBack()
    {
        line--;
        if (line >= dialogueLines.Count || dialogueLines[line] == "")
        {
            textBox.text = "";
            EndDialogue();
        }
        else textBox.text = dialogueLines[line];
    }

    public void UpdateClickRule(bool onClick)
    {
        NextLineAtClick = onClick;
        clickIcon.SetActive(onClick);
    }
}
