using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
    public Button playButton; // Play 按钮
    public Button quitButton; // Quit 按钮
    public Button settingsButton; // Settings 按钮

    

    private void Start()
    {
        // 为按钮添加鼠标悬停事件
        AddHoverEvent(playButton);
        AddHoverEvent(quitButton);
        AddHoverEvent(settingsButton);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return))
        {
            GameObject currentSelected = EventSystem.current.currentSelectedGameObject;
            if (currentSelected == playButton.gameObject)
            {
                playButton.onClick.Invoke();
            }
            else if (currentSelected == quitButton.gameObject)
            {
                quitButton.onClick.Invoke();
            }
            else if (currentSelected == settingsButton.gameObject)
            {
                settingsButton.onClick.Invoke();
            }
        }
    }

    private void AddHoverEvent(Button button)
    {
        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

        // 添加 PointerEnter 事件
        EventTrigger.Entry entryEnter = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        entryEnter.callback.AddListener((eventData) =>
        {
            // 强制设置当前悬停的按钮为选中项
            EventSystem.current.SetSelectedGameObject(button.gameObject);
        });

        trigger.triggers.Add(entryEnter);
    }


    public void PlayGame()
    {
        SceneManager.LoadScene(1); // 加载游戏场景
    }

    public void QuitGame()
    {
        Application.Quit(); // 退出游戏
    }

    //切换场景的进度条
    public GameObject loadingScreen;
    public Text progressText;
    public Slider slider;

    public void LoadLevel(int sceneIndex)
    {
        StartCoroutine(AsyncLoadLevel(sceneIndex));
    }

    IEnumerator AsyncLoadLevel(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        loadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            float progress = operation.progress / 0.9f; //progress的大小范围是为0-0.9
            slider.value = progress;
            progressText.text = Mathf.FloorToInt(progress * 100f) + "%";
            yield return null;
        }
    }

}
