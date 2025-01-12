using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �൱�ڹ���ģʽ�еĹ��������ڽ��ж��/�ظ���Gameobject�Ļ��պ�ʹ��
/// ʹ�õ���Ԥ�����У�tag�����ǹ�����Ч1��size��1
///                        ���ǹ�����Ч2��size��1
/// </summary>
public class MultiObjectPool : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;          // ÿ��Ԥ�����Ψһ��ʶ
        public GameObject prefab;   // ��Ӧ��Ԥ����
        public int size;            // ��ʼ�ش�С
    }

    public List<Pool> pools;                 // �洢������������
    private Dictionary<string, Queue<GameObject>> poolDictionary; // ������ֵ�

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

    // �ӳ��л�ȡ����
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
            // �������û�ж��󣬶�̬����һ���¶���
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

    // ������黹������
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
