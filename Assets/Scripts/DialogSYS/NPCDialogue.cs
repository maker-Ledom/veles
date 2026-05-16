using UnityEngine;
using UnityEngine.Events;

public class NPCDialogue : MonoBehaviour
{
    [SerializeField] private DialogueData dialogue;
    [SerializeField] private Sprite npcIcon;
    public UnityEvent afterDialogueAction; // Сюда в инспекторе перетащи метод OpenPanel из StatsUpgrader или Shop

    public void Interact()
    {
        DialogueManager.Instance.StartDialogue(dialogue, npcIcon, afterDialogueAction);
    }
}