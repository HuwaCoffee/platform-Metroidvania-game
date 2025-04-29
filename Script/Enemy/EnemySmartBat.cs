using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySmartBat : Enemy //继承自Enemy父类
{
    public float speed; //追击速度
    public float radius; //怪物检测半径

    private Transform playerTransform;
    // Start is called before the first frame update
    //使用 new 隐藏父类方法时，子类的方法不会自动执行父类的方法。
    //如果你希望子类能够继承并执行父类的行为，最好使用 override（如果父类方法是 virtual 或 abstract）
    public override void Start()
    {
        base.Start();//调用父类的start方法用base
        playerTransform= GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    public override void Update()
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
