using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasterEgg : MonoBehaviour
{
    public string easterEggPassword;
    public static string Password; //public��static������������������޸ģ�����һ�����ݣ���GameCOntroller�ж�������������

    public GameObject coin;
    public int coinQuantity;
    public float coinUpSpeed;
    public float interValTime;

    void Start()
    {
        Password = ""; //�������Ҳ����GameControl
    }

    // Update is called once per frame
    void Update()
    {
        if (Password == easterEggPassword)
        {
            Password = "";
            StartCoroutine(GenCoins()); //����Э��
        }
    }
    //Э��ʹ���һ������
    IEnumerator GenCoins()
    {
        for(int i = 0; i < coinQuantity; i++)
        {
            WaitForSeconds wait = new WaitForSeconds(interValTime);//ÿ�����֮��ȴ�ʱ����
            GameObject gb = Instantiate(coin, transform.position, Quaternion.identity);  //���ɵ����屾��λ�ã��Ƕ�
            Vector2 randomDirection = new Vector2(Random.Range(-0.3f, 0.3f), 1.0f);
            gb.GetComponent<Rigidbody2D>().velocity = randomDirection * coinUpSpeed;
            yield return wait;
        }
    }
}
