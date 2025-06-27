
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.VisualScripting;

public class SceneTransitionManager : MonoBehaviour {
  public static SceneTransitionManager Instance { get; private set; }

  public ScreenFade screenFade;
  [HideInInspector] public PlayerController playerCtrl;
  [HideInInspector] public PlayerAttack playerAtk;
  [HideInInspector] public PlayerMagic playerMagic;
[HideInInspector] public Animator anim;
   

  void Awake() {
    if (Instance == null) {
      Instance = this;
      DontDestroyOnLoad(gameObject);
      DontDestroyOnLoad(screenFade.gameObject);
      SceneManager.sceneLoaded += OnSceneLoaded;
    } else Destroy(gameObject);
  }



    void OnSceneLoaded(Scene s, LoadSceneMode m)
    {
        playerCtrl = FindObjectOfType<PlayerController>();
        playerAtk = FindObjectOfType<PlayerAttack>();
        playerMagic = FindObjectOfType<PlayerMagic>();
        anim=GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
  }

  public void TransitionTo(
      string sceneName,
      string entryPointName,
      Vector2 postDir, // 应当来自触发器的预设方向
      float postDist,
      float postSpeed)
  {
    StartCoroutine(DoTransition(sceneName, entryPointName, postDir, postDist, postSpeed));
  }

  IEnumerator DoTransition(
      string sceneName,
      string entryPointName,
      Vector2 postDir,
      float postDist,
      float postSpeed)
  {
    if (playerCtrl) playerCtrl.ignoreInput = true;
    if (playerAtk) playerAtk.enabled = false;
    if (playerMagic) playerMagic.enabled = false;

    yield return screenFade.FadeOut();
    yield return SceneManager.LoadSceneAsync(sceneName);

    // 定位到入口点
    var entry = GameObject.Find(entryPointName);
    if (entry && playerCtrl) {
      playerCtrl.transform.position = entry.transform.position;

            // // 根据方向翻转角色
            // if (postDir.x > 0)
            // {
            //     playerCtrl.transform.localScale = new Vector3(-1, 1, 1);
            //     GameController.pos = true;
            // }

            // else if (postDir.x < 0)
            // {
            //     playerCtrl.transform.localScale = Vector3.one;
            //     GameController.pos = false;
            // }
                

      // 初始化强制移动参数
      playerCtrl.forcedRun = true;
      playerCtrl.forcedDir = postDir.normalized;
      playerCtrl.forcedRunSpeed = postSpeed;
    }

    // 并行处理淡入和后跑
    float remainDistance = postDist;
    var fadeInCoroutine = screenFade.FadeIn();
    
    while (remainDistance > 0.01f || fadeInCoroutine.MoveNext()) {
      if (remainDistance > 0.01f) {
        remainDistance -= postSpeed * Time.deltaTime;
      }
      yield return null;
    }

        // 结束强制移动
        if (playerCtrl)
        {
            playerCtrl.forcedRun = false;
            playerCtrl.ignoreInput = false;
      // 归位动画
anim.SetBool("isRun", false);
anim.SetTrigger("isRunning");
anim.SetBool("isIdle", true);
      
    }
    
    if (playerAtk) playerAtk.enabled = true;
    if (playerMagic) playerMagic.enabled = true;
  }
}