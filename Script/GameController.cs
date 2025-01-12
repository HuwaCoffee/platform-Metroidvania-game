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

    
    
}
