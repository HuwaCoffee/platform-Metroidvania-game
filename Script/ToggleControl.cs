using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Localization;
using UnityEngine.UI;
/// <summary>
/// ������Ϸ�Ĳ����࣬���㿪��ʱͨ��unityֱ�ӵ�����
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
            chineseToggle.isOn = true; // Ĭ�Ϲ�ѡ����
            englishToggle.isOn = false;
            // ���ó�ʼ����Ϊ����
            leanLocalization.SetCurrentLanguage("Chinese");
            
        }
        else if(GameController.currentLanguage == "English")
        {
            chineseToggle.isOn = false; // Ĭ�Ϲ�ѡӢ��
            englishToggle.isOn = true;
            // ���ó�ʼ����ΪӢ��
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
        else if (language == "��������")
        {
            GameController.currentLanguage = "Chinese";
            Debug.Log(GameController.currentLanguage);
            chineseToggle.isOn = true;
            englishToggle.isOn = false;
        }
    }
}
