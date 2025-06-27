using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("角色属性")]
    [SerializeField]  // 序列化私有字段
    private int maxHealth;

    [SerializeField]  // 序列化私有字段
    private float speed;

    [Space(10)]  // 在Inspector中添加10像素的间距
    [Header("战斗属性")]
    public int currentHealth;  // 公有字段（默认序列化）
    public int damage;         // 公有字段（默认序列化）

    // 不显示在Inspector的私有字段
    private bool isDie;        // 无特性标注，不可序列化

}
