using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour
{
    [Header("门唯一 ID，用于查找")]
    public int doorID;
    [HideInInspector]public bool isTrigger = false;

    /// <summary>
    /// 只保留「初始化当前位置」逻辑，之后移动都由 MoveTo 调用。
    /// </summary>
    private Vector3 initialPosition;

    void Awake()
    {
        initialPosition = transform.position;
    }

    void Start()
    {
        if (GameController.doors.ContainsKey(doorID))
        {
            this.isTrigger = GameController.doors[doorID].isTrigger;
        }
        else
        {
            GameController.doors.Add( doorID,new DoorSaveState { doorID = this.doorID, isTrigger = this.isTrigger });
        }
        if (isTrigger)
        {
            gameObject.SetActive(false); //如果已经触发，隐藏门
        }
        
    }

    /// <summary>
    /// 通用接口：把门从当前位置平滑移到目标位置
    /// </summary>
    public void MoveTo(Vector3 targetPosition, float duration)
    {
        isTrigger = true;
        StopAllCoroutines();
        StartCoroutine(MoveRoutine(targetPosition, duration));
    }

    private IEnumerator MoveRoutine(Vector3 target, float duration)
    {
        Vector3 start = transform.position;
        float t = 0f;
        while (t < duration)
        {
            transform.position = Vector3.Lerp(start, target, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        transform.position = target;
        
    }

    /// <summary>
    /// 快速重置回初始位置
    /// </summary>
    public void ResetDoor()
    {
        StopAllCoroutines();
        transform.position = initialPosition;
    }
}
