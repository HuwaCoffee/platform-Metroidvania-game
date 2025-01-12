using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerAttack : MonoBehaviour
{
   
    public MultiObjectPool multiObjectPool;// 引用对象池

    private GameObject go; // 当前特效
    private Transform playerTransform;

    public int damage;
    public float time; // 攻击持续时间
    public float startTime;
    public float attackCD;
    public float attackTrans;

    private Animator anim;
    private PolygonCollider2D myCollider2D;
    private int attackFlag = 0;
    private float attackCounter;
    private bool pos = true;
    private float moveDir;
   /* // 新增变量，用于锁定攻击方向
    private bool attackDirectionLocked = false;
    private bool lockedPos; // 锁定的攻击方向*/

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

        //时刻根据moveDir更新pos，注意如果改变
        moveDir = Input.GetAxis("Horizontal");
        if (moveDir > 0) pos = true;
        else if (moveDir < 0) pos = false;
    }

    void Attack()
    {
       
            
           
        

      
        //自创“甩刀”玩法，看看能不能结合一些关卡设计进行练习，比如必须同时按左右两个开关
        if (Input.GetButtonDown("Attack"))
        {  // 锁定攻击方向
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
        // 计算特效的生成位置
        
        

        if (pos)
        {
            Vector3 offset = attackFlag==1 ? new Vector3(0.555f, 0.025f, 0f) : new Vector3(0.810f, 0.014f, 0f);   //这个数值是在unity中一点一点挪出来的
            Vector3 effectPosition = playerTransform.position + offset; 
            go = (attackFlag == 1)
                ? multiObjectPool.GetObject("主角攻击特效1", effectPosition, Quaternion.Euler(0, 180, 0))
               
                : multiObjectPool.GetObject("主角攻击特效2", effectPosition, Quaternion.Euler(0, 180, 0)); 
        }
        else 
        {
            Vector3 offset = attackFlag == 1 ? new Vector3(-0.555f, 0.025f, 0f) : new Vector3(-0.810f, 0.014f, 0f);   //这个数值是在unity中一点一点挪出来的
            Vector3 effectPosition = playerTransform.position + offset;
            go = (attackFlag == 1)
                ? multiObjectPool.GetObject("主角攻击特效1", effectPosition, Quaternion.identity)
               
                : multiObjectPool.GetObject("主角攻击特效2", effectPosition, Quaternion.identity);
            
        }
        // 设置特效为玩家的子对象
        go.transform.SetParent(playerTransform);



        yield return new WaitForSeconds(time);

        if (attackFlag == 1)
        {
            multiObjectPool.ReturnObject("主角攻击特效1", go);
      
        }
        else
        {
            multiObjectPool.ReturnObject("主角攻击特效2", go);
    
        }

        myCollider2D.enabled = false;
        anim.ResetTrigger("isAttack");
        anim.SetTrigger("goNull");
        anim.SetBool("isIdle", true);

        // 解锁方向
        //attackDirectionLocked = false;

        Debug.Log("攻击完成，回到站立状态");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //比较是否碰到bat
        if (collision.gameObject.CompareTag("Enemy"))
        {
            //调用Enemy类的TakeDamage()方法
            collision.GetComponent<Enemy>().TakeDamage(damage);
        }
    }
}
