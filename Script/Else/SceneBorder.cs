using UnityEngine;

public class SceneBorder : MonoBehaviour
{
    [Header("边界配置")]
    public int borderID = 1;
    public Transform minPos;
    public Transform maxPos;
    
    [Header("Gizmos")]
    public Color borderColor = Color.red;
    
    void OnDrawGizmos()
    {
        if (!minPos || !maxPos) return;
        
        Gizmos.color = borderColor;
        Gizmos.DrawLine(
            new Vector3(minPos.position.x, minPos.position.y, 0),
            new Vector3(maxPos.position.x, minPos.position.y, 0)
        );
        Gizmos.DrawLine(
            new Vector3(maxPos.position.x, minPos.position.y, 0),
            new Vector3(maxPos.position.x, maxPos.position.y, 0)
        );
    }
}