using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace xyk.platform_Metroidvania
{
    /// <summary>
    /// ��ҿ����࣬ʵ������ƶ�����Ծ������������̣�����
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        public float runSpeed;            //���max�ܲ��ٶ�

        public float accelerationTime;    // ���������ʱ�䣨֡��
        public float decelerationTime;    // ���������ʱ�䣨֡��

        private float currentSpeed = 0f;       // ��ǰ�ٶ�
        private float targetSpeed = 0f;        // Ŀ���ٶȣ����������趨��
        private float acceleration;            // ���ٶȣ��ɼ���ʱ�����㣩
        private float deceleration;            // ���ٶȣ��ɼ���ʱ�����㣩




        public float jumpSpeed;           //��Ծ�ٶ�
        public float fallGravityRatio;    //���µ���ʱ����������

        public float bufferTime = 0.2f; // ���뻺��ʱ�䣨�룩
        private float jumpInputBufferTime = 0f; // ���뻺��ʱ��
        public float coyoteTime = 0.2f;  // ����ʱ��
        private float coyoteTimeCounter = 0f;  // ����ʱ���ʱ��

        public float maxFallSpeed;        //�������ٶ�

        public float hoverGravityScale;     // �Ϳ�ʱ����������
        public float littleSpeed;            // ����ӽ���ߵ���ٶ���ֵ

        private float doubleJumpTimer = 0f;  // ��¼������ʵʱʣ����ȴʱ�䣬Ϊ0ʱ���ܶ�����
        public float doubleJumpCooldown; //����������ȴʱ�䣬��ʾ��һ����Ծ��������������ʼ֮���ʱ��������λ���롣

        public float doubleJumpSpeed;     //����������������һ��ȫ�ֵ�static��bool�����û�����ĳ�����ƣ��޸����bool����ö�����������
        private BoxCollider2D myFeet;     //�����ж��Ƿ��ڵ���                                        
        private Rigidbody2D myRigidbody;  //��rb�޸�����λ��
        private Animator myAnim;
        private bool isGround;
        private bool canDoubleJump;
        private bool isGlide = false;
        private bool isFall = false;
        private float speed;//��ǰ�ٶ�
        private bool isHovering = false;           // �Ƿ����Ϳ�״̬
        private float normalGravity;               //Ĭ������

       

        bool hasAlreadyDoubleJump = false;
        // Start is called before the first frame update
        void Start()
        {

            myRigidbody = GetComponent<Rigidbody2D>();      //��ȡrb���
            myAnim = GetComponent<Animator>();              //��ȡ�������
            myFeet = GetComponent<BoxCollider2D>();         //��ȡ��ײ�����

            normalGravity = myRigidbody.gravityScale;      //��¼��ʼ�����������޸ĺ󷵻�ԭ����

            // ����ÿ֡�ļ��ٶȣ��ٶ�/ʱ�䣩
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


                Jump();             //�ܷ���������ж��ں���Jump��
                CheckGround();
                SwitchAnimation();
                Fall();//�Լ��ӵģ�ʵ�ָ�ƽ̨��׹�䣬����ͥ��Ϸ�Ƚ����ã���߹ؿ�����̶ȣ���û�ж���
            }
        }

        void Fall()
        {
            bool playerIsFall = myRigidbody.velocity.y < 0;
            //�����ڽӽ���ߵ㴦��������ʹ���������������½�����һ�����ͿոУ�����������Ϸ�з�ӳ
            // ����Ƿ�ӽ���ߵ㣨�Ϳ�״̬��
            if (!isGlide)
            {
                if (Mathf.Abs(myRigidbody.velocity.y) < littleSpeed)
                {
                    if (!isHovering) // ״̬�л�Ϊ�Ϳ�
                    {
                        isHovering = true;
                        myRigidbody.gravityScale = hoverGravityScale; // �����Ϳ�����
                    }
                }
                else
                {
                    if (isHovering) // ״̬�л�Ϊ���Ϳ�
                    {
                        isHovering = false;

                        // ���ݴ�ֱ�ٶȾ��������ָ�������״̬
                        if (myRigidbody.velocity.y > 0)
                        {
                            myRigidbody.gravityScale = normalGravity; // �����׶�
                        }
                        else
                        {
                            //���������������������������ָв��ã���ע�͵�
                            myRigidbody.gravityScale = normalGravity * fallGravityRatio; // ����׶�
                            //myRigidbody.gravityScale = normalGravity;
                        }
                    }
                }
            }


            // ������������ٶ�
            if (playerIsFall && myRigidbody.velocity.y < -maxFallSpeed)
            {
                myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, -maxFallSpeed);
            }
        }
        void CheckGround()
        {
            //myFeet�����Ƿ�Ӵ���Ground��
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
            /* //ֱ��ͨ��ˮƽAxis��ֵ���ٶȼӵ�runSpeed�����ַ�ʽ���ɿأ�ʹ�õ���unity�Դ���Horizontal���ԣ��޷��Լ������ٶȱ仯����
             float moveDir = Input.GetAxis("Horizontal");//(-1��1֮��)
             Vector2 playerVel = new Vector2(moveDir * runSpeed, myRigidbody.velocity.y);
             //�޸���ײ��xֵ
             myRigidbody.velocity = playerVel;

             //������bool��ֵ����ת
             bool playerHasXAxisSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
             myAnim.SetBool("isRun", playerHasXAxisSpeed);*/

            float moveDir = Input.GetAxis("Horizontal"); // (-1 to 1)

            // �趨Ŀ���ٶ�
            if (moveDir != 0)
            {

                targetSpeed = Mathf.Sign(moveDir) * runSpeed;
            }
            //���ּ�Ŀ���ٶ�Ϊ0
            if (!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A)){ Debug.Log("����"); targetSpeed = 0; }
            //ת�򱣳�ԭ�٣���Ȼһֱadad�ָл����
            if (Mathf.Sign(currentSpeed) != Mathf.Sign(currentSpeed)) { currentSpeed *= -1; return ; }
           // Debug.Log(currentSpeed);

            // ��������루����Ҫ���٣������������ٶ�
            if (currentSpeed != targetSpeed)
            {
                if (Mathf.Abs(targetSpeed) > Mathf.Abs(currentSpeed))
                {
                    // ����
                    currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration);
                   // Debug.Log(Time.deltaTime);
                }
                else
                {
                    // ����
                    currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, deceleration);
                }
            }

            // ���½�ɫ��ˮƽ�ٶ�
            Vector2 playerVel = new Vector2(currentSpeed, myRigidbody.velocity.y);
            myRigidbody.velocity = playerVel;

            // ���ö������ܲ�״̬
            bool playerHasXAxisSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
            myAnim.SetBool("isRun", playerHasXAxisSpeed);
            myAnim.SetBool("isIdle", false);
        }




        void Jump()
        {

            if (Input.GetButtonDown("Jump"))  //��һ����
            {

                // �ڰ���ʱ�������뻺��
                jumpInputBufferTime = bufferTime;

            }
            if (jumpInputBufferTime > 0.01f)
            {
                if (isGround || coyoteTimeCounter > 0)
                {
                    myRigidbody.gravityScale = normalGravity;       //�ָ���������(��ֹ������ʱ��������������������)
                    //����������Ծ���൱�ڴ���ˮƽ�ٶ��������������x����Ϊ0.0f�����Ǵ�ֱ����
                    Vector2 jumpVel = new Vector2(myRigidbody.velocity.x, jumpSpeed);
                    myRigidbody.velocity = jumpVel;

                    //�����л���jump
                    myAnim.SetBool("isJump", true);

                    //������������Ϊtrue
                    canDoubleJump = true;

                    // ���ö�������ȴ��ʱ�������Ǽ�ʱ��
                    doubleJumpTimer = doubleJumpCooldown;
                    coyoteTimeCounter = 0;
                }
                else if (GameController.canDoubleJump)     //������
                {

                    //���⿼�ǣ���Ground�����£�����Ծ������������Ŀǰ������û���л���
                    if (hasAlreadyDoubleJump == false && !canDoubleJump)
                    {
                        //�����һ����Ծ���������canDoubleJump�Ĺ���
                        canDoubleJump = true;
                        hasAlreadyDoubleJump = true;
                        // ���ö�������ȴ��ʱ��
                        doubleJumpTimer = -1;
                    }
                    if (canDoubleJump && doubleJumpTimer < 0)
                    {
                        myAnim.SetBool("isDoubleJump", true);
                        myRigidbody.gravityScale = normalGravity;       //�ָ���������

                        Vector2 doubleJumpVel = new Vector2(myRigidbody.velocity.x, doubleJumpSpeed);
                        myRigidbody.velocity = doubleJumpVel;
                        hasAlreadyDoubleJump = true;
                        canDoubleJump = false;

                    }

                }




            }

            // �����Ծ�����ɿ�״̬��������Ծ�߶�
            if (Input.GetButtonUp("Jump") && myRigidbody.velocity.y > 0)
            {
                // ����ɿ���Ծ�����������ϵ��ٶȣ�ģ�����
                myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, myRigidbody.velocity.y * 0.5f);
            }
            // ��������ʱ��
            if (!isGround)
            {
                coyoteTimeCounter -= Time.deltaTime;  // ֻ�ڲ��ڵ���ʱ��������ʱ���ʱ��
            }
            else
            {

                coyoteTimeCounter = coyoteTime;  // ����ڵ���ʱ��������ʱ��

            }

           // Debug.Log(coyoteTime);
            // ���¶�������ȴ��ʱ��

            //Debug.Log(doubleJumpTimer);
            doubleJumpTimer -= Time.deltaTime;

            //���»���ʱ��
            jumpInputBufferTime -= Time.deltaTime;
        }
        void Glide()
        {
            //�ܹ�������ʱ�Ž��л���
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

            //���𲽶����������Զ��л�Ϊ�ܲ�����
            AnimatorStateInfo stateInfo = myAnim.GetCurrentAnimatorStateInfo(0);
            // ����Ƿ�Ϊ�𲽶���������normalizedTime >= 1.0
            if (stateInfo.IsName("Run") && stateInfo.normalizedTime >= 1.0f)
            {
                // �����Ѿ����������ڿ��Խ���ת������������
                if (!stateInfo.IsName("Running"))
                {
                    myAnim.SetBool("isRunning", true);
                    Debug.Log("�𲽶����ѽ������л������ܶ���");
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
                //myRigidbody�������������Ϊ������˶����ƣ����Ϊ���������������ƶ���Ϊ�����򸺷���Ϊ0��ֹ��
                //��translation�������ǵ�ǰ���������

                //fall����doublefallʱ�������ո���Ի��裬����������Ϊԭ����0.5��,����fall�Ķ���
                if (myRigidbody.velocity.y < -0.01f)
                {

                    Glide();
                    myAnim.SetBool("isJump", false);
                    myAnim.SetBool("isFall", true);
                }
            }
           
            if(!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A)) myAnim.SetBool("isIdle", true);

            if (!isGround && myRigidbody.velocity.y < -0.5f) // ��������жϺ͸��ߵ��ٶ���ֵ
            {
                myAnim.SetBool("isFall", true);
            }
            else if (isGround && myAnim.GetBool("isFall")) // ����ɫ���ʱ��� isFall ״̬
            {
                myAnim.SetBool("isFall", false);
                if (Mathf.Abs(Input.GetAxis("Horizontal")) <0.1f)
                {
                    myAnim.SetBool("isIdle", true);
                }
            }

            if (myAnim.GetBool("isDoubleJump"))
            {
                //myRigidbody�������������Ϊ������˶����ƣ����Ϊ���������������ƶ���Ϊ�����򸺷���Ϊ0��ֹ��
                //��translation�������ǵ�ǰ���������
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
