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
        if (GameController.CoinNum != 0)
        {
            startCoinQuantity = GameController.CoinNum;
        }
        CurrentCoinQuantity = startCoinQuantity;
    }

    // Update is called once per frame
    void Update()
    {
        coinQuantity.text = CurrentCoinQuantity.ToString();

    }


     // 原有代码...
    
    public static void UpdateCoinDisplay() {
        // 更新所有货币显示UI
        CoinUI[] allCoinUI = FindObjectsOfType<CoinUI>();
        foreach (var ui in allCoinUI) {
            ui.UpdateDisplay();
        }
    }

    private void UpdateDisplay() {
        coinQuantity.text = CurrentCoinQuantity.ToString();
    }
}
