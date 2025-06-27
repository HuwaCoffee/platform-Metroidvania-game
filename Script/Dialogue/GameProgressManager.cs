
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class GameProgressManager : MonoBehaviour
{
    public static GameProgressManager Instance { get; private set; }

    [Header("任务完成标记（根据 QuestType 枚举长度自动初始化）")]
    public bool[] completedQuests;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // 自动根据 QuestType 枚举大小初始化
            int questCount = System.Enum.GetValues(typeof(DialogueData.QuestType)).Length;
            completedQuests = new bool[questCount];
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 标记某个任务完成
    /// </summary>
    public void CompleteQuest(DialogueData.QuestType quest)
    {
        completedQuests[(int)quest] = true;
    }

    /// <summary>
    /// 查询某个任务是否完成
    /// </summary>
    public bool CheckQuestComplete(DialogueData.QuestType quest)
    {
        return completedQuests[(int)quest];
    }
}
