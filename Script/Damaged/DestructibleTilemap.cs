
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
[RequireComponent(typeof(Collider2D))]
public class DestructibleTilemap : MonoBehaviour
{
    [Header("需要与玩家攻击碰撞的标签")]
    [Tooltip("玩家攻击的碰撞体应设置为此标签")]
    public string attackTag = "Attack";

    [Header("清除范围")]
    [Tooltip("以攻击点为中心，周围多大范围（世界坐标）内的瓦片都一并清除")]
    public float clearRadius = 1f;

    [SerializeField] private GameObject ashEffect;

    private Tilemap tilemap;

    void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        // 确保 Collider2D 是 Trigger
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(attackTag)) return;

        Vector3 hitPoint = other.bounds.center;
        ClearTilesAround(hitPoint);
    }

    private void ClearTilesAround(Vector3 worldPoint)
    {
        // 1. 先计算清除半径对应的“格子半径”
        //    假设你的 Tilemap 每个格子大小正好是 1x1 世界单位：
        int cellRadius = Mathf.CeilToInt(clearRadius);

        // 2. 把世界点转到格子坐标
        Vector3Int centerCell = tilemap.WorldToCell(worldPoint);

        // 3. 遍历中心格子周围的一个正方形区域
        for (int dx = -cellRadius; dx <= cellRadius; dx++)
        {
            for (int dy = -cellRadius; dy <= cellRadius; dy++)
            {
                Vector3Int cellPos = new Vector3Int(
                    centerCell.x + dx,
                    centerCell.y + dy,
                    centerCell.z
                );

                // 4. 把格子中心再转回世界坐标，判断距离
                Vector3 cellWorldCenter = tilemap.GetCellCenterWorld(cellPos);
                if (Vector3.Distance(cellWorldCenter, worldPoint) <= clearRadius)
                {
                    if (tilemap.HasTile(cellPos))
                    {
                        tilemap.SetTile(cellPos, null);
                        Instantiate(ashEffect, cellWorldCenter, Quaternion.identity);
                        GameController.camShake.Shake();
                    }
                }
            }
        }
    }
}
