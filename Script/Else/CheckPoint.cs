using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 保存点脚本
public class CheckPoint : MonoBehaviour
{
   
[SerializeField] private Rigidbody2D myRigidbody;  //获取主角的位置position

[SerializeField] private float saveRadius; //存档点半径
[SerializeField] private GameObject bloodEffect; //存档点半径


public void Update(){
    //注意Transform.position默认返回的是Vector3
    float sqrDist = ((Vector2)transform.position - myRigidbody.position).sqrMagnitude; //向量的平方，比模长要好算
    if(sqrDist<saveRadius*saveRadius && Input.GetKeyDown(KeyCode.E) &&GameController.isGameAlive){
        GameController.camShake.Zoom();
        Instantiate(bloodEffect, myRigidbody.position, Quaternion.identity);
        PlayerPrefs.SetFloat("RespawnX", transform.position.x);
        PlayerPrefs.SetFloat("RespawnY", transform.position.y);
        PlayerPrefs.Save();      //  PlayerPrefs 用于持久化存储 
        HealthBar.HealthCurrent =  HealthBar.HealthMax;
        
    }
}




}
