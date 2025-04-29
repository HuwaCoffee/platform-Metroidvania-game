using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 系统可能常用的静态功能，如屏幕抖动，场景转换，以及解锁能力
/// </summary>
public class GameController : MonoBehaviour
{
    
    public static bool isGameAlive = true;

    public static CameraShake camShake; //屏幕抖动

    public static string currentLanguage="English";

    public static bool canDoubleJump = false;

    public static bool canGlide = false;

    public static bool canDash = false;

    public static bool canMagic = false;

    public static bool canFlip = true;//一些形如法术，攻击，如果攻击特效有多帧，生成时限制角色不能转向，否则就会产生旋转刀的效果
    public static bool pos = false;//人物朝向，在一些常见会用（比如角色攻击时会锁定）
    public static bool canAttack=true; //初始不让攻击，到达第一个检查点才可以
}
