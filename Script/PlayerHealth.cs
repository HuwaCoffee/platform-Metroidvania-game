using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health;
    public int numBlinks;
    public float blinkSeconds;
    public float invincibleSeconds;//无敌时间
    public float dieTime;//死亡时间（最好和动画时间一致）
    private Renderer myRender;
    private bool isHurt=false;
    private Animator anim;
    private ScreenFlash sf;
    // Start is called before the first frame update
    void Start()
    {
        myRender = GetComponent<Renderer>();
        anim = GetComponent<Animator>();
        HealthBar.HealthMax = health;
        HealthBar.HealthCurrent = health;
        sf = GetComponent<ScreenFlash>(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DamagePlayer(int damage)
    {
        //受伤闪烁期间不会再次受伤，相当于无敌时间
        if (isHurt == false)
        {
            sf.FlashScreen();
            health -= damage;
            HealthBar.HealthCurrent = health;
            
            if (health <= 0)
            {
                GameController.isGameAlive = false;
                isHurt = true;
                anim.SetTrigger("isDie");
                //0.833秒后调用KillPlayer方法
                Invoke("KillPlayer", dieTime);
            }
            else
            {
                isHurt = true;
                WaitForHurt(invincibleSeconds);
                BlinkPlayer(numBlinks, blinkSeconds);
            }
            
        }
        
    }

    void KillPlayer()
    {
        Destroy(gameObject);
    }

    void BlinkPlayer(int numBlinks,float blinkSeconds)
    {
        StartCoroutine(DoBlink(numBlinks, blinkSeconds));
    }
    
    void WaitForHurt(float invincibleSeconds)
    {
        StartCoroutine(DoWait(invincibleSeconds));
    }

    IEnumerator DoBlink(int numBlinks, float blinkSeconds)
    {
        for(int i = 0; i < numBlinks * 2; i++)
        {
            Debug.Log(i);
            myRender.enabled = !myRender.enabled;
            yield return new WaitForSeconds(blinkSeconds);//seconds秒后开始闪烁
        }
        myRender.enabled = true;
    }
    IEnumerator DoWait(float invincibleSeconds)
    {
        yield return new WaitForSeconds(invincibleSeconds);//seconds秒后可以受到伤害
        isHurt = false;
    }
}
