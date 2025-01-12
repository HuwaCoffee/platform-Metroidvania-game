using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Localization;
using UnityEngine.UI;
/// <summary>
/// 管理游戏的测试类，方便开发时通过unity直接调数据
/// </summary>
public class ToggleControl : MonoBehaviour
{
    public Toggle englishToggle;
    public Toggle chineseToggle;
    public LeanLocalization leanLocalization;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(GameController.currentLanguage);
        if (GameController.currentLanguage == "Chinese")
        {
            chineseToggle.isOn = true; // 默认勾选中文
            englishToggle.isOn = false;
            // 设置初始语言为中文
            leanLocalization.SetCurrentLanguage("Chinese");
            
        }
        else if(GameController.currentLanguage == "English")
        {
            chineseToggle.isOn = false; // 默认勾选英文
            englishToggle.isOn = true;
            // 设置初始语言为英文
            leanLocalization.SetCurrentLanguage("English");
            
        }


    }

    

    // Update is called once per frame
    public void UpdateLanguage(string language)
    {
        leanLocalization.SetCurrentLanguage(language);

        if (language == "English")
        {
            Debug.Log(GameController.currentLanguage);
            GameController.currentLanguage = "English";
            chineseToggle.isOn = false;
            englishToggle.isOn = true;
        }
        else if (language == "简体中文")
        {
            GameController.currentLanguage = "Chinese";
            Debug.Log(GameController.currentLanguage);
            chineseToggle.isOn = true;
            englishToggle.isOn = false;
        }
    }
}
