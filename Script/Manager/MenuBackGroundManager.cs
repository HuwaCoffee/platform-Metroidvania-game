using UnityEngine;

public class MenuBackGroundManager : MonoBehaviour
{
    [Header("滚动参数")]
    public float scrollSpeed = 1f; // 基础滚动速度
    public bool horizontalScroll = true; // 横向滚动
    [Range(0, 1)]
    public float parallaxFactor = 0.5f; // 视差因子（0：静止，1：全速）

    private Transform[] images;
    private float imageWidth;//（2000左右）
    private float accumulatedDelta; // 累积位移量（等于1920时换位置）
    private Vector3 startPosition;

    void Start()
    {
        InitializeBackground();
    }

    void FixedUpdate()
    {
        UpdateScrollMovement();
        CheckBoundary();
    }

    void InitializeBackground()
    {
        images = new Transform[] { //获取两个图片的位置
            transform.GetChild(0),
            transform.GetChild(1)
        };

        SpriteRenderer sprite = images[0].GetComponent<SpriteRenderer>(); //精灵渲染器组件.bounds.size.x/.y可以获得图片的宽和高
        imageWidth = horizontalScroll ? 
            sprite.bounds.size.x : 
            sprite.bounds.size.y;

        // 初始化第二张图片位置
        Vector3 offset = horizontalScroll ? 
            Vector3.right * imageWidth : 
            Vector3.up * imageWidth;
        images[1].position = images[0].position + offset; //把1号图片右移一个width的距离

        startPosition = images[0].position; //0号图片初始位置
    }

    void UpdateScrollMovement()
    {
        // 计算基于时间的位移
        float speed = scrollSpeed * parallaxFactor;
        accumulatedDelta += speed * Time.deltaTime; //一帧时间*速度等于一帧位移量

        foreach (Transform img in images)
        {
            Vector3 newPos = startPosition;    //记录实时位移
            if (horizontalScroll)
            {
                newPos.x += accumulatedDelta +       
                           (img == images[0] ? 0 : imageWidth);
            }
            else
            {
                newPos.y += accumulatedDelta + 
                           (img == images[0] ? 0 : imageWidth);
            }
            img.position = newPos;          //将图片位移设置为newPos
        }
    }

    void CheckBoundary()
    {
        foreach (Transform img in images)
        {
            if (horizontalScroll)
            {
                // 当图片完全移出左侧屏幕时重置到右侧
                if (img.position.x + imageWidth/2 < GetScreenLeft())
                {
                    Vector3 newPos = img.position;
                    newPos.x += 2 * imageWidth;
                    img.position = newPos;
                }
                // 当图片完全移出右侧屏幕时重置到左侧
                else if (img.position.x - imageWidth/2 > GetScreenRight())
                {
                    Vector3 newPos = img.position;
                    newPos.x -= 2 * imageWidth;
                    img.position = newPos;
                }
            }
        }
    }

    // 获取屏幕边界（基于正交相机）
    float GetScreenLeft()
    {
        return Camera.main.ViewportToWorldPoint(Vector3.zero).x;
    }

    float GetScreenRight()
    {
        return Camera.main.ViewportToWorldPoint(Vector3.one).x;
    }
}