
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class NPCDialogueController : MonoBehaviour
{
    [Header("对话数据 (SO)")]
    public DialogueData dialogueAsset;

    /// <summary>
    /// 获取指定索引的 DialogueNode（外部如 Merchant 根据索引播放）
    /// </summary>
    public DialogueData.DialogueNode GetNode(int index)
    {
        int clamped = Mathf.Clamp(index, 0, dialogueAsset.dialogueNodes.Length - 1);
        return dialogueAsset.dialogueNodes[clamped];
    }
}
