using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float speed;
    public float waitTime;
    public Transform[] movePos;
    
    private int i;
    private float defaultTime;//�洢��ʼֵ
    private Transform playerDefTransform;
    // Start is called before the first frame update
    void Start()
    {
        i = 1;
        defaultTime = waitTime;
        playerDefTransform = GameObject.FindGameObjectWithTag("Player").transform.parent;//PlayerĬ�ϵĸ���
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
            collision.gameObject.transform.parent = gameObject.transform;//������ǰ��������Ӧ���壨Player���ĸ����ǵ�ǰ����
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
            collision.gameObject.transform.parent = playerDefTransform;//Player�ĸ�����PlayerĬ�ϵĸ���
        }
        
    }
}
