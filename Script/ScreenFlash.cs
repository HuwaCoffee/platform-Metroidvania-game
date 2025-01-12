using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ScreenFlash : MonoBehaviour
{
    //public Image img;
    public float time; //��˸ʱ��
    public Color flashColor;//��ɫ��͸����UI����
    private Color defaultColor;//��ȡ��ɫ͸����UI����
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
