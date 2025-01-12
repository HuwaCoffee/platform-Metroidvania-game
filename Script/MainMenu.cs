using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
    public Button playButton; // Play ��ť
    public Button quitButton; // Quit ��ť
    public Button settingsButton; // Settings ��ť

    

    private void Start()
    {
        // Ϊ��ť��������ͣ�¼�
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

        // ��� PointerEnter �¼�
        EventTrigger.Entry entryEnter = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        entryEnter.callback.AddListener((eventData) =>
        {
            // ǿ�����õ�ǰ��ͣ�İ�ťΪѡ����
            EventSystem.current.SetSelectedGameObject(button.gameObject);
        });

        trigger.triggers.Add(entryEnter);
    }


    public void PlayGame()
    {
        SceneManager.LoadScene(1); // ������Ϸ����
    }

    public void QuitGame()
    {
        Application.Quit(); // �˳���Ϸ
    }

    //�л������Ľ�����
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
            float progress = operation.progress / 0.9f; //progress�Ĵ�С��Χ��Ϊ0-0.9
            slider.value = progress;
            progressText.text = Mathf.FloorToInt(progress * 100f) + "%";
            yield return null;
        }
    }

}
