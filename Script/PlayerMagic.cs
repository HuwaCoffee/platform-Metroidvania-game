using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMagic : MonoBehaviour
{
    public GameObject magi;   //�̲�����
    public float speed;       //�����ٶ�



    private Transform playerTransform;
    /*private Transform sickleTransform;
    private Vector2 startSpeed;

    private CameraShake camshake;*/
    

    //ͨ���ж�pos��֪���Ƿ���0������1������
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
        float moveDir = Input.GetAxis("Horizontal");//(-1��1֮��)
        
        if (moveDir > 0) pos = true;
        else if (moveDir < 0) pos = false; //���ǵ���ֹ�������ֻ������д��������һ�εĳ���


        if (Input.GetButtonDown("Magic"))
        {  //������ϣ�&&�з���ֵ
            Debug.Log("��U");
            if (pos)
            {
               GameObject go=Instantiate(magi, playerTransform.localPosition, Quaternion.identity) as GameObject; //ħ���ű�������ħ�������ϣ�������λ�����������
               go.GetComponent<Magic>().pos = true;
            }
            else
            {
                GameObject go = Instantiate(magi, playerTransform.localPosition, Quaternion.Euler(0, 180, 0)) as GameObject; //ħ���ű�������ħ�������ϣ�������λ�����������
                go.GetComponent<Magic>().pos = false;
            }
           
        }

      

    }


    
}
