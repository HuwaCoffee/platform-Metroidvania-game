using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySmartBat : Enemy //�̳���Enemy����
{
    public float speed; //׷���ٶ�
    public float radius; //������뾶

    private Transform playerTransform;
    // Start is called before the first frame update
    void Start()
    {
        base.Start();//���ø����start������base
        playerTransform= GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        if (playerTransform != null)
        {
            float distance = (transform.position-playerTransform.position).sqrMagnitude; //��������֮�����
            if (distance < radius)
            {
                transform.position = Vector2.MoveTowards(transform.position, playerTransform.position,speed*Time.deltaTime);

            }
        }
    }
}
