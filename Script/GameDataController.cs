using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 数据管理类，如攻击伤害，法术伤害，生命值，蓝量，如果增加肉鸽的几细胞玩法，还可设置小怪的血量，攻击招式                                                                                                            
/// </summary>
public class GameDataController : MonoBehaviour
{
    public static int playerAttackDamage=5; //角色攻击伤害5,9,13,17,21
    public static int hp=3; //血量(一碰就死（蔚蓝，以及玩的画画的新游戏），与空洞骑士之间，类似茶杯头)3,4
    public static int mp; //蓝量
    public static int magicDamage;
}
