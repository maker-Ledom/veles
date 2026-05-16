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

    private DialogueData _currentDialogue;
    private int _currentIndex;
    private UnityEvent _onDialogueEnd;

    private void Awake()
    {
        Instance = this;
        dialoguePanel.SetActive(false);
    }

    public void StartDialogue(DialogueData data, Sprite npcSprite, UnityEvent onEndAction)
    {
        _currentDialogue = data;
        _onDialogueEnd = onEndAction;
        _currentIndex = 0;
        
        npcPortrait.sprite = npcSprite;
        dialoguePanel.SetActive(true);
        
    // СТАВИМ НА ПАУЗУ
        Time.timeScale = 0f; 
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.isAnyPanelOpen = true;
            // Если в твоем GameManager есть метод управления курсором, вызови его здесь
            // Например, если он делает Cursor.visible = true;
        }
        DisplayLine();
    }

    public void OnNextButtonClick()
    {
        _currentIndex++;
        if (_currentIndex < _currentDialogue.lines.Length)
        {
            DisplayLine();
        }
        else
        {
            EndDialogue();
        }
    }

    private void DisplayLine()
    {
        var line = _currentDialogue.lines[_currentIndex];

        if (line.speaker == Speaker.Hero)
        {
            heroTextField.text = line.text;
            npcTextField.text = ""; // Скрываем текст NPC
            
            heroPortrait.color = activeColor;
            npcPortrait.color = inactiveColor;
            heroPortrait.transform.localScale = Vector3.one * 1.1f; // Легкое увеличение
            npcPortrait.transform.localScale = Vector3.one;
        }
        else
        {
            npcTextField.text = line.text;
            heroTextField.text = ""; // Скрываем текст героя
            
            npcPortrait.color = activeColor;
            heroPortrait.color = inactiveColor;
            npcPortrait.transform.localScale = Vector3.one * 1.1f;
            heroPortrait.transform.localScale = Vector3.one;
        }
    }

    private void EndDialogue()
    {
        // МЫ НЕ СНИМАЕМ ПАУЗУ СРАЗУ, если открывается магазин
        // Мы просто закрываем панель диалога
        dialoguePanel.SetActive(false);

        if (_onDialogueEnd != null && _onDialogueEnd.GetPersistentEventCount() > 0)
        {
            // Вызываем открытие магазина/прокачки
            _onDialogueEnd.Invoke(); 
        }
        else
        {
            // Если после диалога ничего не открывается — возвращаем время в норму
            ResumeGame();
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.isAnyPanelOpen = false;
            GameManager.Instance.ResumeGameplayState(); 
        }
    }
}