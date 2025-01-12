using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace xyk.platform_Metroidvania
{
    public class PauseMenu : MonoBehaviour
    {


        public static bool GameIsPaused = false;
        public GameObject pauseMenuUI; // ��ͣ�˵���UI
        public Button resumeButton; // Resume ��ť
        public Button quitButton; // Quit ��ť
        public Button settingsButton; // Settings ��ť
        public Button MainButton; //��ҳ�水ť

        public PlayerController playerController; //�����ҿ��ƽű�������
        public PlayerAttack playerAttack;
        public PlayerMagic playerMagic;


        private void Start()
        {
            // Ϊ��ť��������ͣ�¼�
            AddHoverEvent(resumeButton);
            AddHoverEvent(quitButton);
            AddHoverEvent(settingsButton);
            AddHoverEvent(MainButton);



        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (GameIsPaused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }

            // ������� Enter ��
            if (Input.GetKeyDown(KeyCode.Return))
            {
                GameObject currentSelected = EventSystem.current.currentSelectedGameObject;
                if (currentSelected == resumeButton.gameObject)
                {
                    Resume();
                }
                else if (currentSelected == quitButton.gameObject)
                {
                    QuitGame();
                }
                else if (currentSelected == settingsButton.gameObject)
                {
                    Settings();
                }
                else if (currentSelected == MainButton.gameObject)
                {
                    MainMenu();
                }
            }
        }

        public void Resume()
        {
            pauseMenuUI.SetActive(false);
            Time.timeScale = 1.0f;

            playerController.enabled = true;// ������ҿ��ƽű�
            playerAttack.enabled = true;
            playerMagic.enabled = true;

            GameIsPaused = false;
        }

        public void Pause()
        {
            pauseMenuUI.SetActive(true);
            EventSystem.current.SetSelectedGameObject(resumeButton.gameObject); // Ĭ��ѡ�� Resume ��ť

            playerController.enabled = false;// ������ҿ��ƽű�
            playerAttack.enabled = false;
            playerMagic.enabled = false;

            Time.timeScale = 0.0f;  //��ֹʱ��
            GameIsPaused = true;
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public void Settings()
        {
            GameIsPaused = false;
            Time.timeScale = 1.0f;
            SceneManager.LoadScene("Settings");  //��ת����
        }

        public void MainMenu()
        {
            GameIsPaused = false;
            Time.timeScale = 1.0f;
            SceneManager.LoadScene("Menu");  //��ת����
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
                EventSystem.current.SetSelectedGameObject(button.gameObject);
            });

            trigger.triggers.Add(entryEnter);
        }
    }
}