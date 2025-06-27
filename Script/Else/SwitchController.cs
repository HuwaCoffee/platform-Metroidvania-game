using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchController : MonoBehaviour
{
    public int doorID;           // Inspector 里指定

    public int switchID; 
    public Vector3 offset;       // 本机关希望的偏移向量
    public float moveDuration;   // 本机关希望的移动时长
    private Animator anim;

    [HideInInspector]public bool hasTriggered=false;

    void Start()
    {
        anim = GetComponent<Animator>();
        if (GameController.switches.ContainsKey(switchID))
        {
            this.hasTriggered = GameController.switches[switchID].isTriggered;
        }
        else
        {
            GameController.switches.Add(switchID, new SwitchSaveState { switchID = this.switchID, isTriggered = this.hasTriggered });
        }
        if (hasTriggered)
        {
            anim.SetTrigger("isTrigger");
        }

        
    }
    public void TakeTrigger()
    {
        if (!hasTriggered)
        {
            // 2. 打开这扇门
            var door = FindDoor(doorID);
            if (door != null)
                door.MoveTo(door.transform.position + offset, moveDuration);
            hasTriggered = true;
        }
        GameController.doors[doorID].isTrigger = true;
        GameController.switches[switchID].isTriggered = true;
        anim.SetTrigger("isTrigger");
        GameController.camShake.Shake();
        // 1. 当玩家完成任务时调用
        GameProgressManager.Instance.CompleteQuest(DialogueData.QuestType.FindSword);

        
        
      

    }
      private DoorController FindDoor(int id)
    {
        var allDoors = GameObject.FindObjectsOfType<DoorController>();
    foreach (var d in allDoors)
    {
        if (d.doorID == id)
            return d;
    }
    return null;
    }
}
