using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public int damage;
    public int health;
    public float flashTime;
    public GameObject bat;

    private SpriteRenderer sr;
    private Color originalColor;//�������ɫ
    private PlayerHealth playerHealth;

    //��Ч
    public GameObject bloodEffect;

    //�˺�����
    public GameObject floatPoint;

    //������
    public GameObject dropCoin;
    // Start is called before the first frame update
    public void Start()
    {
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
    }

    // Update is called once per frame
    public void Update()
    {
        if (health <= 0)
        {
            //����Ӳ��
            Instantiate(dropCoin, transform.position, Quaternion.identity);
            Destroy(gameObject);

            //���ٸ�����(��ǰ�������������)
            Destroy(bat);
        }
    }

    public void TakeDamage(int playerDamage)
    {
        GameObject gb = Instantiate(floatPoint, transform.position, Quaternion.identity) as GameObject;
        Debug.Log("�������˺�"+playerDamage);
        gb.transform.GetChild(0).GetComponent<TextMesh>().text = playerDamage.ToString();
        health -= playerDamage;
        FlashColor(flashTime);
        //��bloodEffect���壬����������λ�ã�����ת����ת�ĳ�ʼ�Ƕȣ�0,0,0��
        Instantiate(bloodEffect, transform.position, Quaternion.identity);
        GameController.camShake.Shake();
    }

    void FlashColor(float time)
    {
        sr.color = Color.red;
        //��ʱ����ResetColor
        Invoke("ResetColor", time);

    }

    void ResetColor()
    {
        sr.color = originalColor;
    }

    //�˷����Ǵ�����������������������ʱ���ã����������Ĵ�������ֵ��collision
    //Enter�ǽ��봥����,Exit���뿪��Stay�ǳ���������
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        //collision�Ǵ�����������ǰ����������
        if(collision.gameObject.CompareTag("Player")&&collision.GetType().ToString()== "UnityEngine.CapsuleCollider2D")
        {
            if (playerHealth != null)
            {
                playerHealth.DamagePlayer(damage);
            }
        }
    }
}
