using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public Transform target;
    public float smoothing;
    public Vector2 minPosition;
    public Vector2 maxPosition;

    public Transform staticBackground, farBackground, middleBackground; //��ֹ�����Ĵ�ɫ������Զ�����о�  ���ֲ����
    private Vector2 lastPos; //���һ�����λ��

    // Start is called before the first frame update
    void Start()
    {
        lastPos = transform.position; //�����ʼλ��
        //��GameController���camShake��������ֵΪTagΪCameraShake�����CameraShake�ű����
        GameController.camShake = GameObject.FindGameObjectWithTag("CameraShake").GetComponent<CameraShake>();
    }

    void FixedUpdate()
    {
        if (target != null)
        {
            //�����������һ֡�͵�ǰ֮֡���ƶ��ľ��루�����ԣ����о���������ƶ����룬�����ĸ��ã�
            Vector2 amoutToMove = new Vector2(transform.position.x - lastPos.x, transform.position.y - lastPos.y);

            //�ƶ�Զ�����о�
            farBackground.position += new Vector3(amoutToMove.x * 0.1f, amoutToMove.y * 0.1f, 0f);
            staticBackground.position += new Vector3(amoutToMove.x , amoutToMove.y , 0f);
            middleBackground.position += new Vector3(amoutToMove.x * 0.5f, amoutToMove.y * 0.5f, 0f);
            lastPos = transform.position;
            
            
            if (transform.position != target.position)
            {
                Vector3 targetPos = target.position;
                //����x��,y��
                targetPos.x = Mathf.Clamp(targetPos.x, minPosition.x, maxPosition.x);
                targetPos.y = Mathf.Clamp(targetPos.y, minPosition.y, maxPosition.y);
                //����z��
                targetPos.z = transform.position.z;
                //���Բ�ֵ
                transform.position = Vector3.Lerp(transform.position, targetPos, smoothing);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCamPosLimit(Vector2 minPos,Vector2 maxPos)
    {
        minPosition = minPos;
        maxPosition = maxPos;
    }
}
