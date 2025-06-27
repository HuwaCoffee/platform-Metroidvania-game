 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 using UnityEngine.UI;
 
 [System.Serializable]
public class ShopItem {
    public string itemName;
    public Sprite icon;
    public int price;
    public ItemType itemType;
    public int value; // 增益数值
}
