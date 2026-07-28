using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum DialogueType
{
    TUTORIAL_HUB,
    FOREST1,
    FOREST2,
    FOREST3,
    FOREST4,
    ARMORY,
    MECH_DEPLOYMENT,
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("Dialgoues")]
    [SerializeField]
    Dialogue TutorialMenu;

    [SerializeField]
    Dialogue Forest1,
        Forest2,
        Forest3,
        Forest4;

    [SerializeField]
    Dialogue Armory,
        Deployment;

    [Header("References")]
    [SerializeField]
    GameObject canvas;

    public void HideCanvas()
    {
        canvas.SetActive(false);
    }

    public void ShowCanvas()
    {
        canvas.SetActive(true);
    }

    [SerializeField]
    RectTransform box;

    [SerializeField]
    TextMeshProUGUI charName;

    [SerializeField]
    TextMeshProUGUI dialogueArea;
    Queue<DialogueLine> lines;

    [Header("Properties")]
    [SerializeField]
    bool isDialogueActive = false;

    [SerializeField]
    float typingSpeed = 0.2f;

    [SerializeField]
    bool isTyping = false;

    [SerializeField]
    TweenSettings<float> show;

    [SerializeField]
    TweenSettings<float> hide;

    public void AnimateShow()
    {
        Tween.UIAnchoredPositionY(box, show);
    }

    public void AnimateHide()
    {
        Tween.UIAnchoredPositionY(box, hide).OnComplete(HideCanvas);
    }

    DialogueLine currentLine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        lines = new Queue<DialogueLine>();

        canvas = transform.Find("Canvas").gameObject;
        box = transform.Find("Canvas/Panel/DialogueBox").GetComponent<RectTransform>();

        box.GetComponent<Button>()
            .onClick.AddListener(() =>
            {
                //NextLine
                DisplayNextDialogueLine();
                SoundManager.Instance.PlaySFX("Select Planet");
            });

        charName = transform
            .Find("Canvas/Panel/DialogueBox/Header/Text")
            .GetComponent<TextMeshProUGUI>();
        dialogueArea = transform
            .Find("Canvas/Panel/DialogueBox/Body/Text")
            .GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (isDialogueActive)
        {
            if (InputManager.Instance.GetSpaceButton())
            {
                ButtonClick();
            }
        }
    }

    public void ButtonClick()
    {
        box.GetComponent<Button>().onClick.Invoke();
    }

    public void TestKeyInput()
    {
        if (InputManager.Instance.GetSpaceButton())
        {
            DisplayNextDialogueLine();
            SoundManager.Instance.PlaySFX("Select Planet");
        }
    }

    public void StartDialogue(DialogueType type)
    {
        InputManager.Instance.DisableMechRotate();
        InputManager.Instance.DisableDebug();
        InputManager.Instance.DisableLevelCamera();
        InputManager.Instance.DisableLegacyInputs();
        Dialogue dialogue;
        switch (type)
        {
            case DialogueType.TUTORIAL_HUB:
                dialogue = TutorialMenu;
                break;
            case DialogueType.FOREST1:
                dialogue = Forest1;
                break;
            case DialogueType.FOREST2:
                dialogue = Forest2;
                break;
            case DialogueType.FOREST3:
                dialogue = Forest3;
                break;
            case DialogueType.FOREST4:
                dialogue = Forest4;
                break;
            case DialogueType.ARMORY:
                dialogue = Armory;
                break;
            case DialogueType.MECH_DEPLOYMENT:
                dialogue = Deployment;
                break;
            default:
                dialogue = null;
                break;
        }

        isDialogueActive = true;

        AnimateShow();

        lines.Clear();

        foreach (DialogueLine dialogueLine in dialogue.dialogueLines)
        {
            lines.Enqueue(dialogueLine);
        }

        DisplayNextDialogueLine();
    }

    public void DisplayNextDialogueLine()
    {
        if (lines.Count == 0 && !isTyping)
        {
            EndDialogue();
            return;
        }

        if (isTyping)
        {
            StopAllCoroutines();
            dialogueArea.text = currentLine.line;
            isTyping = false;
            return;
        }

        currentLine = lines.Dequeue();

        //characterIcon.sprite = currentLine.character.icon;
        charName.text = currentLine.character.name;

        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentLine));
    }

    IEnumerator TypeSentence(DialogueLine dialogueLine)
    {
        isTyping = true;
        dialogueArea.text = "";
        string fullText = dialogueLine.line;
        string visibleText = "";
        int i = 0;

        while (i < fullText.Length)
        {
            if (fullText[i] == '<')
            {
                // Find the closing '>' and append the whole tag instantly
                int closeIndex = fullText.IndexOf('>', i);
                if (closeIndex != -1)
                {
                    visibleText += fullText.Substring(i, closeIndex - i + 1);
                    dialogueArea.text = visibleText;
                    i = closeIndex + 1;
                    continue; // No delay for tags
                }
            }

            visibleText += fullText[i];
            dialogueArea.text = visibleText;
            i++;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }

    void EndDialogue()
    {
        InputManager.Instance.EnableMechRotate();
        InputManager.Instance.EnableDebug();
        InputManager.Instance.EnableLevelCamera();
        InputManager.Instance.EnableLegacyInputs();
        isDialogueActive = false;
        AnimateHide();
        //HideCanvas();
    }
}
