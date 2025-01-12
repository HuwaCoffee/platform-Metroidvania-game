using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 相当于工厂模式中的工厂，善于进行多个/重复的Gameobject的回收和使用
/// 使用到的预制体有，tag：主角攻击特效1，size：1
///                        主角攻击特效2，size：1
/// </summary>
public class MultiObjectPool : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;          // 每个预制体的唯一标识
        public GameObject prefab;   // 对应的预制体
        public int size;            // 初始池大小
    }

    public List<Pool> pools;                 // 存储多个对象池配置
    private Dictionary<string, Queue<GameObject>> poolDictionary; // 对象池字典

    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    // 从池中获取对象
    public GameObject GetObject(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag {tag} does not exist.");
            return null;
        }

        GameObject obj;
        if (poolDictionary[tag].Count > 0)
        {
            obj = poolDictionary[tag].Dequeue();
        }
        else
        {
            // 如果池中没有对象，动态创建一个新对象
            Pool pool = pools.Find(p => p.tag == tag);
            if (pool != null)
            {
                obj = Instantiate(pool.prefab);
            }
            else
            {
                Debug.LogWarning($"Prefab with tag {tag} not found in pools.");
                return null;
            }
        }

        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);

        return obj;
    }

    // 将对象归还到池中
    public void ReturnObject(string tag, GameObject obj)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag {tag} does not exist.");
            return;
        }

        obj.SetActive(false);
        poolDictionary[tag].Enqueue(obj);
    }
}
