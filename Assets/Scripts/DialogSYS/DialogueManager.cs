using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI heroTextField;
    [SerializeField] private TextMeshProUGUI npcTextField;
    [SerializeField] private Image heroPortrait;
    [SerializeField] private Image npcPortrait;

    [Header("Settings")]
    [SerializeField] private Color activeColor = Color.white;
    [SerializeField] private Color inactiveColor = new Color(0.5f, 0.5f, 0.5f, 1f);

    private DialogueData currentDialogue;
    private int currentIndex;
    private UnityEvent onDialogueEnd;

    private void Awake()
    {
        Instance = this;
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    public void StartDialogue(DialogueData data, Sprite npcSprite, UnityEvent endAction)
    {
        if (data == null || data.lines.Length == 0) return;

        currentDialogue = data;
        onDialogueEnd = endAction;
        currentIndex = 0;

        if (npcPortrait != null)
            npcPortrait.sprite = npcSprite;

        if (GameManager.Instance != null)
            GameManager.Instance.OpenPanel(dialoguePanel);

        DisplayCurrentLine();
    }

    public void OnNextButtonClick()
    {
        if (currentDialogue == null || currentDialogue.lines == null)
        {
            Debug.LogWarning("Dialogue already ended");
            EndDialogue();
            return;
        }

        currentIndex++;
        if (currentIndex < currentDialogue.lines.Length)
        {
            DisplayCurrentLine();
        }
        else
        {
            EndDialogue();
        }
    }

    private void DisplayCurrentLine()
    {
        if (currentDialogue == null || currentDialogue.lines.Length <= currentIndex)
        {
            EndDialogue();
            return;
        }

        DialogueLine line = currentDialogue.lines[currentIndex];

        if (line.speaker == Speaker.Hero)
        {
            if (heroTextField != null)
                heroTextField.text = line.text;
            if (npcTextField != null)
                npcTextField.text = "";

            if (heroPortrait != null)
            {
                heroPortrait.color = activeColor;
                heroPortrait.transform.localScale = Vector3.one * 1.1f;
            }
            if (npcPortrait != null)
            {
                npcPortrait.color = inactiveColor;
                npcPortrait.transform.localScale = Vector3.one;
            }
        }
        else
        {
            if (npcTextField != null)
                npcTextField.text = line.text;
            if (heroTextField != null)
                heroTextField.text = "";

            if (npcPortrait != null)
            {
                npcPortrait.color = activeColor;
                npcPortrait.transform.localScale = Vector3.one * 1.1f;
            }
            if (heroPortrait != null)
            {
                heroPortrait.color = inactiveColor;
                heroPortrait.transform.localScale = Vector3.one;
            }
        }
    }

    private void EndDialogue()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.CloseTopPanel();
        else
            dialoguePanel.SetActive(false);

        if (onDialogueEnd != null && onDialogueEnd.GetPersistentEventCount() > 0)
            onDialogueEnd.Invoke();

        currentDialogue = null;
        onDialogueEnd = null;
    }
}