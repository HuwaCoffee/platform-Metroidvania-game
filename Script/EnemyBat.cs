using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBat : Enemy
{
    public float speed;
    public float startWaitTime;
    private float waitTime;

    public Transform movePos;//下一次要移动的位置
    //移动范围
    public Transform leftDownPos;
    public Transform rightUpPos;

    
    // Start is called before the first frame update
    public void Start()
    {
        base.Start();
        waitTime = startWaitTime;
        movePos.position =GetRandomPos();//初始位置
        /*Debug.Log(movePos.position);
        Debug.Log(transform.position);*/
    }

    // Update is called once per frame
    public void Update()
    {

        base.Update();
        transform.position = Vector2.MoveTowards(transform.position, movePos.position, speed * Time.deltaTime);
        if (Vector2.Distance(transform.position, movePos.position)<0.1f)//比较是否到达目标位置
        {
            if (waitTime <= 0)
            {
                movePos.position = GetRandomPos();
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
        Vector2 rndPos = new Vector2(Random.Range(leftDownPos.position.x, rightUpPos.position.x), Random.Range(leftDownPos.position.y, rightUpPos.position.y));
        return rndPos;
    }
   



}
