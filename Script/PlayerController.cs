using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace xyk.platform_Metroidvania
{
    /// <summary>
    /// 玩家控制类，实现玩家移动，跳跃，二段跳，冲刺，滑翔
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        public float runSpeed;            //最大max跑步速度

        public float accelerationTime;    // 加速所需的时间（帧）
        public float decelerationTime;    // 减速所需的时间（帧）

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
        private bool isFall = false;
        private float speed;//当前速度
        private bool isHovering = false;           // 是否处于滞空状态
        private float normalGravity;               //默认重力

       

        bool hasAlreadyDoubleJump = false;
        // Start is called before the first frame update
        void Start()
        {

            myRigidbody = GetComponent<Rigidbody2D>();      //获取rb组件
            myAnim = GetComponent<Animator>();              //获取动画组件
            myFeet = GetComponent<BoxCollider2D>();         //获取碰撞体组件

            normalGravity = myRigidbody.gravityScale;      //记录初始重力，方便修改后返回原重力

            // 计算每帧的加速度（速度/时间）
            acceleration = runSpeed / accelerationTime;
            deceleration = runSpeed / decelerationTime;

        }

        // Update is called once per frame
        void Update()
        {

            if (GameController.isGameAlive)
            {
                Run();

                Flip();


                Jump();             //能否二段跳的判断内含在Jump中
                CheckGround();
                SwitchAnimation();
                Fall();//自己加的，实现高平台的坠落，在箱庭游戏比较有用，提高关卡立体程度，还没有动作
            }
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
            //myFeet物体是否接触到Ground层
            /*isGround = myFeet.IsTouchingLayers(LayerMask.GetMask("Ground")) ||
                       myFeet.IsTouchingLayers(LayerMask.GetMask("MovingPlatform")) ||
                       myFeet.IsTouchingLayers(LayerMask.GetMask("Item"));*/
            isGround = myFeet.IsTouchingLayers(LayerMask.GetMask("Ground", "MovingPlatform", "Item"));
            if (isGround) hasAlreadyDoubleJump = false;

            if (isGround) hasAlreadyDoubleJump = false;
            //Debug.Log(isGround);
        }
        void Flip()
        {
            bool playerHasXAxisSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
            if (playerHasXAxisSpeed)
            {
                //Debug.Log(myRigidbody.velocity.x);
                if (myRigidbody.velocity.x > 0.1f)
                {
                    transform.localRotation = Quaternion.Euler(0, 180, 0);
                }
                if (myRigidbody.velocity.x < -0.1f)
                {
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
            if (!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A)){ Debug.Log("松手"); targetSpeed = 0; }
            //转向保持原速，不然一直adad手感会奇怪
            if (Mathf.Sign(currentSpeed) != Mathf.Sign(currentSpeed)) { currentSpeed *= -1; return ; }
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

                    //将二段跳设置为true
                    canDoubleJump = true;

                    // 重置二段跳冷却计时器和土狼计时器
                    doubleJumpTimer = doubleJumpCooldown;
                    coyoteTimeCounter = 0;
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
           
            if(!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A)) myAnim.SetBool("isIdle", true);

            if (!isGround && myRigidbody.velocity.y < -0.5f) // 加入地面判断和更高的速度阈值
            {
                myAnim.SetBool("isFall", true);
            }
            else if (isGround && myAnim.GetBool("isFall")) // 当角色落地时清除 isFall 状态
            {
                myAnim.SetBool("isFall", false);
                if (Mathf.Abs(Input.GetAxis("Horizontal")) <0.1f)
                {
                    myAnim.SetBool("isIdle", true);
                }
            }

            if (myAnim.GetBool("isDoubleJump"))
            {
                //myRigidbody的向量可以理解为身体的运动趋势，如果为正，则向正方向移动，为负则向负方向，为0则静止，
                //而translation的向量是当前身体的坐标
                if (myRigidbody.velocity.y < -0.1f)
                {
                    Glide();
                    myAnim.SetBool("isDoubleJump", false);
                    myAnim.SetBool("isDoubleFall", true);
                }
            }
            if (myAnim.GetBool("isDoubleFall"))
            {
                Glide();
                if (isGround)
                {
                    myAnim.SetBool("isDoubleFall", false);
                    myAnim.SetBool("isIdle", true);
                }
            }
        }
    }
}
