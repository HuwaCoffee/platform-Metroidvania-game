using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public Transform target;
    public float smoothing;
    public Vector2 minPosition;
    public Vector2 maxPosition;

    public Transform staticBackground, farBackground, middleBackground; //静止不动的纯色背景，远景，中景  （分层卷动）
    private Vector2 lastPos; //最后一次相机位置

    // Start is called before the first frame update
    void Start()
    {
        lastPos = transform.position; //相机初始位置
        //将GameController类的camShake参数，赋值为Tag为CameraShake物体的CameraShake脚本组件
        GameController.camShake = GameObject.FindGameObjectWithTag("CameraShake").GetComponent<CameraShake>();
    }

    void FixedUpdate()
    {
        if (target != null)
        {
            //计算相机在上一帧和当前帧之间移动的距离（先试试，不行就用人物的移动距离，看看哪个好）
            Vector2 amoutToMove = new Vector2(transform.position.x - lastPos.x, transform.position.y - lastPos.y);

            //移动远景和中景
            farBackground.position += new Vector3(amoutToMove.x * 0.1f, amoutToMove.y * 0.1f, 0f);
            staticBackground.position += new Vector3(amoutToMove.x , amoutToMove.y , 0f);
            middleBackground.position += new Vector3(amoutToMove.x * 0.5f, amoutToMove.y * 0.5f, 0f);
            lastPos = transform.position;
            
            
            if (transform.position != target.position)
            {
                Vector3 targetPos = target.position;
                //限制x轴,y轴
                targetPos.x = Mathf.Clamp(targetPos.x, minPosition.x, maxPosition.x);
                targetPos.y = Mathf.Clamp(targetPos.y, minPosition.y, maxPosition.y);
                //限制z轴
                targetPos.z = transform.position.z;
                //线性差值
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
