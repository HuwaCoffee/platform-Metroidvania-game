using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerAttack : MonoBehaviour
{
   public static PlayerAttack Instance;
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
   
    private float moveDir;

    void Awake()
    {
        // 单例初始化
        if (Instance == null) Instance = this;
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        anim = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
        myCollider2D = GetComponent<PolygonCollider2D>();
        myCollider2D.enabled = false;
        attackCounter = attackTrans;

        if (GameController.damage != 0)
        {
            damage = GameController.damage;
        }

    }

    void Update()
    {
        if (GameController.isGameAlive && attackCounter < (attackTrans - attackCD)&& GameController.canAttack)
        {
            Attack();
        }
        attackCounter -= Time.deltaTime;

        
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
        // 计算特效的生成位置，锁定角色朝向
        GameController.canFlip = false;
        if (GameController.pos)
        {
            Vector3 offset = attackFlag==1 ? new Vector3(0.560f, -0.024f, 0f) : new Vector3(0.823f, -0.080f, 0f);   //这个数值是在unity中一点一点挪出来的
            Vector3 effectPosition = playerTransform.position + offset; 
            go = (attackFlag == 1)
                ? multiObjectPool.GetObject("主角攻击特效1", effectPosition, Quaternion.Euler(0, 180, 0))
               
                : multiObjectPool.GetObject("主角攻击特效2", effectPosition, Quaternion.Euler(0, 180, 0)); 
        }
        else 
        {
            Vector3 offset = attackFlag == 1 ? new Vector3(-0.560f, -0.024f, 0f) : new Vector3(-0.823f, -0.080f, 0f);   //这个数值是在unity中一点一点挪出来的
            Vector3 effectPosition = playerTransform.position + offset;
            go = (attackFlag == 1)
                ? multiObjectPool.GetObject("主角攻击特效1", effectPosition, Quaternion.identity)
               
                : multiObjectPool.GetObject("主角攻击特效2", effectPosition, Quaternion.identity);
            
        }
        // 设置特效为玩家的子对象
        go.transform.SetParent(playerTransform);
        


        yield return new WaitForSeconds(time);
        GameController.canFlip = true;
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

        if (collision.gameObject.CompareTag("OneWayDoor"))
        {
            collision.GetComponent<DamagedDoor>().TakeDamage();
        }
        if (collision.gameObject.CompareTag("Treasure"))
        {
            collision.GetComponent<DamagedTreasure>().TakeDamage();
        }
        if (collision.gameObject.CompareTag("Switch"))
        {
            collision.GetComponent<SwitchController>().TakeTrigger();
        }
    }
}
