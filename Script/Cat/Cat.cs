using System.Collections;
using UnityEngine;


public class Cat : MonoBehaviour
{
    [Header("对话配置 (SO)")]
    public DialogueData dialogueAsset;      // 在 Inspector 里拖入 SO，里面应有 3 条节点

    [Header("玩家 & 交互设置")]
    public Transform playerTransform;
    public float interactRadius = 2f;

    private DialogueWindowDisplay dialogueUI;
    private int currentNodeIndex = 0;

    [Header("静止玩家的参数")]
    public PlayerController playerController;
    public PlayerAttack     playerAttack;
    public PlayerMagic      playerMagic;

    [Header("E键提示设置")]
    public SpriteRenderer eKeySprite; // 拖入E键提示的SpriteRenderer组件
    public float fadeDuration = 0.5f; // 淡入淡出持续时间
    public Vector3 iconOffset = new Vector3(0, 1.5f, 0); // 图标相对NPC的偏移量（头顶位置）
    private Coroutine fadeCoroutine; // 淡入淡出协程引用
    private float targetAlpha; // 目标透明度
    void Start()
    {
        dialogueUI = DialogueWindowDisplay.Instance;
        if (dialogueUI == null)
            Debug.LogError("[Cat] 找不到 DialogueWindowDisplay 单例！");


             dialogueUI = DialogueWindowDisplay.Instance;
        if (dialogueUI == null)
            Debug.LogError("[Cat] 找不到 DialogueWindowDisplay 单例！");
        
        // 初始化E键提示为完全透明
        if (eKeySprite != null)
        {
            eKeySprite.color = new Color(eKeySprite.color.r, eKeySprite.color.g, eKeySprite.color.b, 0f);
        }
    }

    void Update()
    {
// 更新E键图标位置（始终位于NPC头顶）
        if (eKeySprite != null)
        {
            eKeySprite.transform.position = transform.position + iconOffset;
            // 可选：始终面向玩家（2D情况可省略）
            //eKeySprite.transform.LookAt(playerTransform);
        }

        bool isPlayerInRange = IsPlayerNear() && !dialogueUI.DialogBoxActive;
        
        // 设置目标透明度
        targetAlpha = isPlayerInRange ? 1f : 0f;

        // 检测透明度变化需求
        if (Mathf.Abs(eKeySprite.color.a - targetAlpha) > 0.01f)
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeEKey());
        }


        // 玩家在半径内，按 E 触发对话，且当前没有对话窗口活跃
        if (Input.GetKeyDown(KeyCode.E)
            && IsPlayerNear()
            && !dialogueUI.DialogBoxActive)
        {
           
            StartCoroutine(PlayCurrentNode());
        }
    }
 // E键淡入淡出协程
    // 淡入淡出协程（直接操作SpriteRenderer的颜色）
    private IEnumerator FadeEKey()
    {
        Color currentColor = eKeySprite.color;
        float startAlpha = currentColor.a;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);
            currentColor.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            eKeySprite.color = currentColor;
            yield return null;
        }

        // 确保最终值准确
        currentColor.a = targetAlpha;
        eKeySprite.color = currentColor;
        fadeCoroutine = null;
    }
    void DisablePlayerControl()
    {
         // 清零速度
        var rb = playerController.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        // 禁用
        playerController.enabled = false;
        playerAttack.enabled     = false;
        playerMagic.enabled      = false;

        // 强制 Idle 动画
        var anim = playerController.GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetBool("isRun", false);
            anim.SetBool("isIdle", true);
        }
    }
  
 void EnablePlayerControl()
    {
        playerController.enabled = true;
        playerAttack.enabled     = true;
        playerMagic.enabled      = true;
    }
    bool IsPlayerNear()
    {
        return (playerTransform.position - transform.position).sqrMagnitude
               <= interactRadius * interactRadius;
    }

    IEnumerator PlayCurrentNode()
    {
         // 1. 停止移动并禁用玩家控制
            DisablePlayerControl();

        // 播放对话
        int idx = Mathf.Clamp(currentNodeIndex, 0, dialogueAsset.dialogueNodes.Length - 1);
        var lines = dialogueAsset.dialogueNodes[idx].dialogueLines;
        bool finished = false;
        dialogueUI.ShowDialogue(lines, () => finished = true);

        // 等待对话完
        yield return new WaitUntil(() => finished);

        // 解锁玩家
        EnablePlayerControl();

        // 推进索引，但不要越过最后一条
        currentNodeIndex = Mathf.Min(
            currentNodeIndex + 1,
            dialogueAsset.dialogueNodes.Length - 1
        );
    }

    // （可选）在 Scene 里画出交互范围
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}
