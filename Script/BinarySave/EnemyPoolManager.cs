using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPoolManager : MonoBehaviour
{
    public static EnemyPoolManager Instance;

    [System.Serializable]
    public class PoolConfig
    {
        public string enemyType;
        public GameObject prefab;
        public int initialSize;
    }

    public List<PoolConfig> poolConfigs;
    private Dictionary<string, Queue<GameObject>> pools = 
        new Dictionary<string, Queue<GameObject>>();

    void Awake()
    {
        Instance = this;
        InitializePools();
    }

    private void InitializePools()
    {
        foreach (var config in poolConfigs)
        {
            var queue = new Queue<GameObject>();
            for (int i = 0; i < config.initialSize; i++)
            {
                var obj = Instantiate(config.prefab);
                obj.SetActive(false);
                queue.Enqueue(obj);
            }
            pools.Add(config.enemyType, queue);
        }
    }

    public GameObject SpawnEnemy(string type, Vector3 position)
    {
        if (!pools.ContainsKey(type)) return null;

        var enemy = pools[type].Dequeue();
        enemy.transform.position = position;
        enemy.SetActive(true);
        return enemy;
    }

    public void RecycleEnemy(GameObject enemy)
    {
        // var type = enemy.GetComponent<Enemy>().enemyType;
        // enemy.SetActive(false);
        // pools[type].Enqueue(enemy);
    }

    public void ResetAllEnemies()
    {
        foreach (var pool in pools.Values)
        {
            foreach (var enemy in pool)
            {
                if (enemy.activeSelf)
                {
                    //enemy.GetComponent<Enemy>().ResetState();
                    RecycleEnemy(enemy);
                }
            }
        }
    }
}
