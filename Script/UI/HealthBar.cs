using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Text healthText;
    public static int HealthCurrent;  //只要成员变量有static，就能直接通过HealthBar.*获取，不需要在栈中声明
    public static int HealthMax;

    private Image healthBar;
    // Start is called before the first frame update
    void Start()
    {
        healthBar = GetComponent<Image>();
        //HealthCurrent = HealthMax;
    }

    // Update is called once per frame.
    void Update()
    {
        healthBar.fillAmount = (float)HealthCurrent / HealthMax;
        healthText.text = HealthCurrent.ToString() + "/" + HealthMax.ToString();
    }
}
