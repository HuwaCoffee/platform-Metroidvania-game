using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBat : Enemy
{
    public float speed;
    public float startWaitTime;
    private float waitTime;

    private Vector2 targetPos;//下一次要移动的位置
    //移动范围
    public Transform leftDownPos;
    public Transform rightUpPos;

    
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        waitTime = startWaitTime;
        targetPos =GetRandomPos();//初始位置
        /*Debug.Log(movePos.position);
        Debug.Log(transform.position);*/
    }

    // Update is called once per frame
    public override void Update()
    {

        base.Update();
        transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        if (Vector2.Distance(transform.position, targetPos)<0.1f)//比较是否到达目标位置
        {
            if (waitTime <= 0)
            {
                targetPos = GetRandomPos();
                waitTime = startWaitTime;
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }
    }

    Vector2 GetRandomPos()
    {
        Vector2 rndPos = new Vector2(UnityEngine.Random.Range(leftDownPos.position.x, rightUpPos.position.x), UnityEngine.Random.Range(leftDownPos.position.y, rightUpPos.position.y));
        return rndPos;
    }
   



}
