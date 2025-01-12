using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMagic : MonoBehaviour
{
    public GameObject magi;   //绿波本身
    public float speed;       //飞行速度



    private Transform playerTransform;
    /*private Transform sickleTransform;
    private Vector2 startSpeed;

    private CameraShake camshake;*/
    

    //通过判断pos得知主角方向，0代表朝左，1代表朝右
    private bool pos = true;

    // Start is called before the first frame update
    void Start()
    {
        
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();


      /*  sickleTransform = GetComponent<Transform>();
        camshake = GameObject.FindGameObjectWithTag("CameraShake").GetComponent<CameraShake>();*/
    }

    // Update is called once per frame
    void Update()
    {
        float moveDir = Input.GetAxis("Horizontal");//(-1到1之间)
        
        if (moveDir > 0) pos = true;
        else if (moveDir < 0) pos = false; //考虑到静止的情况，只能这样写，保留上一次的朝向


        if (Input.GetButtonDown("Magic"))
        {  //后面加上，&&有法术值
            Debug.Log("按U");
            if (pos)
            {
               GameObject go=Instantiate(magi, playerTransform.localPosition, Quaternion.identity) as GameObject; //魔法脚本挂载在魔法物理上，但生成位置在玩家身上
               go.GetComponent<Magic>().pos = true;
            }
            else
            {
                GameObject go = Instantiate(magi, playerTransform.localPosition, Quaternion.Euler(0, 180, 0)) as GameObject; //魔法脚本挂载在魔法物理上，但生成位置在玩家身上
                go.GetComponent<Magic>().pos = false;
            }
           
        }

      

    }


    
}
