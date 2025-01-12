using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashBin : MonoBehaviour
{

    private bool isPlayerInTrashBin=false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
            if (Input.GetButtonDown("Submit"))
        {
            Debug.Log("当前金币数：" + CoinUI.CurrentCoinQuantity);
            if (isPlayerInTrashBin)
            {
                Debug.Log("在垃圾桶附近按E");
                if (CoinUI.CurrentCoinQuantity > 0)
                {
                    SoundsManager.PlayThrowCoinClip();
                    TrashBinCoin.coinCurrent++;
                    CoinUI.CurrentCoinQuantity--;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //collision是触发器，即当前碰到的物体
        if (collision.gameObject.CompareTag("Player") && collision.GetType().ToString() == "UnityEngine.CapsuleCollider2D")
        {
            
            isPlayerInTrashBin = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        //collision是触发器，即当前碰到的物体
        if (collision.gameObject.CompareTag("Player") && collision.GetType().ToString() == "UnityEngine.CapsuleCollider2D")
        {
            isPlayerInTrashBin = false;
         
        }
    }
}
