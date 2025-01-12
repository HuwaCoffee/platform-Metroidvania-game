using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatDestroy : MonoBehaviour
{
    public int batFlag; //表示当前蝙蝠在彩蛋中的编号 ，解密，密码门都可以用这种方法实现
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //unity自带方法，被destroy时自动触发
    private void OnDestroy()
    {
        EasterEgg.Password += batFlag.ToString();
        Debug.Log("蝙蝠"+batFlag+"死掉了");
        Debug.Log(EasterEgg.Password);
    }
}
