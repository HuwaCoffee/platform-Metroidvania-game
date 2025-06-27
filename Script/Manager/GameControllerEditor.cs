using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControllerEditor : GameController
{
    public bool GameAlive = true;

   // public CameraShake Shake; //屏幕抖动

    //public string Language = "English";
    //Player Prefabs中存储当前场景号和坐标，和在静态类中村有什么区别？
    public bool DoubleJump = false;

    public bool Glide = false;

    public bool Dash = false;

    public bool Magic = false;

    private void Update()
    {
        isGameAlive = GameAlive;
        canDoubleJump = DoubleJump;
        canGlide = Glide;
        canDash = Dash;
        canMagic = Magic;



    }


}
