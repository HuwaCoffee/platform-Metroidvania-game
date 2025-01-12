using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySnake : Enemy
{
    public float speed;
    public float waitTime;
    public Transform[] movePos;

    private int i = 0;
    private bool movingRight = true;
    private float wait;
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        wait = waitTime;
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        transform.position = Vector2.MoveTowards(transform.position,movePos[i].position,speed*Time.deltaTime);
        if (Vector2.Distance(transform.position, movePos[i].position) < 0.1f)
        {
            if (waitTime >= 0)
            {
                waitTime -= Time.deltaTime;
            }
            else{
                if (movingRight)  //ÌùÍ¼×ªÏò
                {
                    transform.eulerAngles = new Vector3(0, -180, 0);
                    movingRight = false;
                }
                else
                {
                    transform.eulerAngles = new Vector3(0, 0, 0);
                    movingRight = true;
                }
                i = (i==1) ? i = 0:i = 1;
                waitTime = wait;
            }
        }
    }
}
