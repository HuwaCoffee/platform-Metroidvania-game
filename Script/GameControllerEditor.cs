using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControllerEditor : GameController
{
    public bool GameAlive = true;

   // public CameraShake Shake; //ÆÁÄ»¶¶¶¯

    public string Language = "English";

    public bool DoubleJump = false;

    public bool Glide = false;

    public bool Dash = false;

    public bool Magic = false;

    private void Update()
    {
        isGameAlive = GameAlive;
        currentLanguage = Language;
        canDoubleJump = DoubleJump;
        canGlide = Glide;
        canDash = Dash;
        canMagic = Magic;



    }


}
