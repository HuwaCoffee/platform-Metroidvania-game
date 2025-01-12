using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float speed;
    public float waitTime;
    public Transform[] movePos;
    
    private int i;
    private float defaultTime;//存储初始值
    private Transform playerDefTransform;
    // Start is called before the first frame update
    void Start()
    {
        i = 1;
        defaultTime = waitTime;
        playerDefTransform = GameObject.FindGameObjectWithTag("Player").transform.parent;//Player默认的父类
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, movePos[i].position, speed * Time.deltaTime);
        if (Vector2.Distance(transform.position, movePos[i].position) < 0.1f)
        {
            if (waitTime < 0.0f)
            {
                i=(i == 0) ? 1 : 0;
                waitTime = defaultTime;
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.ToString());
        if (collision.CompareTag("Player") && collision.GetType().ToString() == "UnityEngine.BoxCollider2D")
        {
            collision.gameObject.transform.parent = gameObject.transform;//碰到当前触发器对应物体（Player）的父类是当前物体
        }
        if (collision.CompareTag("CoinItem") && collision.GetType().ToString() == "UnityEngine.BoxCollider2D")
        {
            collision.gameObject.transform.parent = gameObject.transform;
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.GetType().ToString() == "UnityEngine.BoxCollider2D")
        {
            collision.gameObject.transform.parent = playerDefTransform;//Player的父类是Player默认的父类
        }
        
    }
}
