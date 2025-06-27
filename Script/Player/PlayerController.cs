using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 玩家控制类，实现玩家移动，跳跃，二段跳，冲刺，滑翔
/// </summary>
public class PlayerController : MonoBehaviour
{

        
        public static PlayerController Instance;
         [HideInInspector] public bool forcedRun = false;
 [HideInInspector] public Vector2 forcedDir = Vector2.zero;  // 强制跑的方向（单位向量）
public float forcedRunSpeed ;     // 强制跑时使用的速度
 

    public float runSpeed;            //最大max跑步速度

    public float accelerationTime;    // 加速所需的时间（帧）
    public float decelerationTime;    // 减速所需的时间（帧）
[Header("地面检测设置")]
[SerializeField] private float groundCheckRadius = 0.2f; // 检测半径
[SerializeField] private Vector2 groundCheckOffset = new Vector2(0, -0.1f); // 检测点偏移
[SerializeField] private LayerMask groundLayerMask; // 在Inspector中配置图层


    private float currentSpeed = 0f;       // 当前速度
    private float targetSpeed = 0f;        // 目标速度（根据输入设定）
    private float acceleration;            // 加速度（由加速时间推算）
    private float deceleration;            // 减速度（由减速时间推算）
    public float jumpSpeed;           //跳跃速度
    public float fallGravityRatio;    //向下掉落时倍增的重力

    public float bufferTime = 0.2f; // 输入缓冲时间（秒）
    private float jumpInputBufferTime = 0f; // 输入缓冲时间
    public float coyoteTime = 0.2f;  // 土狼时间
    private float coyoteTimeCounter = 0f;  // 土狼时间计时器

    public float maxFallSpeed;        //最大掉落速度

    public float hoverGravityScale;     // 滞空时的重力比例
    public float littleSpeed;            // 定义接近最高点的速度阈值

    private float doubleJumpTimer = 0f;  // 记录二段跳实时剩余冷却时间，为0时才能二段跳
    public float doubleJumpCooldown; //二段跳的冷却时间，表示从一段跳跃结束到二段跳开始之间的时间间隔，单位是秒。

    public float doubleJumpSpeed;     //二段跳（可以设置一个全局的static的bool，当用户触发某个机制，修改这个bool，获得二段跳能力）
    private BoxCollider2D myFeet;     //用于判断是否在地面                                        
    private Rigidbody2D myRigidbody;  //用rb修改人物位置
    private Animator myAnim;
    private bool isGround;
    private bool canDoubleJump;
    private bool isGlide = false;
    //private bool isFall = false;
    private float speed;//当前速度
    private bool isHovering = false;           // 是否处于滞空状态
    private float normalGravity;               //默认重力

    bool hasAlreadyDoubleJump = false;
    // Start is called before the first frame update

    //前两帧原地不到，第三帧扭曲特效，第四帧瞬移一段(1身高)，第五帧距离为（0.5身高），第六帧(0.5)，第七（0.5）
    //第八（0.25），第九（0.25），第十（0.1），第十一（0.1），十二（0.05），第十三回正动作，没有距离
    //总体距离3个身高，多一丢丢
    //第二帧的画面和第一帧一样，是冻结的，其余都正常
    [Header("Dash 设置")]
    public float playerHeight = 2f;
    public float dashDuration = 0.15f;   // 冲刺持续时间（秒）
    public float dashCooldown = 1f;      // （可选）冲刺后冷却时间
    public float dashDistance = 5f;      // 冲刺总位移（各方向一致）

    private bool isDashing = false;      // 正在冲刺中
    private bool cDash = GameController.canDash;         // 是否可发起冲刺
    private Vector2 dashDirection;       // 冲刺方向
    private int dashFrameCount;          // 冲刺持续的 FixedUpdate 帧数

    // 按用户提供的数据（单位：身高），索引 0～12 对应帧 1～13

    [SerializeField]
    private Transform cameraPos;
    [SerializeField]
    private CameraFollow cameraFollow;
    private readonly float[] dashDisplacements = new float[] {
    0f,    // 帧 1
    0f,    // 帧 2，此帧和上一帧一样
    0f,    // 帧 3
    1f,    // 帧 4：1 身高
    0.5f,  // 帧 5
    0.5f,  // 帧 6
    0.5f,  // 帧 7
    0.25f, // 帧 8
    0.25f, // 帧 9
    0.1f,  // 帧 10
    0.1f,  // 帧 11
    0.05f, // 帧 12
    0f     // 帧 13
};
    [Header("灰尘拖尾")]
    public ParticleSystem dust;
    public Vector2 centerOffset = new Vector2(0f, -0.6f);  // 正下方偏移（脚底）
    public Vector2 backOffset = new Vector2(0.38f, -0.6f);  // 偏后偏移（脚后跟）
    public bool ignoreInput = false;

void Awake()
    {
        // 单例初始化
        if (Instance == null) Instance = this;
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }}
    void Start()
    {

        // 确保图层掩码正确配置
    if (groundLayerMask == 0)
    {
        // 自动创建图层掩码
        groundLayerMask = (1 << LayerMask.NameToLayer("Ground"))
                       | (1 << LayerMask.NameToLayer("MovingPlatform"))
                       | (1 << LayerMask.NameToLayer("Item"));
    }

        myRigidbody = GetComponent<Rigidbody2D>();      //获取rb组件
        myAnim = GetComponent<Animator>();              //获取动画组件
        myFeet = GetComponent<BoxCollider2D>();         //获取碰撞体组件

        normalGravity = myRigidbody.gravityScale;      //记录初始重力，方便修改后返回原重力

        // 计算每帧的加速度（速度/时间）
        acceleration = runSpeed / accelerationTime;
        deceleration = runSpeed / decelerationTime;

        //这样，无论 FixedUpdate 的时间步长如何，都能保证冲刺约 0.15 秒。
        dashFrameCount = Mathf.CeilToInt(dashDuration / Time.fixedDeltaTime);


    }

    // Update is called once per frame
    void Update()
    {

         if (isDashing) return;
  // 如果 forcedRun，直接返回但**不阻断** FixedUpdate
  if (forcedRun) {
    // 这里可以把动画切换也放进 Update
    myAnim.SetBool("isRun", true);
    myAnim.SetBool("isIdle", false);
    return;
  }
  if (ignoreInput) {
    // 只跳过“玩家输入相关的”分支
  } else
        
        if (GameController.isGameAlive)
        {
            Run();
            if (GameController.canFlip)
                Flip();


            Jump();             //能否二段跳的判断内含在Jump中
            CheckGround();
            Dash();
            SwitchAnimation();
            Fall();//自己加的，实现高平台的坠落，在箱庭游戏比较有用，提高关卡立体程度，还没有动作
        }
    }

    void FixedUpdate() {
  if (forcedRun) {
    

     if (forcedDir.x > 0.1f)
            {
                if (GameController.pos == false)
                {
                    GameController.pos = !GameController.pos;
                }
                transform.localRotation = Quaternion.Euler(0, 180, 0);
            }
        
            if (forcedDir.x < -0.1f)
            {
                if (GameController.pos == true)
                {
                    GameController.pos = !GameController.pos;
                }
                transform.localRotation = Quaternion.Euler(0, 0, 0);
            }





  // 强制播放跑步动作
     

    // 起步
    myAnim.SetBool("isRun", true);
    // 真正循环
    myAnim.SetTrigger("isRunning");
    // 禁用 Idle
    myAnim.SetBool("isIdle", false);
   


    Debug.Log($"强制移动中 | 速度:{forcedRunSpeed} 方向:{forcedDir}");
    Vector2 delta = forcedDir * forcedRunSpeed * Time.fixedDeltaTime;
    myRigidbody.MovePosition(myRigidbody.position + delta);
  }
}


    void Dash()
    { Debug.Log("能否冲刺："+GameController.canDash);
        //Debug.Log("冲刺！");
        // 按键名可在 Input Manager 中设为 "Dash"（例如 LeftShift）
        if (!GameController.canDash) return;
        if ((Input.GetButtonDown("Dash") || Input.GetKeyDown(KeyCode.L)) && !isDashing && cDash)
        {
            // 原始离散方向，包含四正八向
            float ix = Input.GetAxisRaw("Horizontal");
            float iy = Input.GetAxisRaw("Vertical");

            // 地面上不允许向下冲刺
            if (isGround && iy < 0) iy = 0;

            dashDirection = new Vector2(ix, iy).normalized;
            if (dashDirection == Vector2.zero)
            {
                // 无输入时沿朝向冲刺
                dashDirection = GameController.pos ? Vector2.right : Vector2.left;
            }
            //动画切换至Dash
            //myAnim.SetBool("isIdle", false);

            StartCoroutine(DashCoroutine());
        }


    }

    // private IEnumerator DashCoroutine()
    // {
    //     // isDashing = true;
    //     // canDash = false;               // 消耗冲刺机会

    //     // Vector3 startPos = transform.position;
    //     // Vector3 endPos = startPos + (Vector3)dashDirection * dashDistance;
    //     // float elapsed = 0f;

    //     // // 插值平移：每帧更新位置
    //     // while (elapsed < dashDuration)
    //     // {
    //     //     float t = elapsed / dashDuration;
    //     //     transform.position = Vector3.Lerp(startPos, endPos, t);
    //     //     elapsed += Time.deltaTime;
    //     //     yield return null;         // 等待下一渲染帧 :contentReference[oaicite:5]{index=5}
    //     // }
    //     // // 确保精确结束点
    //     // transform.position = endPos;

    //     // // 结束冲刺，恢复控制
    //     // isDashing = false;


    //     isDashing = true;
    //     canDash = false;

    //     // 记录冲刺方向（已归一化）
    //     Vector2 dir = dashDirection.normalized;
    //     // Unity 文档：MovePosition 在下一次物理更新中瞬移刚体，不受重力影响 :contentReference[oaicite:0]{index=0}

    //     for (int i = 0; i < dashDisplacements.Length; i++)
    //     {
    //         // 计算本帧位移向量
    //         Vector2 delta = dir * (dashDisplacements[i] * playerHeight);

    //         // 按物理帧移动刚体——每次调用会在 FixedUpdate 时生效 :contentReference[oaicite:1]{index=1}
    //         myRigidbody.MovePosition(myRigidbody.position + delta);

    //          // 2) 同步移动摄像机
    //         cameraPos.position += (Vector3)delta;

    //            // 3) 同步 lastPos，视差不再重复这段 delta
    //         cameraFollow.lastPos = cameraPos.position;

    //         // 等待下一次物理帧，提高同步准确度 :contentReference[oaicite:2]{index=2}
    //         yield return new WaitForFixedUpdate();
    //     }

    //     // 冲刺结束，恢复控制
    //     isDashing = false;
    //     myRigidbody.gravityScale = normalGravity;
    //     //动画切换至Dash
    //     myAnim.SetBool("isDash", false);

    // }

    private IEnumerator DashCoroutine()
    {
        myAnim.SetBool("isDash", true);
        isDashing = true;
        cDash = false;
        // 新增：清除当前速度
        myRigidbody.velocity = Vector2.zero;
        Vector2 dir = dashDirection.normalized;
        Vector3 previousCamPos = cameraPos.position; // 记录冲刺前的相机位置

        for (int i = 0; i < dashDisplacements.Length; i++)
        {
            // 计算本帧位移
            Vector2 delta = dir * (dashDisplacements[i] * playerHeight);

            // 移动角色
            myRigidbody.MovePosition(myRigidbody.position + delta);

            // 计算相机实际位移（考虑相机边界限制）
            // Vector3 newCamPos = cameraPos.position + (Vector3)delta;
            // newCamPos.x = Mathf.Clamp(newCamPos.x, cameraFollow.minPosition.x, cameraFollow.maxPosition.x);
            // newCamPos.y = Mathf.Clamp(newCamPos.y, cameraFollow.minPosition.y, cameraFollow.maxPosition.y);

            // 应用视差效果
            //Vector3 actualCamDelta = newCamPos - cameraPos.position;
            //cameraFollow.ApplyParallaxDelta(actualCamDelta);

            // 更新相机位置
            //cameraPos.position = newCamPos;

            yield return new WaitForFixedUpdate();
        }

        isDashing = false;
        myRigidbody.gravityScale = normalGravity;
        myAnim.SetBool("isDash", false);
    }

    void Fall()
    {

        bool playerIsFall = myRigidbody.velocity.y < 0;
        //限制在接近最高点处的重量，使得无论上升还是下降，有一定的滞空感，更容易在游戏中反映
        // 检查是否接近最高点（滞空状态）
        if (!isGlide)
        {
            if (Mathf.Abs(myRigidbody.velocity.y) < littleSpeed)
            {
                if (!isHovering) // 状态切换为滞空
                {
                    isHovering = true;
                    myRigidbody.gravityScale = hoverGravityScale; // 设置滞空重力
                }
            }
            else
            {
                if (isHovering) // 状态切换为非滞空
                {
                    isHovering = false;

                    // 根据垂直速度决定重力恢复到哪种状态
                    if (myRigidbody.velocity.y > 0)
                    {
                        myRigidbody.gravityScale = normalGravity; // 上升阶段
                    }
                    else
                    {
                        //这里是增大下落重力，但由于手感不好，先注释掉
                        myRigidbody.gravityScale = normalGravity * fallGravityRatio; // 下落阶段
                                                                                     //myRigidbody.gravityScale = normalGravity;
                    }
                }
            }
        }


        // 限制最大下落速度
        if (playerIsFall && myRigidbody.velocity.y < -maxFallSpeed)
        {
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, -maxFallSpeed);
        }
    }
    void CheckGround()
    {
       // 计算检测点位置（角色脚部 + 偏移）
    Vector2 checkPosition = (Vector2)myFeet.transform.position + groundCheckOffset;
    
    // 使用更可靠的Physics2D.OverlapCircle
    isGround = Physics2D.OverlapCircle(checkPosition, groundCheckRadius, groundLayerMask);
    
    // 高级调试可视化
    Debug.DrawRay(checkPosition, Vector2.down * groundCheckRadius, 
                 isGround ? Color.green : Color.red, 0.1f);
    
    
    
    if (isGround)
    {
        cDash = true;
        hasAlreadyDoubleJump = false;
        Debug.Log($"地面检测成功! 位置: {checkPosition}");
    }
    else
    {
        Debug.LogWarning($"地面检测失败! 图层掩码: {groundLayerMask} 位置: {checkPosition}");
    }
    }
    void Flip()
    {
        bool playerHasXAxisSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        if (playerHasXAxisSpeed)
        {
            Vector2 offset;
            //Debug.Log(myRigidbody.velocity.x);
            if (myRigidbody.velocity.x > 0.1f)
            {
                if (GameController.pos == false)
                {
                    GameController.pos = !GameController.pos;
                    if (isGround)
                    {
                        offset = backOffset; // 向左时，粒子在右后方
                        // 设置粒子系统位置为玩家位置 + 偏移
                        dust.transform.position = (Vector2)transform.position + offset;
                        dust.Play();
                    }

                }
                transform.localRotation = Quaternion.Euler(0, 180, 0);
            }
            if (myRigidbody.velocity.x < -0.1f)
            {
                if (GameController.pos == true)
                {
                    GameController.pos = !GameController.pos;
                    if (isGround)
                    {
                        offset = new Vector2(-backOffset.x, backOffset.y); // 向右时，粒子在左后方

                        // 设置粒子系统位置为玩家位置 + 偏移
                        dust.transform.position = (Vector2)transform.position + offset;
                        dust.Play();
                    }

                }
                transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }
    void Run()
    {
        /* //直接通过水平Axis的值将速度加到runSpeed，这种方式不可控，使用的是unity自带的Horizontal属性，无法自己控制速度变化快慢
         float moveDir = Input.GetAxis("Horizontal");//(-1到1之间)
         Vector2 playerVel = new Vector2(moveDir * runSpeed, myRigidbody.velocity.y);
         //修改碰撞体x值
         myRigidbody.velocity = playerVel;

         //动画的bool赋值和跳转
         bool playerHasXAxisSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
         myAnim.SetBool("isRun", playerHasXAxisSpeed);*/

        float moveDir = Input.GetAxis("Horizontal"); // (-1 to 1)

        // 设定目标速度
        if (moveDir != 0)
        {

            targetSpeed = Mathf.Sign(moveDir) * runSpeed;
        }
        //松手即目标速度为0
        if (!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A)) { Debug.Log("松手"); targetSpeed = 0; }
        //转向保持原速，不然一直adad手感会奇怪
        if (Mathf.Sign(currentSpeed) != Mathf.Sign(currentSpeed)) { currentSpeed *= -1; return; }
        // Debug.Log(currentSpeed);

        // 如果有输入（即想要加速），则逐步增加速度
        if (currentSpeed != targetSpeed)
        {
            if (Mathf.Abs(targetSpeed) > Mathf.Abs(currentSpeed))
            {
                // 加速
                currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration);
                // Debug.Log(Time.deltaTime);
            }
            else
            {
                // 减速
                currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, deceleration);
            }
        }

        // 更新角色的水平速度
        Vector2 playerVel = new Vector2(currentSpeed, myRigidbody.velocity.y);
        myRigidbody.velocity = playerVel;

        // 设置动画的跑步状态
        bool playerHasXAxisSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        myAnim.SetBool("isRun", playerHasXAxisSpeed);
        myAnim.SetBool("isIdle", false);
    }




    void Jump()
    {

        if (Input.GetButtonDown("Jump"))  //第一段跳
        {

            // 在按键时启动输入缓冲
            jumpInputBufferTime = bufferTime;

        }
        if (jumpInputBufferTime > 0.01f)
        {
            if (isGround || coyoteTimeCounter > 0)
            {
                myRigidbody.gravityScale = normalGravity;       //恢复正常重力(防止土狼跳时由于下落重力而跳不高)
                                                                //人物向上跳跃，相当于带着水平速度向上跳，如果把x设置为0.0f，就是垂直上跳
                Vector2 jumpVel = new Vector2(myRigidbody.velocity.x, jumpSpeed);
                myRigidbody.velocity = jumpVel;

                //动画切换至jump
                myAnim.SetBool("isJump", true);
                dust.transform.position = (Vector2)transform.position + centerOffset;
                dust.Play();
                //将二段跳设置为true
                canDoubleJump = true;

                // 重置二段跳冷却计时器和土狼计时器
                doubleJumpTimer = doubleJumpCooldown;
                coyoteTimeCounter = 0;
                jumpInputBufferTime = 0f;// 触发后立即清除缓冲
            }
            else if (GameController.canDoubleJump)     //二段跳
            {

                //特殊考虑，从Ground上走下，再跳跃触发二段跳（目前动画还没有切换）
                if (hasAlreadyDoubleJump == false && !canDoubleJump)
                {
                    //代替第一段跳跃，完成设置canDoubleJump的功能
                    canDoubleJump = true;
                    hasAlreadyDoubleJump = true;
                    // 重置二段跳冷却计时器
                    doubleJumpTimer = -1;
                }
                if (canDoubleJump && doubleJumpTimer < 0)
                {
                    myAnim.SetBool("isDoubleJump", true);
                    myRigidbody.gravityScale = normalGravity;       //恢复正常重力

                    Vector2 doubleJumpVel = new Vector2(myRigidbody.velocity.x, doubleJumpSpeed);
                    myRigidbody.velocity = doubleJumpVel;
                    hasAlreadyDoubleJump = true;
                    canDoubleJump = false;

                }
                jumpInputBufferTime = 0f; // 触发后立即清除缓冲

            }




        }

        // 检测跳跃键的松开状态，决定跳跃高度
        if (Input.GetButtonUp("Jump") && myRigidbody.velocity.y > 0)
        {
            // 如果松开跳跃键，减少向上的速度，模拟短跳
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, myRigidbody.velocity.y * 0.5f);
        }
        // 更新土狼时间
        if (!isGround)
        {
            coyoteTimeCounter -= Time.deltaTime;  // 只在不在地面时减少土狼时间计时器
        }
        else
        {

            coyoteTimeCounter = coyoteTime;  // 玩家在地面时重置土狼时间

        }

        // Debug.Log(coyoteTime);
        // 更新二段跳冷却计时器

        //Debug.Log(doubleJumpTimer);
        doubleJumpTimer -= Time.deltaTime;

        //更新缓冲时间
        jumpInputBufferTime -= Time.deltaTime;
    }
    void Glide()
    {
        //能够二段跳时才进行滑翔
        if (GameController.canGlide)
        {
            speed = Vector3.Magnitude(myRigidbody.velocity);
            if (Input.GetButton("Jump") && isGlide == false)
            {
                Debug.Log(speed);
                /*if (speed >= maxSpeed)
                {
                    myRigidbody.AddForce(500 * Vector3.up);
                }*/

                myRigidbody.gravityScale *= 0.25f;
                isGlide = true;
            }
        }

    }
    void SwitchAnimation()
    {

        //当起步动画结束后自动切换为跑步动画
        AnimatorStateInfo stateInfo = myAnim.GetCurrentAnimatorStateInfo(0);
        // 检查是否为起步动画并且其normalizedTime >= 1.0
        if (stateInfo.IsName("Run") && stateInfo.normalizedTime >= 1.0f)
        {
            // 动画已经结束，现在可以进行转换或其他操作
            if (!stateInfo.IsName("Running"))
            {
                myAnim.SetBool("isRunning", true);
                Debug.Log("起步动画已结束，切换至奔跑动画");
            }
        }


        if (((!Input.GetButton("Jump")) || isGround) && isGlide == true)
        {
            myRigidbody.gravityScale = 2;
            isGlide = false;
        }
        //myAnim.SetBool("isIdle", false);
        if (myAnim.GetBool("isJump"))
        {
            //myRigidbody的向量可以理解为身体的运动趋势，如果为正，则向正方向移动，为负则向负方向，为0则静止，
            //而translation的向量是当前身体的坐标

            //fall或者doublefall时，长按空格可以滑翔，即降低重力为原来的0.5倍,播放fall的动画
            if (myRigidbody.velocity.y < -0.01f)
            {

                Glide();
                myAnim.SetBool("isJump", false);
                myAnim.SetBool("isFall", true);
            }
        }

        if (!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A) && !myAnim.GetBool("isDash")) myAnim.SetBool("isIdle", true);

        if (!isGround && myRigidbody.velocity.y < -0.5f) // 加入地面判断和更高的速度阈值
        {
            myAnim.SetBool("isFall", true);
        }
        else if (isGround && myAnim.GetBool("isFall")) // 当角色落地时清除 isFall 状态
        {
            myAnim.SetBool("isFall", false);
            if (Mathf.Abs(Input.GetAxis("Horizontal")) < 0.1f)
            {
                myAnim.SetBool("isIdle", true);
            }
        }

        // if (myAnim.GetBool("isDoubleJump"))
        // {
        //     //myRigidbody的向量可以理解为身体的运动趋势，如果为正，则向正方向移动，为负则向负方向，为0则静止，
        //     //而translation的向量是当前身体的坐标
        //     if (myRigidbody.velocity.y < -0.1f)
        //     {
        //         Glide();
        //         myAnim.SetBool("isDoubleJump", false);
        //         myAnim.SetBool("isDoubleFall", true);
        //     }
        // }
        // if (myAnim.GetBool("isDoubleFall"))
        // {
        //     Glide();
        //     if (isGround)
        //     {
        //         myAnim.SetBool("isDoubleFall", false);
        //         myAnim.SetBool("isIdle", true);
        //     }
        // }
    }
}

