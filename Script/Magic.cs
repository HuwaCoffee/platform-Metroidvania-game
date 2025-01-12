using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magic : MonoBehaviour
{

    public float destroyTime;
    private bool isMagicHit;
    public int damage;        //����˺�
    private Rigidbody2D rb2d;
    public bool pos;     //��PlayerMagic�������޸ķ����õ�
    public float speed;


    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, destroyTime);
        GameController.camShake.Shake();
        Debug.Log("posΪ" + pos);

    }

    // Update is called once per frame
    void Update()
    {

        /* rb2d = GetComponent<Rigidbody2D>();
         rb2d.velocity = transform.right * speed;*/

        if (pos)
        { //���ַ�ʽʵ��ƽ��
            Vector3 currentPosition = transform.position;

            // ����ʱ�����x������
            currentPosition.x += speed * Time.deltaTime;

            // ���������λ��
            transform.position = currentPosition;
        }
        else
        {
            Vector3 currentPosition = transform.position;

            // ����ʱ��ݼ�x������
            currentPosition.x -= speed * Time.deltaTime;

            // ���������λ��
            transform.position = currentPosition;
            //transform.position += new Vector3(-speed, 0, 0);  //��speedΪ�ٶ���x�ᷴ�������;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //collision�Ǵ�����������ǰ����������
        if (collision.gameObject.CompareTag("Enemy")) //���UnityEngine.CapsuleCollider2D��ʲô��˼�����ӿ�����
        {
            Debug.Log("Magic��������");
            isMagicHit = true;
            collision.GetComponent<Enemy>().TakeDamage(damage);
            GameController.camShake.Shake();      //����
            Destroy(gameObject);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        //collision�Ǵ�����������ǰ����������
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Magic�뿪����");
            isMagicHit = false;

        }

    }

}
