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
        public GameObject pauseMenuUI; // 暂停菜单的UI
        public Button resumeButton; // Resume 按钮
        public Button quitButton; // Quit 按钮
        public Button settingsButton; // Settings 按钮
        public Button MainButton; //主页面按钮

        public PlayerController playerController; //添加玩家控制脚本的引用
        public PlayerAttack playerAttack;
        public PlayerMagic playerMagic;


        private void Start()
        {
            // 为按钮添加鼠标悬停事件
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

            // 如果按下 Enter 键
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

            playerController.enabled = true;// 开启玩家控制脚本
            playerAttack.enabled = true;
            playerMagic.enabled = true;

            GameIsPaused = false;
        }

        public void Pause()
        {
            pauseMenuUI.SetActive(true);
            EventSystem.current.SetSelectedGameObject(resumeButton.gameObject); // 默认选中 Resume 按钮

            playerController.enabled = false;// 禁用玩家控制脚本
            playerAttack.enabled = false;
            playerMagic.enabled = false;

            Time.timeScale = 0.0f;  //禁止时间
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
            SceneManager.LoadScene("Settings");  //跳转场景
        }

        public void MainMenu()
        {
            GameIsPaused = false;
            Time.timeScale = 1.0f;
            SceneManager.LoadScene("Menu");  //跳转场景
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
                EventSystem.current.SetSelectedGameObject(button.gameObject);
            });

            trigger.triggers.Add(entryEnter);
        }
    }
}