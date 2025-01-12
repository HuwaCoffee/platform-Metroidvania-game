using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasterEgg : MonoBehaviour
{
    public string easterEggPassword;
    public static string Password; //public且static，可以由其他类进行修改，共用一个数据，在GameCOntroller中都是这样的数据

    public GameObject coin;
    public int coinQuantity;
    public float coinUpSpeed;
    public float interValTime;

    void Start()
    {
        Password = ""; //这个后面也放在GameControl
    }

    // Update is called once per frame
    void Update()
    {
        if (Password == easterEggPassword)
        {
            Password = "";
            StartCoroutine(GenCoins()); //启动协成
        }
    }
    //协成使金币一起生成
    IEnumerator GenCoins()
    {
        for(int i = 0; i < coinQuantity; i++)
        {
            WaitForSeconds wait = new WaitForSeconds(interValTime);//每个金币之间等待时间间隔
            GameObject gb = Instantiate(coin, transform.position, Quaternion.identity);  //生成的物体本身，位置，角度
            Vector2 randomDirection = new Vector2(Random.Range(-0.3f, 0.3f), 1.0f);
            gb.GetComponent<Rigidbody2D>().velocity = randomDirection * coinUpSpeed;
            yield return wait;
        }
    }
}
