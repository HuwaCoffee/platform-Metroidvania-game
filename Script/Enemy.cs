using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public int damage;
    public int health;
    public float flashTime;
    public GameObject bat;

    private SpriteRenderer sr;
    private Color originalColor;//最初的颜色
    private PlayerHealth playerHealth;

    //特效
    public GameObject bloodEffect;

    //伤害数字
    public GameObject floatPoint;

    //掉落金币
    public GameObject dropCoin;
    // Start is called before the first frame update
    public void Start()
    {
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
    }

    // Update is called once per frame
    public void Update()
    {
        if (health <= 0)
        {
            //生成硬币
            Instantiate(dropCoin, transform.position, Quaternion.identity);
            Destroy(gameObject);

            //销毁父物体(当前生物和两个坐标)
            Destroy(bat);
        }
    }

    public void TakeDamage(int playerDamage)
    {
        GameObject gb = Instantiate(floatPoint, transform.position, Quaternion.identity) as GameObject;
        Debug.Log("玩家造成伤害"+playerDamage);
        gb.transform.GetChild(0).GetComponent<TextMesh>().text = playerDamage.ToString();
        health -= playerDamage;
        FlashColor(flashTime);
        //将bloodEffect物体，生成在自身位置，不旋转（旋转的初始角度，0,0,0）
        Instantiate(bloodEffect, transform.position, Quaternion.identity);
        GameController.camShake.Shake();
    }

    void FlashColor(float time)
    {
        sr.color = Color.red;
        //延时调用ResetColor
        Invoke("ResetColor", time);

    }

    void ResetColor()
    {
        sr.color = originalColor;
    }

    //此方法是触发函数，当触发器被触发时调用，并将碰到的触发器赋值给collision
    //Enter是进入触发器,Exit是离开，Stay是持续在里面
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        //collision是触发器，即当前碰到的物体
        if(collision.gameObject.CompareTag("Player")&&collision.GetType().ToString()== "UnityEngine.CapsuleCollider2D")
        {
            if (playerHealth != null)
            {
                playerHealth.DamagePlayer(damage);
            }
        }
    }
}
