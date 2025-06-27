
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

[CreateAssetMenu(fileName = "NPC Dialogue", menuName = "Dialogue/NPC Dialogue")]
public class DialogueData : ScriptableObject
{
    public int npcID;                       // 挂载此资源的 NPC ID
    public DialogueNode[] dialogueNodes;    // 按顺序填写所有节点

    public enum QuestType { None, FindSword, DefeatBoss }

    [System.Serializable]
    public class DialogueNode
    {
        [TextArea(1, 6)]
        public string[] dialogueLines;               // 本节点的所有台词

        public bool requireQuestComplete;    // 是否需要任务完成才可播放
        public QuestType requiredQuest;      // 如果 requireQuestComplete 为 true，检查此任务
    }
}
