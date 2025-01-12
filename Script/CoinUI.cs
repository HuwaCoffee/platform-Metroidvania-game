using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinUI : MonoBehaviour
{
    public int startCoinQuantity;//初始金币
    public Text coinQuantity;

    public static int CurrentCoinQuantity;//当前数量

    // Start is called before the first frame update
    void Start()
    {
        CurrentCoinQuantity = startCoinQuantity;
    }

    // Update is called once per frame
    void Update()
    {
        coinQuantity.text = CurrentCoinQuantity.ToString();

    }
}
