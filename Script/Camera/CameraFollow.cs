using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 相机跟随效果，当用户向上时延迟，向下掉落时锁定
/// 往左Idle时向左平移一段，向右Idle时向右平移，run时跟随
/// </summary>
public class CameraFollow : MonoBehaviour
{

    public Transform target;
    private float smoothing;
    public Vector2 minPosition;
    public Vector2 maxPosition;  //可以将最大最小值点设置为List，且在地图中设置多个触发点，每个触发点对应一个最大最小值
    public float idleOffset;

    public Transform staticBackground, farBackground, middleBackground; //静止不动的纯色背景，远景，中景  （分层卷动）
    private Vector2 lastPos; //最后一次相机位置
    private Animator myAnim;
    private Rigidbody2D myRigidbody;

    public float idleSmoothing; //比较慢，0.05正好
    public float elseSmoothing;//略快一些,0.12
   
    
    //public float transitionSpeed; // 状态平滑过渡速度5f
    //下降时不需要平滑紧跟Player

    // Start is called before the first frame update
    void Start()
    {
        lastPos = transform.position; //相机初始位置
        //将GameController类的camShake参数，赋值为Tag为CameraShake物体的CameraShake脚本组件
        GameController.camShake = GameObject.FindGameObjectWithTag("CameraShake").GetComponent<CameraShake>();   
        myAnim = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();              //获取动画组件
        myRigidbody = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
       
    }

    void FixedUpdate()
    {
        if (target != null)
        {
            //计算相机在上一帧和当前帧之间移动的距离（先试试，不行就用人物的移动距离，看看哪个好）
            Vector2 amoutToMove = new Vector2(transform.position.x - lastPos.x, transform.position.y - lastPos.y);

            //移动远景和中景
            farBackground.position += new Vector3(amoutToMove.x * 0.1f, amoutToMove.y * 0.1f, 0f);
            staticBackground.position += new Vector3(amoutToMove.x, amoutToMove.y, 0f);            //由于背景跟随的是相机，但是相机的y轴不动，所以背景y轴也不动
            middleBackground.position += new Vector3(amoutToMove.x * 0.5f, amoutToMove.y * 0.5f, 0f);
            lastPos = transform.position;


            //移动相机follow
            if (transform.position != target.position)
            {
                Vector3 targetPos = target.position;
                //限制x轴,y轴
                targetPos.x = Mathf.Clamp(targetPos.x, minPosition.x, maxPosition.x);
                targetPos.y = Mathf.Clamp(targetPos.y, minPosition.y, maxPosition.y);
                //限制z轴
                targetPos.z = transform.position.z;
                //线性差值
                if (myAnim.GetBool("isIdle"))
                {
                    if (GameController.pos == false) { 
                        targetPos.x = Mathf.Clamp(targetPos.x -= idleOffset, minPosition.x, maxPosition.x);
                    }
                    else { //targetPos.x += idleOffset; 
                        targetPos.x = Mathf.Clamp(targetPos.x += idleOffset, minPosition.x, maxPosition.x);
                    }
                  
                    smoothing = idleSmoothing;


                }
                else
                {
                    smoothing = elseSmoothing;
                }

                transform.position = Vector3.Lerp(transform.position, targetPos, smoothing);


            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetCamPosLimit(Vector2 minPos, Vector2 maxPos)
    {
        minPosition = minPos;
        maxPosition = maxPos;
    }
}

