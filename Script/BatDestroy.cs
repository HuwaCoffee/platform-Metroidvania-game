using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatDestroy : MonoBehaviour
{
    public int batFlag; //��ʾ��ǰ�����ڲʵ��еı�� �����ܣ������Ŷ����������ַ���ʵ��
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //unity�Դ���������destroyʱ�Զ�����
    private void OnDestroy()
    {
        EasterEgg.Password += batFlag.ToString();
        Debug.Log("����"+batFlag+"������");
        Debug.Log(EasterEgg.Password);
    }
}
