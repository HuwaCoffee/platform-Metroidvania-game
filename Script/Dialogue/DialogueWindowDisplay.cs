
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

[DefaultExecutionOrder(100)]
public class DialogueWindowDisplay : MonoBehaviour
{
    public static DialogueWindowDisplay Instance;

    [Header("对话窗口配置")]
    public GameObject dialogBox;
    public Text       dialogBoxText;
    public float      textSpeed = 0.05f;

    [Header("长段对话音效（一次整段播放）")]
    public AudioClip  dialogueAudio;   // Inspector 拖入你的“长音频”

    private AudioSource audioSource;   // 用来播放 dialogueAudio

    private string[] lines;
    private int      index;
    private bool     isTyping;
    private Coroutine typingCoroutine;
    private Action   onDialogueComplete;
    private bool     ignoreNextE = false;

    /// <summary>
    /// 对话框当前是否激活中（正在展示或打字中）
    /// </summary>
    public bool DialogBoxActive { get; private set; }

    void Awake()
    {
        // 单例初始化
        if (Instance == null) Instance = this;
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 确保有 AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop        = false;

        dialogBox.SetActive(false);
        DialogBoxActive = false;
    }

    void Update()
    {
        if (DialogBoxActive && Input.GetKeyDown(KeyCode.E))
        {
            if (ignoreNextE)
            {
                ignoreNextE = false;
                return;
            }

            if (isTyping)
            {
                // 立即完成当前行
                StopCoroutine(typingCoroutine);
                dialogBoxText.text = lines[index];
                isTyping = false;
                 // 对话完毕：先关闭音频，再关闭面板
                if (audioSource.isPlaying)
                    audioSource.Stop();
            }
            else
            {
                NextLine();
            }
        }
    }

    /// <summary>
    /// 外部调用：显示一组对话，并在播完后执行 onComplete 回调
    /// </summary>
    public void ShowDialogue(string[] dialogueLines, Action onComplete)
    {
        if (dialogueLines == null || dialogueLines.Length == 0)
        {
            onComplete?.Invoke();
            return;
        }

        

        ignoreNextE        = true;
        lines              = dialogueLines;
        onDialogueComplete = onComplete;
        index              = 0;
        DialogBoxActive    = true;
        dialogBox.SetActive(true);

        typingCoroutine = StartCoroutine(TypeLine());
    }

    private IEnumerator TypeLine()
    {
        isTyping = true;
        dialogBoxText.text = "";

        foreach (char c in lines[index])
        {
            dialogBoxText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

         // 对话完毕：先关闭音频，再关闭面板
            if (audioSource.isPlaying)
                audioSource.Stop();

        isTyping = false;
    }

    private void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            // —— 播放“长段音频” —— 
        if (dialogueAudio != null)
            audioSource.PlayOneShot(dialogueAudio);
            typingCoroutine = StartCoroutine(TypeLine());
        }
        else
        {
           

            dialogBox.SetActive(false);
            DialogBoxActive = false;
            onDialogueComplete?.Invoke();
        }
    }
}

