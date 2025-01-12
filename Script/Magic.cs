using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magic : MonoBehaviour
{

    public float destroyTime;
    private bool isMagicHit;
    public int damage;        //造成伤害
    private Rigidbody2D rb2d;
    public bool pos;     //给PlayerMagic类用于修改方向用的
    public float speed;


    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, destroyTime);
        GameController.camShake.Shake();
        Debug.Log("pos为" + pos);

    }

    // Update is called once per frame
    void Update()
    {

        /* rb2d = GetComponent<Rigidbody2D>();
         rb2d.velocity = transform.right * speed;*/

        if (pos)
        { //两种方式实现平移
            Vector3 currentPosition = transform.position;

            // 根据时间递增x轴坐标
            currentPosition.x += speed * Time.deltaTime;

            // 更新物体的位置
            transform.position = currentPosition;
        }
        else
        {
            Vector3 currentPosition = transform.position;

            // 根据时间递减x轴坐标
            currentPosition.x -= speed * Time.deltaTime;

            // 更新物体的位置
            transform.position = currentPosition;
            //transform.position += new Vector3(-speed, 0, 0);  //以speed为速度向x轴反方向飞行;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //collision是触发器，即当前碰到的物体
        if (collision.gameObject.CompareTag("Enemy")) //这个UnityEngine.CapsuleCollider2D是什么意思。不加可以吗
        {
            Debug.Log("Magic碰到敌人");
            isMagicHit = true;
            collision.GetComponent<Enemy>().TakeDamage(damage);
            GameController.camShake.Shake();      //抖动
            Destroy(gameObject);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        //collision是触发器，即当前碰到的物体
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Magic离开敌人");
            isMagicHit = false;

        }

    }

}
