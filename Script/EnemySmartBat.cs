using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySmartBat : Enemy //继承自Enemy父类
{
    public float speed; //追击速度
    public float radius; //怪物检测半径

    private Transform playerTransform;
    // Start is called before the first frame update
    void Start()
    {
        base.Start();//调用父类的start方法用base
        playerTransform= GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        if (playerTransform != null)
        {
            float distance = (transform.position-playerTransform.position).sqrMagnitude; //计算两点之间距离
            if (distance < radius)
            {
                transform.position = Vector2.MoveTowards(transform.position, playerTransform.position,speed*Time.deltaTime);

            }
        }
    }
}
