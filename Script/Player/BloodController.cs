using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 利用代码控制帧动画，解决在UI中动画器不生效的问题
/// </summary>
public class BloodController : MonoBehaviour
{
    public Image image; // 绑定 UI Image
    public Sprite[] frames; // 帧动画的图片数组
    public float frameRate; // 每帧的时间

    private int currentFrame;
    private float timer;

    void Update()
    {
        if (frames.Length == 0) return;

        timer += Time.deltaTime;
        if (timer >= frameRate)
        {
            timer = 0;
            currentFrame = (currentFrame + 1) % frames.Length;
            image.sprite = frames[currentFrame];
        }
    }

 

}
