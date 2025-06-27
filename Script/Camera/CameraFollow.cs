using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ParallaxLayer
{
    public Transform layer;    // 这一层的 Transform
    [Range(0f, 1f)]
    public float factor;       // 视差系数：0 表示不动，1 表示和相机同速
}


/// <summary>
/// 相机跟随效果，当用户向上时延迟，向下掉落时锁定
/// 往左Idle时向左平移一段，向右Idle时向右平移，run时跟随
/// </summary>
public class CameraFollow : MonoBehaviour
{


    //private float smoothing;


    //public Transform staticBackground, farBackground, middleBackground; //静止不动的纯色背景，远景，中景  （分层卷动）
    private Vector3 lastPos; //最后一次相机位置
    private Animator myAnim;
    private Rigidbody2D myRigidbody;




    [Header("目标 & 边界")]
    public Transform target;
    public Transform minPosition, maxPosition;
    public float idleSmoothing; //比较慢，0.05正好
    public float elseSmoothing;//略快一些,0.12
    public float dashSmoothing = 0.3f; //更快，0.3
    public float idleOffset;

    private float _dashCameraOffsetX; // Dash期间相机与角色的水平偏移
    private bool _isDashFollowing;    // 是否处于Dash跟随模式

    [Header("Parallax Layers (远→近)")]
    public ParallaxLayer[] layers;    // 9 层
                                      // 3. 摄像机中心可移动范围
    private float minCamX;
    private float maxCamX;
    private float minCamY;
    private float maxCamY;


    [Header("场景边界配置")]
    public SceneBorder[] sceneBorders; // 所有场景边界配置

    private Dictionary<int, (Vector2 min, Vector2 max)> borderDict = new Dictionary<int, (Vector2, Vector2)>();
    private int currentBorderID = 1;
    //public float transitionSpeed; // 状态平滑过渡速度5f
    //下降时不需要平滑紧跟Player

    // Start is called before the first frame update
    void Start()
    {
        lastPos = transform.position; //相机初始位置
        //将GameController类的camShake参数，赋值为Tag为CameraShake物体的CameraShake脚本组件(为什么要在此赋值？)
        GameController.camShake = GameObject.FindGameObjectWithTag("CameraShake").GetComponent<CameraShake>();
        myAnim = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();              //获取动画组件
        myRigidbody = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
 


        // 初始化边界字典
        foreach (var border in sceneBorders)
        {
            if (border != null && border.minPos && border.maxPos)
            {
                borderDict[border.borderID] = (border.minPos.position, border.maxPos.position);
            }
        }
        UpdateCameraBounds();

    }

    void UpdateCameraBounds()
    {
        if (!borderDict.ContainsKey(currentBorderID))
        {
            Debug.LogError($"未找到ID为{currentBorderID}的边界配置！");
            return;
        }

        var (levelMin, levelMax) = borderDict[currentBorderID];

        // 计算摄像机边界（原有逻辑）
        float halfH = Camera.main.orthographicSize;
        float halfW = halfH * Camera.main.aspect;

        minCamX = levelMin.x + halfW;
        maxCamX = levelMax.x - halfW;
        minCamY = levelMin.y + halfH;
        maxCamY = levelMax.y - halfH;
    }
    // 新增方法：切换边界
    public void SwitchBorder(int newBorderID)
    {
        if (newBorderID == currentBorderID) return;

        currentBorderID = newBorderID;
        UpdateCameraBounds();

        // // 立即更新摄像机位置
        // Vector3 clampedPos = transform.position;
        // clampedPos.x = Mathf.Clamp(clampedPos.x, minCamX, maxCamX);
        // clampedPos.y = Mathf.Clamp(clampedPos.y, minCamY, maxCamY);
        // transform.position = clampedPos;
    }


    void FixedUpdate()
    {
      
        // 4. 视差滚动
        Vector3 camPos = transform.position;
        Vector3 delta = camPos - lastPos;
        foreach (var pl in layers)
            if (pl.layer != null)
                pl.layer.position += Vector3.right * (delta.x * pl.factor);
        lastPos = camPos;

        // 5. 跟随 & Clamp
        if (target == null) return;

        // 1. 角色原始位置
        Vector3 tgt = target.position;

        // 2. 计算 idleOffset
        float offsetX = myAnim.GetBool("isIdle") && !GameController.pos
                      ? -idleOffset
                      : idleOffset;

        // 3. 得到带偏移的“理想摄像机中心”
        Vector3 desiredPos = new Vector3(tgt.x + offsetX, tgt.y, transform.position.z);

        // 4. Clamp 到可见范围
        desiredPos.x = Mathf.Clamp(desiredPos.x, minCamX, maxCamX);
        desiredPos.y = Mathf.Clamp(desiredPos.y, minCamY, maxCamY);

        // 5. 根据状态选择插值速度
        float smooth = myAnim.GetBool("isDash") ? dashSmoothing
                     : (myAnim.GetBool("isIdle") ? idleSmoothing : elseSmoothing);

        // 6. 平滑移动到 desiredPos
        transform.position = Vector3.Lerp(transform.position, desiredPos, smooth);

    }

}

