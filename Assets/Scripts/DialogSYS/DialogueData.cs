using UnityEngine;

[System.Serializable]
public enum Speaker { Hero, NPC }

[System.Serializable]
public struct DialogueLine
{
    public Speaker speaker;
    [TextArea(3, 10)]
    public string text;
}

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    public DialogueLine[] lines;
}