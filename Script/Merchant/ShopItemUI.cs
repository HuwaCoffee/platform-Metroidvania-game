using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShopItemUI : MonoBehaviour {
    [SerializeField] Image iconImage;
    [SerializeField] Text nameText;
    [SerializeField] Text priceText;
    [SerializeField] GameObject highlight;

    public void Initialize(ShopItem item, bool selected) {
        iconImage.sprite = item.icon;
        nameText.text = item.itemName;
        priceText.text = $"价格: {item.price}G";
        highlight.SetActive(selected);
    }

    public void SetSelected(bool state) {
        highlight.SetActive(state);
    }
}
