using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagedTreasure : MonoBehaviour
{
    [SerializeField] private int health;
    public int TreasureId; //用于更新场景 

    [SerializeField] private string coinPoolTag = "Coin"; // 需要与对象池配置的tag一致
    [SerializeField] private float coinSpread = 1f; // 硬币散布范围

    [HideInInspector] public bool isOpened = false;


    void Start()
    {
         if (GameController.chests.ContainsKey(TreasureId))
        {
            this.isOpened = GameController.chests[TreasureId].isOpened;
        }
        else
        {
            GameController.chests.Add(TreasureId, new ChestSaveState { chestID = this.TreasureId, isOpened = this.isOpened });
        }
        if (isOpened)
        {
            gameObject.SetActive(false);
        }
       
    }

    public virtual void Update()
    {
        if (health <= 0)
        {

            //在此处生成10枚硬币
            GenerateCoins(10); // 生成10枚硬币
            isOpened = true;
            GameController.chests[TreasureId].isOpened=true;
            Destroy(transform.gameObject); //是不是不摧毁比较好，而是隐藏，不然无法获取到isOpen的对象实例
        }
    }

    public void TakeDamage()
    {

        Debug.Log("玩家造成伤害" + 1);

        health -= 1;

        //在此处生成5枚硬币
        GenerateCoins(5); // 生成10枚硬币
        GameController.camShake.Shake();

    }
    
     private void GenerateCoins(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            // 计算随机散布位置
            Vector3 spawnPos = transform.position + new Vector3(
                UnityEngine.Random.Range(-coinSpread, coinSpread),
                UnityEngine.Random.Range(-coinSpread, coinSpread),
                0
            );

            // 从对象池获取硬币
            GameObject coin = MultiObjectPool.Instance.GetObject(
                coinPoolTag,
                spawnPos,
                Quaternion.identity
            );

            // 可选：添加物理效果
            if (coin.TryGetComponent<Rigidbody2D>(out var rb))
            {
                Vector2 force = new Vector2(
                    UnityEngine.Random.Range(-3f, 3f),
                    UnityEngine.Random.Range(5f, 8f)
                );
                rb.AddForce(force, ForceMode2D.Impulse);
            }
        }
    }
}
