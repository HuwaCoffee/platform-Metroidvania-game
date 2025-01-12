using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ScreenFlash : MonoBehaviour
{
    //public Image img;
    public float time; //闪烁时间
    public Color flashColor;//红色半透明的UI背景
    private Color defaultColor;//获取红色透明的UI背景
    // Start is called before the first frame update
    void Start()
    {
        defaultColor = GameObject.FindGameObjectWithTag("UI").GetComponent<Image>().color;
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void FlashScreen()
    {
        StartCoroutine(Flash());
    }

    IEnumerator Flash()
    {
        GameObject.FindGameObjectWithTag("UI").GetComponent<Image>().color = flashColor;
       
       
        yield return new WaitForSeconds(time);
        GameObject.FindGameObjectWithTag("UI").GetComponent<Image>().color = defaultColor;
    } 
    IEnumerator justFlash()
    {
        GameObject.FindGameObjectWithTag("UI").GetComponent<Image>().color = flashColor;
       
       
        yield return new WaitForSeconds(time);
        GameObject.FindGameObjectWithTag("UI").GetComponent<Image>().color = defaultColor;
    }
}
