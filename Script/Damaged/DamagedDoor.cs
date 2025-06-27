using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagedDoor : MonoBehaviour
{
    [SerializeField] private int health;
    //特效
    [SerializeField] private GameObject ashEffect;

    [SerializeField] private int doorId; //用于更新场景 

    public virtual void Update()
    {
        if (health <= 0)
        {
            //销毁物体
            Destroy(transform.parent.gameObject);
        }
    }

    public void TakeDamage()
    {

        Debug.Log("玩家造成伤害" + 1);

        health -= 1;

        //将bloodEffect物体，生成在自身位置，不旋转（旋转的初始角度，0,0,0）
        
         if (ashEffect != null)
        Instantiate(ashEffect, transform.parent.position + Vector3.down, Quaternion.identity);
    else
        Debug.LogWarning($"[{name}] ashEffect is null!");
        GameController.camShake.Shake();
        
    }
}
