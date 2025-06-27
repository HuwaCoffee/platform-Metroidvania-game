using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class Merchant : MonoBehaviour
{
    [Header("对话配置 (SO)")]
    public DialogueData dialogueAsset;      // 在 Inspector 里拖入 NPCDialogueSO

    [Header("玩家 & 交互设置")]
    public Transform playerTransform;
    public float interactRadius = 2f;

    [Header("商店 UI")]
    public GameObject shopUI;
    public ShopItem[] items;
    public Transform itemListParent;
    public GameObject itemPrefab;

     
   

    [Header("商品")]
    public PlayerController playerController;
    public PlayerAttack     playerAttack;
    public PlayerMagic      playerMagic;
    public PlayerHealth playerHealth;

    private DialogueWindowDisplay dialogueUI;
    private int currentNodeIndex = 0;

     private int selectedIndex=0;
     
     [Header("长段对话音效（一次整段播放）")]
    public AudioClip  dialogueAudio;   // Inspector 拖入你的“长音频”

    private AudioSource audioSource;   // 用来播放 dialogueAudio

    [HideInInspector] public bool isShopOpen=false;  //给暂停页面用的
    
    private DialogueWindowDisplay dwd; //给对话框脚本

    void Start()
    {
         // 确保有 AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop        = false;
        
        dwd =GetComponent<DialogueWindowDisplay>();
        dialogueUI = DialogueWindowDisplay.Instance;
        if (dialogueUI == null)
            Debug.LogError("[Merchant] 找不到 DialogueWindowDisplay 单例！");
    }

    void Update()
    {  
        Debug.Log($"[Merchant] Before E: currentNode={currentNodeIndex}, " +
          $"require={dialogueAsset.dialogueNodes[3].requireQuestComplete}, " +
          $"CheckQuestComplete=> {GameProgressManager.Instance.CheckQuestComplete( dialogueAsset.dialogueNodes[3].requiredQuest)}");

        // 如果商店面板开启，处理购买/关闭逻辑...
        if (isShopOpen)
        {
            HandleNavigation();
            HandlePurchase();
            // 按 ESC 关闭商店（不再需要距离限制）
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleShop();

            }
            return;
        }
        
        //玩家在附近按E，且商店没有打开，文本框没有打开时，才有效果
        if (Input.GetKeyDown(KeyCode.E) && IsPlayerNear() && !dialogueUI.DialogBoxActive ) //像这种isShopOpen都可以采用shopUI.activeSelf替换
        {
            // 如果条件达成，直接跳到节点 3
            if (currentNodeIndex < 3 && GameProgressManager.Instance.CheckQuestComplete(
                    dialogueAsset.dialogueNodes[3].requiredQuest))
            {
                currentNodeIndex = 3;  
                
        }
     
            // 1. 停止移动并禁用玩家控制
            DisablePlayerControl();
            // 2. 播放当前对话节点  
            PlayDialogueNode(currentNodeIndex);
        }

        
    }

    bool IsPlayerNear()
    {
        return (playerTransform.position - transform.position).sqrMagnitude
               <= interactRadius * interactRadius;
    }

    void PlayDialogueNode(int nodeIndex)
    {
        
        // 保证索引不越界，即就算nodeIndex比最大值还要大，也只会取最后一个Node
        int idx = Mathf.Clamp(nodeIndex, 0, dialogueAsset.dialogueNodes.Length - 1);
        var node = dialogueAsset.dialogueNodes[idx];

        // 如果节点需要任务完成，且未完成，则强制回退到最后一个无条件节点
        if (node.requireQuestComplete
            && !GameProgressManager.Instance.CheckQuestComplete(node.requiredQuest))
        {
            // 找到最后一个无条件节点
            Debug.Log("条件未满足，不能进入第 " + idx + " 条对话，回退到前一条。");
            // 回退到前一条
            idx = Mathf.Max(0, nodeIndex-1);
            node = dialogueAsset.dialogueNodes[idx];
            currentNodeIndex = idx;
        }

        // 播放对话，回调后移到下一节点
         // —— 播放“长段音频” —— 
            if (dialogueAudio != null)
            audioSource.PlayOneShot(dialogueAudio);
        
        dialogueUI.ShowDialogue(node.dialogueLines,OnNodeFinished);
    }
void OnNodeFinished()
{
    EnablePlayerControl();
    if(currentNodeIndex>=3){
        GameController.canDash = true;
    }
    // 最后一个节点（未命中以上任何情况），直接打开商店
        if (currentNodeIndex >= dialogueAsset.dialogueNodes.Length - 2)
        {
            ToggleShop();
        }
    currentNodeIndex++;
    // clamp 到最大值，防止越界
    currentNodeIndex = Mathf.Clamp(
        currentNodeIndex,
        0,
        dialogueAsset.dialogueNodes.Length - 1
    );
        
        
    
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



    void ToggleShop()
    {
        isShopOpen = !isShopOpen;
        shopUI.SetActive(isShopOpen);


        if (isShopOpen)
        {

            playerController.enabled = false;// 禁用玩家控制脚本
            playerAttack.enabled = false;
            playerMagic.enabled = false;

            Time.timeScale = 0;  //禁止时间
            PopulateItems();
        }
        else
        {
            playerController.enabled = true;
            playerAttack.enabled = true;
            playerMagic.enabled = true;
            Time.timeScale = 1;  //禁止时间
        }
    }

    void PopulateItems()
    {

        foreach (Transform child in itemListParent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < items.Length; i++)
        {
            Debug.Log(i);
            GameObject item = Instantiate(itemPrefab, itemListParent);
            ShopItemUI ui = item.GetComponent<ShopItemUI>();
            ui.Initialize(items[i], i == selectedIndex);
        }
    }

    void HandleNavigation()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            selectedIndex = Mathf.Max(0, selectedIndex - 1);
            UpdateSelection();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            selectedIndex = Mathf.Min(items.Length - 1, selectedIndex + 1);
            UpdateSelection();
        }
    }

    void UpdateSelection()
    {
        foreach (Transform child in itemListParent)
        {
            ShopItemUI ui = child.GetComponent<ShopItemUI>();
            ui.SetSelected(child.GetSiblingIndex() == selectedIndex);
        }
    }

    void HandlePurchase()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ShopItem selectedItem = items[selectedIndex];

            if (CoinUI.CurrentCoinQuantity >= selectedItem.price)
            {
                CoinUI.CurrentCoinQuantity -= selectedItem.price;
                GameController.CoinNum -= selectedItem.price;
                ApplyItemEffect(selectedItem);
                CoinUI.UpdateCoinDisplay();
            }
            else
            {
                // 显示购买失败提示
                Debug.Log("金币不足！");
            }
        }
    }

    void ApplyItemEffect(ShopItem item)
    {
        switch (item.itemType)
        {
            case ItemType.HealthUp:
                HealthBar.HealthMax += item.value;
                GameController.healthMax += item.value;
                //HealthBar.HealthCurrent = PlayerHealth.health;

                break;
            case ItemType.AttackUp:
                playerAttack.damage += item.value;
                GameController.damage += item.value;
                //PlayerCombat.Instance.AttackDamage += item.value;
                break;
            case ItemType.SpeedUp:
                //有点太bt了，加移速
                //PlayerMovement.Instance.MoveSpeed += item.value;
                break;
        }
    }

}


