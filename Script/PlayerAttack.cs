using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerAttack : MonoBehaviour
{
   
    public MultiObjectPool multiObjectPool;// ���ö����

    private GameObject go; // ��ǰ��Ч
    private Transform playerTransform;

    public int damage;
    public float time; // ��������ʱ��
    public float startTime;
    public float attackCD;
    public float attackTrans;

    private Animator anim;
    private PolygonCollider2D myCollider2D;
    private int attackFlag = 0;
    private float attackCounter;
    private bool pos = true;
    private float moveDir;
   /* // ��������������������������
    private bool attackDirectionLocked = false;
    private bool lockedPos; // �����Ĺ�������*/

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        anim = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
        myCollider2D = GetComponent<PolygonCollider2D>();
        myCollider2D.enabled = false;
        attackCounter = attackTrans;

    }

    void Update()
    {
        if (GameController.isGameAlive && attackCounter < (attackTrans - attackCD))
        {
            Attack();
        }
        attackCounter -= Time.deltaTime;

        //ʱ�̸���moveDir����pos��ע������ı�
        moveDir = Input.GetAxis("Horizontal");
        if (moveDir > 0) pos = true;
        else if (moveDir < 0) pos = false;
    }

    void Attack()
    {
       
            
           
        

      
        //�Դ���˦�����淨�������ܲ��ܽ��һЩ�ؿ���ƽ�����ϰ���������ͬʱ��������������
        if (Input.GetButtonDown("Attack"))
        {  // ������������
           // attackDirectionLocked = true;
           // lockedPos = pos;
            if (attackCounter < -0.45f)
            {
                attackFlag = 1;
            }
            else
            {
                attackFlag = (attackFlag == 1) ? 2 : 1;
            }

            attackCounter = attackTrans;

            anim.SetInteger("attackFlag", attackFlag);
            StartCoroutine(TriggerAttack());
            StartCoroutine(StartAttack());
        }
    }

    IEnumerator TriggerAttack()
    {
        yield return null;
        anim.SetTrigger("isAttack");
    }

    IEnumerator StartAttack()
    {
        yield return new WaitForSeconds(startTime);

        myCollider2D.enabled = true;
        // ������Ч������λ��
        
        

        if (pos)
        {
            Vector3 offset = attackFlag==1 ? new Vector3(0.555f, 0.025f, 0f) : new Vector3(0.810f, 0.014f, 0f);   //�����ֵ����unity��һ��һ��Ų������
            Vector3 effectPosition = playerTransform.position + offset; 
            go = (attackFlag == 1)
                ? multiObjectPool.GetObject("���ǹ�����Ч1", effectPosition, Quaternion.Euler(0, 180, 0))
               
                : multiObjectPool.GetObject("���ǹ�����Ч2", effectPosition, Quaternion.Euler(0, 180, 0)); 
        }
        else 
        {
            Vector3 offset = attackFlag == 1 ? new Vector3(-0.555f, 0.025f, 0f) : new Vector3(-0.810f, 0.014f, 0f);   //�����ֵ����unity��һ��һ��Ų������
            Vector3 effectPosition = playerTransform.position + offset;
            go = (attackFlag == 1)
                ? multiObjectPool.GetObject("���ǹ�����Ч1", effectPosition, Quaternion.identity)
               
                : multiObjectPool.GetObject("���ǹ�����Ч2", effectPosition, Quaternion.identity);
            
        }
        // ������ЧΪ��ҵ��Ӷ���
        go.transform.SetParent(playerTransform);



        yield return new WaitForSeconds(time);

        if (attackFlag == 1)
        {
            multiObjectPool.ReturnObject("���ǹ�����Ч1", go);
      
        }
        else
        {
            multiObjectPool.ReturnObject("���ǹ�����Ч2", go);
    
        }

        myCollider2D.enabled = false;
        anim.ResetTrigger("isAttack");
        anim.SetTrigger("goNull");
        anim.SetBool("isIdle", true);

        // ��������
        //attackDirectionLocked = false;

        Debug.Log("������ɣ��ص�վ��״̬");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //�Ƚ��Ƿ�����bat
        if (collision.gameObject.CompareTag("Enemy"))
        {
            //����Enemy���TakeDamage()����
            collision.GetComponent<Enemy>().TakeDamage(damage);
        }
    }
}
