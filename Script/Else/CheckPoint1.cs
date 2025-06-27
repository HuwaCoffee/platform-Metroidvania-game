using UnityEngine;

public class CheckPoint1 : MonoBehaviour
{
    [Header("配置参数")]
    public int boarderID = 1; // 对应的场景边界ID
    public Transform triggerArea; // 触发器区域

     public int doorID;
    public Vector3 offset;   // 例如 new Vector3(0, -3, 0)
    public float moveDuration;

    private bool hasTriggered=false;

    [Header("Gizmos")]
    public Color gizmoColor = new Color(0, 1, 0, 0.3f);

    void OnDrawGizmos()
    {
        if (triggerArea == null) return;

        Gizmos.color = gizmoColor;
        Gizmos.DrawCube(triggerArea.position, triggerArea.localScale);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        // 修改点：通过摄像机父物体获取组件
        Transform cameraParent = Camera.main.transform.parent;
        var cameraFollow = cameraParent.GetComponent<CameraFollow>();
        cameraFollow.SwitchBorder(boarderID);
        if (!hasTriggered)
        {
            // 打开门
            var door = FindDoor(doorID);
            if (door != null)
                door.MoveTo(door.transform.position + offset, moveDuration);
            hasTriggered = true;
        }
       
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