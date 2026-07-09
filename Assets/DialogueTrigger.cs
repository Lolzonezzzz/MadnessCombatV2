using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class DialogueChar
{
    public string name;
    public Sprite icon;
}

[System.Serializable]
public class DialogueLine
{
    public DialogueChar character;
    
    [TextArea(3, 10)]
    public string text;
}

[System.Serializable]
public class DialogueEvent
{
    public List<DialogueLine> lines = new List<DialogueLine>();
}
public class DialogueTrigger : MonoBehaviour
{
    public DialogueEvent dialogue;

    public void TriggerDialogue()
    {
        DialogueManager.Instance.StartDialogue(dialogue);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            TriggerDialogue();
        }
    }
}
